using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAPOD_HME.EntitySystem;
using Microsoft.Xna.Framework;
using XNAPLUS.Squared.Tiled;
using Microsoft.Xna.Framework.Media;

namespace DAPOD_HME.Core
{
    class  SurvivalManager
    {
        private static readonly SurvivalManager INSTANCE = new SurvivalManager();
       

        private EntityFactory factory = EntityFactory.Get();
        private MusicManager musicManager = MusicManager.Get();

        public List<StaticEntity> getterList { set; get; }

        private int timer;
        private Random seed;

        private bool[] comboVoiceRem;
        private int comboCounter;
        private int comboTimer;
        private long points;
        private int comboVoiceTimer;

        public static SurvivalManager Get()
        {
            return INSTANCE;
        }
        public void Init()
        {
            comboVoiceRem = new bool[10];
            flushComboList();
            timer = 2000;
            seed = new Random();
            comboCounter = 1;
            comboTimer = 200;
            points = 0;

            //map load its always the same

            // Create List of passive Entities and add all to them
            getterList = new List<StaticEntity>();

            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.APPLE_GREEN));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.APPLE_RED));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.ORANGE));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.POWERUP));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.TREE_SMALL));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.MARIO));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.LUIGI));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.LEMON));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.ICE));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.HAT));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.DUDE));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.DOT));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.BULB));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.BERRIES));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.BANANA));
            getterList.Add(factory.CreatePassive16x16Entities(EntityTypes.Passive16x16.ARTICHOKE));

            // add more entities!

            // Create a random List of 150 passive 16x16 Entities for start up
            factory.currentLevelList = getterList;
            for (int i = 0; i < 150; i++)
            {
                factory.GenerateNewEntity();
            }

            // adding 


        }
        private SurvivalManager() { }

        public void ResetSurivalMode()
        {
            timer = 2000;
            comboCounter = 1;
            comboTimer = 200;
            points = 0;
            factory.currentLevelList = getterList;
            for (int i = 0; i < 150; i++)
            {
                factory.GenerateNewEntity();
            }
            flushComboList();
        }

        public void Update(int delta)
        {

            if (comboCounter > 1)
            {
                if (comboTimer <= 0)
                {
                    comboCounter = 1;
                    flushComboList();
                }
                else
                    comboTimer -= delta;
            }

            if (comboVoiceTimer <= 0)
                MediaPlayer.Volume = 1f;
            else
                comboVoiceTimer -= delta;

            factory.UpdateEnteties(delta);

            // add check for ending
            // System.Console.WriteLine("MaxPool: " + (int)(150 * Player.Get().RelativeSize * 2) + "| currentPool: " + factory.mapEntities.Count);

            if (timer <= 0)
            {
                //add new entity based on size of player
                if (factory.mapEntities.Count < 150 * (Player.Get().RelativeSize *2))
                {
                    factory.GenerateNewEntity();
                    //set timer smaller the greater the player is
                    timer = (int)(seed.NextDouble() * 1000 / (Player.Get().RelativeSize * 4) + 1000 / (Player.Get().RelativeSize * 4));
                }
            }
            else
                timer -= delta;
        }
        public void removeEntityAndAddPoints(int index, BasicEntity entity)
        {
            factory.mapEntities.RemoveAt(index);

            // calculate points based on combo and size of the object
            comboCounter++;
            if (comboCounter >= 999)
                comboCounter = 999;
            comboTimer = (int)(2000 / (Player.Get().RelativeSize));
            points += (int)(entity.Size * 10 * comboCounter + (int)(seed.NextDouble() * 10));
            playComboVoice();
        }

        public long GetPoints()
        {
            return points;
        }
        public int GetCombo()
        {
            return comboCounter;
        }
        public void NullCombo()
        {
            comboCounter = 0;
        }

        private void playComboVoice()
        {
            if (comboCounter % 20 == 0 && comboCounter <= 200)
            {
                if (!comboVoiceRem[comboCounter / 20 - 1])
                {
                    _playComboVoice();
                    MediaPlayer.Volume = 0.4f;
                    comboVoiceRem[comboCounter / 20 - 1] = true;
                    comboVoiceTimer = musicManager.GetDurationOfLastKey() * 1000;
                }
            }
        }
        private void _playComboVoice()
        {
            switch (comboCounter)
            {
                case 20:
                    musicManager.PlaySound("combo_nice", 1f);
                    break;
                case 40:
                    musicManager.PlaySound("combo_sweet", 1f);
                    break;
                case 60:
                    musicManager.PlaySound("combo_hattisch", 1f);
                    break;
                case 80:
                    musicManager.PlaySound("combo_sexy", 1f);
                    break;
                case 100:
                    musicManager.PlaySound("combo_awesome", 1f);
                    break;
                case 120:
                    musicManager.PlaySound("combo_spec", 1f);
                    break;
                case 140:
                    musicManager.PlaySound("combo_fantastic", 1f);
                    break;
                case 160:
                    musicManager.PlaySound("combo_mega", 1f);
                    break;
                case 180:
                    musicManager.PlaySound("combo_ultra", 1f);
                    break;
                case 200:
                    musicManager.PlaySound("combo_extreme", 1f);
                    break;
            }
        }

        private void flushComboList()
        {
            for (int i = 0; i < comboVoiceRem.Length; i++)
                comboVoiceRem[i] = false;
        }


    }
}
