﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Boards;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Transform container;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int mineCount;
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private Button btnRestart;

    [Inject] private IGameBoard gameBoard;
    [Inject] private IFactory<ICell> cellFactory;
    [Inject] private IList<ICell> cells;
    [Inject] private IList<CellData> cellData;
    [Inject] private IGameSolver gameSolver;
    [Inject] private IReactiveProperty<GameStatus> gameStatus;

    public Transform Container
    {
        get { return container; }
    }

    public void Start()
    {
        //setup game status
        gameStatus.Subscribe(status =>
            {
                print("Game status: "+gameStatus.Value.ToString());
                btnRestart.gameObject.SetActive(status != GameStatus.InProgress);
            })
            .AddTo(gameObject);

        //setup button restart
        btnRestart.OnClickAsObservable()
            .Subscribe(unit =>
            {
                SceneManager.LoadScene(0); //restart the game
            })
            .AddTo(gameObject);

        //setup the layout
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = width;

        //build the board
        gameBoard.Build(width, height, mineCount, cellData);

        //create cells
        foreach (var data in cellData)
        {
            var cell = cellFactory.Create();
            cell.SetParent(container);
            cell.SetCellData(data);
            cells.Add(cell);
        }

        //solve the game
        Observable.FromCoroutine(_ => gameSolver.Solve(1f)).Subscribe(_ => { print("Finished"); })
            .AddTo(this);
    }
}