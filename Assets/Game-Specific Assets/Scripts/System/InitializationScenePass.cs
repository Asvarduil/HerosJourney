using UnityEngine;
using System.Collections;

public class InitializationScenePass : MonoBehaviour 
{
	#region Variables / Properties
	
	public string NextSceneName;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start () 
	{
		Application.LoadLevel(NextSceneName);
	}
	
	#endregion Engine Hooks
}
