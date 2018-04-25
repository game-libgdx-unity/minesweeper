using System.Collections.Generic;
using App.Scripts.Boards;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Random = System.Random;

public class GameInstaller : MonoInstaller<GameInstaller>
{
    [Inject] GameSetting settings;

    public override void InstallBindings()
    {
        Container.BindFactory<int, int, int, CellData, CellData.Factory>().FromNew();
        Container.BindFactoryContract<ICell, IFactory<ICell>, Cell.Factory>()
            .FromComponentInNewPrefab(settings.cellPrefab);

        Container.Bind<IList<ICell>>().FromInstance(new List<ICell>()).AsSingle();
        Container.Bind<IList<CellData>>().FromInstance(new List<CellData>()).AsSingle();
        Container.Bind<IReactiveProperty<GameStatus>>().FromInstance(new ReactiveProperty<GameStatus>()).AsSingle();

        Container.Bind<Random>().FromMethod(_ => new Random()).AsTransient();
        Container.Bind<IntReactiveProperty>().FromMethod(_ => new IntReactiveProperty()).AsTransient();
        Container.Bind<BoolReactiveProperty>().FromMethod(_ => new BoolReactiveProperty()).AsTransient();

        Container.Bind<IGameBoard>().To<GameBoard>().AsSingle().NonLazy();
        Container.Bind<IGameSolver>().To<GameSolver>().AsSingle().NonLazy();

        var mapGenerator = FindObjectOfType<MapGenerator>();
        if (mapGenerator)
        {
            Container.BindInstance(mapGenerator).AsSingle();
        }
        else
        {
            Container.Bind<MapGenerator>().FromComponentInNewPrefab(settings.canvasPrefab).AsSingle().NonLazy();
        }
    }
}