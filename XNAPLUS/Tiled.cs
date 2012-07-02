/*
Squared.Tiled
Copyright (C) 2009 Kevin Gadd

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.

  Kevin Gadd kevin.gadd@gmail.com http://luminance.org/
*/
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.IO;
using System.IO.Compression;

namespace XNAPLUS.Squared.Tiled
{
    public class Tileset {
        public class TilePropertyList : Dictionary<string, string> {
        }

        public string Name;
        public int FirstTileID;
        public int TileWidth;
        public int TileHeight;
        public Dictionary<int, TilePropertyList> TileProperties = new Dictionary<int, TilePropertyList>();
        public string Image;
        protected Texture2D _Texture;
        protected int _TexWidth, _TexHeight;

        internal static Tileset Load (XmlReader reader) {
            var result = new Tileset();

            result.Name = reader.GetAttribute("name");
            result.FirstTileID = int.Parse(reader.GetAttribute("firstgid"));
            result.TileWidth = int.Parse(reader.GetAttribute("tilewidth"));
            result.TileHeight = int.Parse(reader.GetAttribute("tileheight"));

            int currentTileId = -1;

            while (reader.Read()) {
                var name = reader.Name;

                switch (reader.NodeType) {
                    case XmlNodeType.Element:
                        switch (name) {
                            case "image":
                                result.Image = reader.GetAttribute("source");
                            break;
                            case "tile":
                                currentTileId = int.Parse(reader.GetAttribute("id"));
                            break;
                            case "property": {
                                TilePropertyList props;
                                if (!result.TileProperties.TryGetValue(currentTileId, out props)) {
                                    props = new TilePropertyList();
                                    result.TileProperties[currentTileId] = props;
                                }

                                props[reader.GetAttribute("name")] = reader.GetAttribute("value");
                            } break;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }

            return result;
        }

        public TilePropertyList GetTileProperties (int index) {
            index -= FirstTileID;

            if (index < 0)
                return null;

            TilePropertyList result = null;
            TileProperties.TryGetValue(index, out result);

            return result;
        }

        public Texture2D Texture {
            get {
                return _Texture;
            }
            set {
                _Texture = value;
                _TexWidth = value.Width;
                _TexHeight = value.Height;
            }
        }

        internal bool MapTileToRect (int index, ref Rectangle rect) {
            index -= FirstTileID;

            if (index < 0)
                return false;

            int rowSize = _TexWidth / TileWidth;
            int row = index / rowSize;
            int numRows = _TexHeight / TileHeight;
            if (row >= numRows)
                return false;

            int col = index % rowSize;

            rect.X = col * TileWidth;
            rect.Y = row * TileHeight;
            rect.Width = TileWidth;
            rect.Height = TileHeight;
            return true;
        }
    }

    public class Layer {
        internal struct TileInfo {
            public Texture2D Texture;
            public Rectangle Rectangle;
        }

        public string Name;
        public int Width, Height;
        public int[] Tiles;
        internal TileInfo[] _TileInfoCache = null;

        internal static Layer Load (XmlReader reader) {
            var result = new Layer();

            result.Name = reader.GetAttribute("name");
            result.Width = int.Parse(reader.GetAttribute("width"));
            result.Height = int.Parse(reader.GetAttribute("height"));
            result.Tiles = new int[result.Width * result.Height];       

            while (!reader.EOF) {
                var name = reader.Name;

                switch (reader.NodeType) {
                    case XmlNodeType.Element:
                        if (name == "data") {
                            var encoding = reader.GetAttribute("encoding");
                            var compressor = reader.GetAttribute("compression");
                            switch (encoding) {
                                case "base64": {
                                    int dataSize = (result.Width * result.Height * 4) + 1024;
                                    var buffer = new byte[dataSize];
                                    reader.ReadElementContentAsBase64(buffer, 0, dataSize);

                                    Stream stream = new MemoryStream(buffer, false);
                                    if (compressor == "gzip")
                                        stream = new GZipStream(stream, CompressionMode.Decompress, false);

                                    using (stream)
                                    using (var br = new BinaryReader(stream)) {
                                        for (int i = 0; i < result.Tiles.Length; i++)
                                            result.Tiles[i] = br.ReadInt32();
                                    }

                                    continue;
                                };

                                default:
                                    throw new Exception("Unrecognized encoding.");
                            }
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }

                reader.Read();
            }

            return result;
        }

        public int GetTile (int x, int y) {
            if ((x < 0) || (y < 0) || (x >= Width) || (y >= Height))
                throw new InvalidOperationException();

            int index = (y * Width) + x;
            return Tiles[index];
        }

        protected void BuildTileInfoCache (List<Tileset> tilesets) {
            Rectangle rect = new Rectangle();
            var cache = new List<TileInfo>();
            int i = 1;

            next:   
            for (int t = 0; t < tilesets.Count; t++) {
                if (tilesets[t].MapTileToRect(i, ref rect)) {
                    cache.Add(new TileInfo {
                        Texture = tilesets[t].Texture,
                        Rectangle = rect
                    });
                    i += 1;
                    goto next;
                }
            }

            _TileInfoCache = cache.ToArray();
        }

        public void Draw (SpriteBatch batch, List<Tileset> tilesets, Rectangle rectangle, Vector2 viewportPosition, int tileWidth, int tileHeight) {
            int i = 0;
            Vector2 destPos = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 viewPos = viewportPosition;

            int minX = (int)Math.Floor(viewportPosition.X / tileWidth);
            int minY = (int)Math.Floor(viewportPosition.Y / tileHeight);
            int maxX = (int)Math.Ceiling((rectangle.Width + viewportPosition.X) / tileWidth);
            int maxY = (int)Math.Ceiling((rectangle.Height + viewportPosition.Y) / tileHeight);

            if (minX < 0)
                minX = 0;
            if (minY < 0)
                minY = 0;
            if (maxX >= Width)
                maxX = Width - 1;
            if (maxY >= Height)
                maxY = Height - 1;

            if (viewPos.X > 0) {
                viewPos.X = ((int)Math.Floor(viewPos.X)) % tileWidth;
            } else {
                viewPos.X = (float)Math.Floor(viewPos.X);
            }

            if (viewPos.Y > 0) {
                viewPos.Y = ((int)Math.Floor(viewPos.Y)) % tileHeight;
            } else {
                viewPos.Y = (float)Math.Floor(viewPos.Y);
            }

            TileInfo info = new TileInfo();
            if (_TileInfoCache == null)
                BuildTileInfoCache(tilesets);

            for (int y = minY; y <= maxY; y++) {
                destPos.X = rectangle.Left;

                for (int x = minX; x <= maxX; x++) {
                    i = (y * Width) + x;
                    int index = Tiles[i] - 1;

                    if ((index >= 0) && (index < _TileInfoCache.Length)) {
                        info = _TileInfoCache[index];
                        batch.Draw(info.Texture, destPos - viewPos, info.Rectangle, Color.White);
                    }
                    
                    destPos.X += tileWidth;
                }

                destPos.Y += tileHeight;
            }
        }
    }

    public class Map {
        public List<Tileset> Tilesets = new List<Tileset>();
        public List<Layer> Layers = new List<Layer>();
        public int Width, Height;
        public int TileWidth, TileHeight;

        public static Map Load (string filename, ContentManager content) {
            var result = new Map();

            using (var stream = System.IO.File.OpenText(filename))
            using (var reader = XmlReader.Create(stream))
            while (reader.Read()) {
                var name = reader.Name;

                switch (reader.NodeType) {
                    case XmlNodeType.DocumentType:
                        if (name != "map")
                            throw new Exception("Invalid map format");
                        break;
                    case XmlNodeType.Element:
                        switch (name) {
                            case "map": {
                                result.Width = int.Parse(reader.GetAttribute("width"));
                                result.Height = int.Parse(reader.GetAttribute("height"));
                                result.TileWidth = int.Parse(reader.GetAttribute("tilewidth"));
                                result.TileHeight = int.Parse(reader.GetAttribute("tileheight"));
                            } break;
                            case "tileset": {
                                using (var st = reader.ReadSubtree()) {
                                    st.Read();
                                    var tileset = Tileset.Load(st);
                                    result.Tilesets.Add(tileset);
                                }
                            } break;
                            case "layer": {
                                using (var st = reader.ReadSubtree()) {
                                    st.Read();
                                    var layer = Layer.Load(st);
                                    result.Layers.Add(layer);
                                }
                            } break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                }
            }

            foreach (var tileset in result.Tilesets) {
                tileset.Texture = content.Load<Texture2D>(
                    Path.Combine(Path.GetDirectoryName(tileset.Image), Path.GetFileNameWithoutExtension(tileset.Image))
                );
            }

            return result;
        }

        public void Draw (SpriteBatch batch, Rectangle rectangle, Vector2 viewportPosition) {
            for (int i = 0; i < Layers.Count; i++)
                Layers[i].Draw(batch, Tilesets, rectangle, viewportPosition, TileWidth, TileHeight);
        }
    }
}
