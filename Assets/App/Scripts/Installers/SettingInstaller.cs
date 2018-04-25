using App.Scripts.Boards;
using UnityEngine;
using Zenject;

public class SettingInstaller : ScriptableObjectInstaller<SettingInstaller>
{
    public GameSetting gameSetting;
    
    public override void InstallBindings()
    {
        Container.BindInstance(gameSetting);
    }
}