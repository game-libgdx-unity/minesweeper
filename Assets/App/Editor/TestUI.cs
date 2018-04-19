using System;
using System.Collections;
using UnityEngine.TestTools;
using Zenject;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Boards;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        Container.Bind<IReactiveProperty<GameStatus>>().FromInstance(new ReactiveProperty<GameStatus>()).AsSingle();
        Container.Bind<IntReactiveProperty>().FromInstance(new IntReactiveProperty()).AsTransient();
        Container.Bind<BoolReactiveProperty>().FromMethod(_ => new BoolReactiveProperty()).AsTransient();
        Container.BindFactory<int, int, int, CellData, CellData.Factory>().FromNew();
        Container.BindFactoryContract<ICell, IFactory<ICell>, Cell.Factory>()
            .FromComponentInNewPrefabResource("cellPrefab");
        Container.Bind<IList<ICell>>().FromInstance(new List<ICell>()).AsSingle();
        Container.Bind<IList<CellData>>().FromInstance(new List<CellData>()).AsSingle();
        Container.Bind<Random>().FromMethod(_ => new Random()).AsTransient();
        Container.Bind<IGameBoard>().To<GameBoard>().AsSingle().NonLazy();
        Container.Bind<IGameSolver>().To<GameSolver>().AsSingle().NonLazy();
        Container.Bind<MapGenerator>().FromComponentInNewPrefabResource("canvasPrefab").AsSingle().NonLazy();
    }

    [UnityTest]
    public IEnumerator Test_GameBoard_SetupFromPrefab()
    {
        GameObject mainCamera = Object.Instantiate(Resources.Load<GameObject>("Main Camera"));
        SceneContext = mainCamera.GetComponent<SceneContext>();
        SceneContext.Install();
        
        yield return null;

        var gameBoard = Container.Resolve<IGameBoard>();
        
        Assert.IsNotNull(gameBoard);
        Assert.IsNotNull(Container);
    }

    [UnityTest]
    public IEnumerator Test_GameBoard_NotNull()
    {
        PreInstall();
        Install();
        
        var gameBoard = Container.Resolve<IGameBoard>();

        PostInstall();

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
        
        PostInstall();

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
        
        PostInstall();
       
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


public abstract class IntegrationTestFixture
{
    SceneContext _sceneContext;

    List<GameObject> _unityTestRunnerObjects;

    bool _hasEndedInstall;
    bool _hasStartedInstall;
    bool _hasDestroyedAll;

    protected DiContainer Container
    {
        get
        {
            return _sceneContext.Container;
        }
    }

    protected SceneContext SceneContext
    {
        get
        {
            return _sceneContext;
        }
        set
        {
            _sceneContext = value;
        }
    }

    [SetUp]
    public virtual void Setup()
    {
        // Only need to record this once for each set of tests
        if (_unityTestRunnerObjects == null)
        {
            _unityTestRunnerObjects = SceneManager.GetActiveScene()
                .GetRootGameObjects().ToList();
        }
    }

    protected void SkipInstall()
    {
        PreInstall();
        PostInstall();
    }

    protected void PreInstall()
    {
        _hasStartedInstall = true;

        Debug.Log(ProjectContext.Instance);

        var go = new GameObject();
        _sceneContext = go.AddComponent<SceneContext>();
        
        _sceneContext.Install();
    }

    bool CurrentTestHasAttribute<T>()
        where T : Attribute
    {
        // tests with double parameters need to have their () removed first
        var name = TestContext.CurrentContext.Test.FullName;

        // Remove all characters after the first open bracket if there is one
        int openBracketIndex = name.IndexOf("(");

        if (openBracketIndex != -1)
        {
            name = name.Substring(0, openBracketIndex);
        }

        // Now we can get the substring starting at the last '.'
        name = name.Substring(name.LastIndexOf(".") + 1);

        return this.GetType().GetMethod(name).GetCustomAttributes(true)
            .Cast<Attribute>().OfType<T>().Any();
    }

    protected void PostInstall()
    {
        _hasEndedInstall = true;
        _sceneContext.Resolve();

        Container.Inject(this);

        if (Container.IsValidating)
        {
            Container.ValidateValidatables();
        }
        else
        {
            // This is necessary because otherwise MonoKernel is not started until later
            // and therefore IInitializable objects are not initialized
            Container.Resolve<MonoKernel>().Initialize();
        }
    }

    protected void DestroyAll()
    {
        // We need to use DestroyImmediate so that all the IDisposable's etc get processed immediately before
        // next test runs
        GameObject.DestroyImmediate(_sceneContext.gameObject);
        _sceneContext = null;

        var allRootObjects = new List<GameObject>();

        // We want to clear all objects across all scenes to ensure the next test is not affected
        // at all by previous tests
        // TODO: How does this handle cases where the test loads other scenes?
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            allRootObjects.AddRange(
                SceneManager.GetSceneAt(i).GetRootGameObjects());
        }

        // We also want to destroy any objects marked DontDestroyOnLoad, especially ProjectContext
        allRootObjects.AddRange(ProjectContext.Instance.gameObject.scene.GetRootGameObjects());

        foreach (var rootObj in allRootObjects)
        {
            // Make sure not to destroy the unity test runner objects that it adds
            if (!_unityTestRunnerObjects.Contains(rootObj))
            {
                // Use DestroyImmediate for other objects too just to ensure we are fully
                // cleaned up before the next test starts
                GameObject.DestroyImmediate(rootObj);
            }
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (!_hasDestroyedAll)
        {
            DestroyAll();
        }

        _hasStartedInstall = false;
        _hasEndedInstall = false;
        _hasDestroyedAll = false;
    }
}