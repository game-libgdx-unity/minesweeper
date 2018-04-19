﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Boards;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// UI implementation for cells
/// </summary>
[SelectionBase]
public class Cell : MonoBehaviour, ICell
{
    private CellData cellData { get; set; }
    private ReactiveProperty<CellType> cellType = new ReactiveProperty<CellType>();
    private Subject<PointerEventData> OnImageClick = new Subject<PointerEventData>();
    private Text textUI;
    private Image background;
    private Outline outline;

    private void Start()
    {
        //inject some components
        background = GetComponent<Image>();
        outline = GetComponent<Outline>();
        textUI = GetComponentInChildren<Text>();

        outline.effectColor = Color.black;

        //only allow to click if this cell hasn't opened yet.
        background.OnPointerClickAsObservable()
            .Where(args => cellType.Value == CellType.UNOPENED)
            .Subscribe(args => OnImageClick.OnNext(args))
            .AddTo(gameObject);

        //when number of adjacent mines get changed
        cellData.AdjacentMines
            .Where(mines => mines > 0)
            .Subscribe(mines => { cellType.Value = (CellType) mines; })
            .AddTo(this);
        
        cellData.IsFlagged
            .Where(isFlagged => isFlagged)
            .Subscribe(isFlagged => { cellType.Value = CellType.FLAGGED; })
            .AddTo(this);
        
        cellData.IsRevealed
            .Subscribe(isRevealed =>
            {
                textUI.enabled = isRevealed;
                background.color = isRevealed ? Color.white : Color.gray;
            })
            .AddTo(this);

        cellData.IsMine
            .Where(isMine => isMine)
            .SelectMany(cellData.IsRevealed)
            .Where(isRevealed=>isRevealed)
            .Subscribe(isMined => { cellType.Value = CellType.MINE; })
            .AddTo(this);

        //change UI when cellType change
        cellType.Where(c => c != CellType.UNOPENED)
            .Subscribe(type =>
            {
                textUI.color = Color.black;
                switch (type)
                {
                    case global::CellType.EMPTY:
                        textUI.text = "";
                        break;
                    case global::CellType.M1:
                        textUI.text = "1";

                        break;
                    case global::CellType.M2:
                        textUI.text = "2";

                        break;
                    case global::CellType.M3:
                        textUI.text = "3";

                        break;
                    case global::CellType.M4:
                        textUI.text = "4";

                        break;
                    case global::CellType.M5:
                        textUI.text = "5";

                        break;
                    case global::CellType.M6:
                        textUI.text = "6";

                        break;
                    case global::CellType.M7:
                        textUI.text = "7";

                        break;
                    case global::CellType.M8:
                        textUI.text = "8";

                        break;
                    case global::CellType.MINE:
                        textUI.text = "M";
                        textUI.color = Color.red;
                        textUI.enabled = true;

                        break;
                    case global::CellType.FLAGGED:
                        textUI.text = "F";
                        textUI.color = Color.blue;
                        textUI.enabled = true;

                        break;
                }
            })
            .AddTo(gameObject);
    }

    public void SetCellData(CellData data)
    {
        this.cellData = data;
    }

    public CellData GetCellData()
    {
        return cellData;
    }

    public void SetParent(Transform parent)
    {
        this.transform.SetParent(parent);
    }

    public class Factory : Factory<ICell>
    {
    }
}