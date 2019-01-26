using System.Collections;
using UnityEngine.TestTools;
using Zenject;
using System.Collections.Generic;
using App.Scripts.Boards;
using NUnit.Framework;
using UnityEngine;
using UniRx;
using Assert = ModestTree.Assert;
using Object = UnityEngine.Object;
using Random = System.Random;

[TestFixture]
public class TestUI : IntegrationTestFixture
{
    const int width = 10;
    const int height = 10;
    const int mines = 10;

    private void Install()
    {
        var gameObject = new GameObject();
        var context = gameObject.AddComponent<SceneContext>();
        var gameInstaller = gameObject.AddComponent<GameInstaller>();
        var gameSetting = Resources.Load<SettingInstaller>("SettingInstaller");

        context.ScriptableObjectInstallers = new List<ScriptableObjectInstaller>() {gameSetting};
        context.Installers = new List<MonoInstaller>() {gameInstaller};
        
        context.Install();
        SceneContext = context;
    }

    [UnityTest]
    public IEnumerator Test__GameBoard_NotNull()
    {
        PreInstall();
        Install();
        
        var gameBoard = Container.Resolve<IGameBoard>();
        
        Assert.IsNotNull(gameBoard);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_Cell_SetParent()
    {
        PreInstall();
        Install();

        var cellFactory = Container.Resolve<IFactory<ICell>>();
        var parentTransform = Container.Resolve<MapGenerator>().transform;

        var cell = cellFactory.Create() as Cell;
        cell.SetParent(parentTransform);
        
        Assert.IsEqual(cell.transform.parent.gameObject.name, parentTransform.gameObject.name);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Test_GameBoard_Build()
    {
        PreInstall();
        Install();
        
        //build the board
        var mapGenerator = Container.Resolve<MapGenerator>();
        var cellFactory = Container.Resolve<IFactory<ICell>>();
        var gameBoard = Container.Resolve<IGameBoard>();
        var cellData = Container.Resolve<IList<CellData>>();
        var cells = Container.Resolve<IList<ICell>>();
       
        gameBoard.Build(width, height, mines, cellData);

        Assert.IsNotNull(mapGenerator);
        Assert.IsNotNull(cellFactory);
        Assert.IsNotNull(gameBoard);
        Assert.IsNotNull(cellData);
        Assert.IsNotNull(cells);

        //create cells
        foreach (var data in cellData)
        {
            var cell = cellFactory.Create();
            cell.SetParent(mapGenerator.Container);
            cell.SetCellData(data);
            cells.Add(cell);
        }

        yield return null;

        Assert.IsEqual(width * height, cells.Count);
    }
}