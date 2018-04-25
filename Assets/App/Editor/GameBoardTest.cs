using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Zenject;
using NUnit.Framework;
using System.Linq;
using App.Scripts.Boards;
using NUnit.Framework.Internal;
using UniRx;
using UnityEngine;
using Random = System.Random;

[TestFixture]
public class GameBoardTest : UnitTestFixture
{
    const int width = 10;
    const int height = 10;
    const int mines = 10;

    private const int firstMoveX = 1;
    private const int firstMoveY = 1;

    [SetUp]
    public void CommonInstall()
    {
        var gameObject = new GameObject();
        var context = gameObject.AddComponent<SceneContext>();
        var gameInstaller = gameObject.AddComponent<GameInstaller>();
        var gameSetting = Resources.Load<SettingInstaller>("SettingInstaller");

        context.ScriptableObjectInstallers = new List<ScriptableObjectInstaller>() {gameSetting};
        context.Installers = new List<MonoInstaller>() {gameInstaller};
        
        context.Install();
        Container = context.Container;
    }

    [Test]
    public void Test_GameBoard_Creation()
    {
        //Arrange
        var gameBoard = Container.Resolve<IGameBoard>();
        var cells = Container.Resolve<IList<CellData>>();
        var random = Container.Resolve<Random>();
        //Act
        gameBoard.Build(width, height, mines, cells);
        gameBoard.FirstMove(1, 1, random);
        //Assert
        Assert.AreEqual(width * height, cells.Count); //total of cells
        Assert.AreEqual(mines, cells.Count(cell => cell.IsMine.Value)); //total of mines
    }
    
    [Test]
    public void Test_Rx()
    {
        //Arrange
        var rx = new BoolReactiveProperty();
        var gameObject = new GameObject();
        rx.Select(b => b.ToString()).RepeatUntilDestroy(gameObject).Subscribe(Debug.Log);
        rx.Value = true;
        rx.Value = true;
    }

    [Test]
    public void Test_GameBoard_FirstMove()
    {
        //Arrange
        var gameBoard = Container.Resolve<IGameBoard>();
        var cells = Container.Resolve<IList<CellData>>();
        var random = Container.Resolve<Random>();
        //Act
        gameBoard.Build(width, height, mines, cells);
        gameBoard.FirstMove(firstMoveX, firstMoveY, random);
        //Assert
        Assert.IsFalse(gameBoard.GetCellAt(firstMoveX, firstMoveY).IsMine.Value); //first move shouldn't get mine
    }

    [Test]
    public void Test_GameBoard_AdjacentMines()
    {
        //Arrange
        var gameBoard = Container.Resolve<IGameBoard>();
        var cells = Container.Resolve<IList<CellData>>();
        var random = Container.Resolve<Random>();
        //Act
        gameBoard.Build(width, height, mines, cells);
        gameBoard.GetCellAt(1, 1).IsMine.Value = true;
        gameBoard.GetCellAt(2, 1).IsMine.Value = false;
        gameBoard.GetCellAt(3, 1).IsMine.Value = true;
        gameBoard.GetCellAt(1, 2).IsMine.Value = false;
        gameBoard.GetCellAt(3, 2).IsMine.Value = true;
        gameBoard.GetCellAt(1, 3).IsMine.Value = false;
        gameBoard.GetCellAt(2, 3).IsMine.Value = true;
        gameBoard.GetCellAt(3, 3).IsMine.Value = false; //there are 4 mines around (2,2)

        //Assert
        Assert.AreEqual(4, gameBoard.GetNeighbors(2, 2).Count(z => z.IsMine.Value));
    }

    [Test]
    public void Test_GameBoard_Solver()
    {
        //Arrange
        var gameBoard = Container.Resolve<IGameBoard>();
        var cells = Container.Resolve<IList<CellData>>();
        var solver = Container.Resolve<IGameSolver>();
        var gameStatus = Container.Resolve<IReactiveProperty<GameStatus>>();
        //Act
        gameBoard.Build(width, height, mines, cells);
        Observable.FromCoroutine(_ => solver.Solve(0f)).Subscribe(_ =>
        {
            //Assert
            //game should finish
            ModestTree.Assert.IsEqual(true, gameStatus.Value != GameStatus.InProgress);

            if (gameStatus.Value == GameStatus.Completed) //if solver success
            {
                ModestTree.Assert.IsEqual(cells.Count(data => data.IsMine.Value),
                    cells.Count(data => data.IsFlagged.Value));
            }
            else //if solver failed
            {
                ModestTree.Assert.IsNotEqual(cells.Count(data => data.IsMine.Value),
                    cells.Count(data => data.IsFlagged.Value));
            }
        });
    }
}