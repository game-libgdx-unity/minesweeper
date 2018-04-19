using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "SettingInstaller", menuName = "Installers/SettingInstaller")]
public class SettingInstaller : ScriptableObjectInstaller<SettingInstaller>
{
    public override void InstallBindings()
    {
    }
}