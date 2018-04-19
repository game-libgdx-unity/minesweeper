//using Zenject;
//using System.Collections;
//using System.Collections.Generic;
//using App.Scripts.Boards;
//using ModestTree;
//using UniRx;
//using UnityEngine;
//using UnityEngine.TestTools;
//using Random = System.Random;
//
//public class BoardIntegrationTest : IntegrationTestFixture
//{
//    const int width = 10;
//    const int height = 10;
//    const int mines = 10;
//
//    private const int firstMoveX = 1;
//    private const int firstMoveY = 1;
//    
//    [UnityTest]
//    public IEnumerator RunTest1()
//    {
//        PreInstall();
//
//        Container.BindFactory<int, int, int, CellData, CellData.Factory>().FromNew();
//        Container.BindFactoryContract<ICell, IFactory<ICell>, Cell.Factory>()
//            .FromComponentInNewPrefabResource("cellPrefab");
//        Container.Bind<IList<ICell>>().FromInstance(new List<ICell>()).AsSingle();
//        Container.Bind<IList<CellData>>().FromInstance(new List<CellData>()).AsSingle();
//        Container.Bind<IReactiveProperty<GameStatus>>().FromInstance(new ReactiveProperty<GameStatus>()).AsSingle();
//        Container.Bind<Random>().FromMethod(_ => new Random()).AsTransient();
//        Container.Bind<IGameBoard>().To<GameBoard>().AsSingle().NonLazy();
//        Container.Bind<IGameSolver>().To<GameSolver>().AsSingle().NonLazy();
//        Container.Bind<MapGenerator>().FromComponentInNewPrefabResource("canvasPrefab").AsSingle().NonLazy();
//
//        PostInstall();
//
//        //build the board
          //        var mapGenerator = Container.Resolve<MapGenerator>();
          //        var cellFactory = Container.Resolve<Cell.Factory>();
          //        var gameBoard = Container.Resolve<IGameBoard>();
          //        var cellData = Container.Resolve<IList<CellData>>();
          //        var cells = Container.Resolve<IList<ICell>>();
          //        gameBoard.Build(width, height, mines, cellData);
          //        //create cells
          //        foreach (var data in cellData)
          //        {
          //            var cell = cellFactory.Create();
          //            cell.SetParent(mapGenerator.Container);
          //            cell.SetCellData(data);
          //            cells.Add(cell);
          //        }
          //
          //        yield return new WaitForSeconds(2f);
          //
          //        Assert.IsEqual(width * height, cells.Count);
          //        
//        yield break;
//    }
//}