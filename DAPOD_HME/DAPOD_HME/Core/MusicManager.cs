using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace DAPOD_HME.Core
{
    class MusicManager
    {
        private static readonly MusicManager INSTANCE = new MusicManager();

        private Dictionary<string, SoundEffect> soundeffects;       // normal soundeffects
        private SoundEffect eatSound;
        private Song gamePlayBg, menuBg, WinBg;
        private string lastKey;

        public static MusicManager Get()
        {
            return INSTANCE;
        }

        public void Init(ContentManager content) 
        {
            // load all the sounds we need
            soundeffects = new Dictionary<string, SoundEffect>();

            soundeffects.Add("enter", content.Load<SoundEffect>("Sounds/Enter"));
            soundeffects.Add("cursor", content.Load<SoundEffect>("Sounds/Cursor"));
            soundeffects.Add("combo", content.Load<SoundEffect>("Sounds/Combo"));
            soundeffects.Add("dead", content.Load<SoundEffect>("Sounds/Dead"));
            soundeffects.Add("start", content.Load<SoundEffect>("Sounds/Start"));
            soundeffects.Add("extraLife", content.Load<SoundEffect>("Sounds/Extra_Life"));
            soundeffects.Add("combo_nice", content.Load<SoundEffect>("Sounds/Combo_Nice"));
            soundeffects.Add("combo_sweet", content.Load<SoundEffect>("Sounds/Combo_Sweet"));
            soundeffects.Add("combo_hattisch", content.Load<SoundEffect>("Sounds/Combo_Hattisch"));
            soundeffects.Add("combo_sexy", content.Load<SoundEffect>("Sounds/Combo_Sexy"));
            soundeffects.Add("combo_awesome", content.Load<SoundEffect>("Sounds/Combo_Awesome"));
            soundeffects.Add("combo_spec", content.Load<SoundEffect>("Sounds/Combo_Spec"));
            soundeffects.Add("combo_fantastic", content.Load<SoundEffect>("Sounds/Combo_Fantastic"));
            soundeffects.Add("combo_mega", content.Load<SoundEffect>("Sounds/Combo_Mega"));
            soundeffects.Add("combo_ultra", content.Load<SoundEffect>("Sounds/Combo_Ultra"));
            soundeffects.Add("combo_extreme", content.Load<SoundEffect>("Sounds/Combo_Extreme"));

            eatSound = content.Load<SoundEffect>("Sounds/Fruit_Eat");

            // load bgMusic and tell the MediaPlayer to repeat the song.
            //gamePlayBg = content.Load<Song>("Sounds/GameBG");
            //menuBg = content.Load<Song>("Sounds/MenuBG");
            WinBg = content.Load<Song>("Sounds/GameWon");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.IsShuffled = false;
        }
        private MusicManager() { }


        public void PlayEat()
        {
            eatSound.Play(1, 1 - (Player.Get().RelativeSize / 10), 0);
        }
        public void PlaySound(string key, float volume)
        {
            lastKey = key;
            soundeffects[key].Play(volume,0,0);
        }
        public int GetDurationOfLastKey()
        {
            return soundeffects[lastKey].Duration.Seconds;
        }
        public void PlayGameplayMusic()
        {
            //MediaPlayer.Play(gamePlayBg);
        }
        public void PlayMenuMusic()
        {
            //MediaPlayer.Play(menuBg);
        }
        public void PlayWinMusic()
        {
            MediaPlayer.Play(WinBg);
        }
        public bool isSongStopped()
        {
            return MediaPlayer.State == MediaState.Stopped;
        }


    }
}
