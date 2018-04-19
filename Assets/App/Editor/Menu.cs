using UnityEditor;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
	public class MenuTest : MonoBehaviour
	{
		static string lastScenePathKey = "lastScenePath";

		[MenuItem("Tools/Vinh/Open things %`")]
		static void DoSomething()
		{
			EditorApplication.ExecuteMenuItem("Assets/Open");
		}
	}
}