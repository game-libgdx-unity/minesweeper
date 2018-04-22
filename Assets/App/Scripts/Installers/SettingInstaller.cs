using App.Scripts.Boards;
using UnityEngine;
using Zenject;

public class SettingInstaller : ScriptableObjectInstaller<SettingInstaller>
{
    [SerializeField] private GameSetting gameSetting;
    
    public override void InstallBindings()
    {
        Container.BindInstance(gameSetting);
    }
}