using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Irrelevant.Assets.Scripts
{
    public enum Level
    {
        Intro    = 0,
        Tutorial = 1,
        Actual   = 2,
        Death    = 3,
    }

    /// <summary>
    /// Gamestate persisting between scenes.
    /// </summary>
    public sealed class PersistentGameState
    {
        public Level Level
        {
            get { return (Level)Application.loadedLevel; }
        }

        public void LoadDeath()
        {
            // TODO: Load death, not intro.
            //Application.LoadLevel((int)Level.Death);
            LoadIntro();
        }

        public void LoadIntro()
        {
            Application.LoadLevel((int) Level.Intro);
        }

        public void LoadNextLevel()
        {
            Level nextLevel = GetNextLevel(Level);
            Application.LoadLevel((int) nextLevel);
        }

        public bool IsLastWave(int waveIndex)
        {
            return Level == Level.Tutorial && waveIndex == 2;
        }

        public int GetWaveSize(int waveIndex)
        {
            int[] levelWaveSizes = waveSizes[GetLevelIndex(Level)];
            return levelWaveSizes[Math.Min(waveIndex, levelWaveSizes.Length - 1)];
        }

        public float GetWaveTime(int waveIndex)
        {
            float[] levelWaveTimes = waveTimes[GetLevelIndex(Level)];
            return levelWaveTimes[Math.Min(waveIndex, levelWaveTimes.Length - 1)];
        }

        private static int GetLevelIndex(Level currentLevel)
        {
            return (int)currentLevel - (int)Level.Tutorial;
        }

        private static Level GetNextLevel(Level currentLevel)
        {
            switch (currentLevel)
            {
                case Level.Intro:
                    return Level.Tutorial;
                case Level.Tutorial:
                    return Level.Actual;
                case Level.Actual:
                    // TODO: Victory screen!
                    return Level.Intro;
                default:
                    return Level.Intro;
            }
        }

        private int[][] waveSizes = new[] { 
            new[] { 16, 32, },
            new[] { 32, 32, 32, 32, 40 } 
        };
        
        private float[][] waveTimes = new[] { 
            new float[] { 16, 12, },
            new float[] { 16, 12,  9,  8,  8 }
        };
    }
}
