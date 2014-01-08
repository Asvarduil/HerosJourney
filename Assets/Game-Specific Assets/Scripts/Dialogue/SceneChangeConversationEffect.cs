using UnityEngine;
using System.Collections;

public class SceneChangeConversationEffect : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;
	public string SceneName;
	public Vector3 ScenePosition;

	private Fader _fader;
	private TransitionManager _sceneChange;

	#endregion Variables / Properties

	#region Engine Hooks

	public void Start()
	{
		_fader = (Fader) GameObject.FindObjectOfType(typeof(Fader));
		_sceneChange = TransitionManager.Instance;
	}

	#endregion Engine Hooks

	#region Exposed Functionality

	public void ChangeScenes()
	{
		StartCoroutine(BeginTransition());
	}

	#endregion Exposed Functionality

	#region Methods

	private IEnumerator BeginTransition()
	{		
		if(DebugMode)
		{
			Debug.Log("Executing scene transition from conversation event...");
			Debug.Log("Transitioning to: " + SceneName + " at " + ScenePosition);
		}
		
		_fader.FadeOut();
		while(_fader.ScreenShown)
		{
			yield return 0;
		}
		
		_sceneChange.PrepareTransition(ScenePosition, Vector3.zero, SceneName);
		_sceneChange.ChangeScenes();
	}

	#endregion Methods
}
