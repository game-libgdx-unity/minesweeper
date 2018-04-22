using UnityEngine;
using System;
namespace App.Scripts.Boards
{
    [Serializable]
    public class GameSetting
    {
        public int Width;
        public int Height;
        public int MineCount;
            
        public GameObject cellPrefab;
        public GameObject canvasPrefab;
    }
}