﻿﻿using System;
using System.Collections.Generic;

namespace App.Scripts.Boards
{
    public interface IGameBoard : IBoard
    {
        void Build(int width, int height, int mines, IList<CellData> cells);
        CellData GetCellAt(int firstMoveX, int firstMoveY);
        void Open(int randomX, int randomY);
        List<CellData> GetNeighbors(int x, int y, int depth = 1);
       
        void FirstMove(int randomX, int randomY, Random random);
        void Flag(int neighborX, int neighborY);
    }
}