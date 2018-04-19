﻿﻿using UniRx;
using Zenject;

public class CellData
{
    public int ID { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    [Inject] public IntReactiveProperty AdjacentMines { get; set; }
    [Inject] public BoolReactiveProperty IsRevealed { get; set; }
    [Inject] public BoolReactiveProperty IsMine { get; set; }
    [Inject] public BoolReactiveProperty IsFlagged { get; set; }

    public CellData(int id, int x, int y)
    {
        ID = id;
        X = x;
        Y = y;
    }

    public class Factory : Factory<int, int, int, CellData>
    {
    }
}