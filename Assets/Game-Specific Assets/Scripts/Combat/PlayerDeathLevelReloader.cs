using UnityEngine;
using System.Collections;

public class PlayerDeathLevelReloader : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	
	private bool _reloading = false;
	private Fader _fader;
	private SidescrollingCamera _camera;
	private TransitionManager _sceneChange;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_fader = (Fader) FindObjectOfType(typeof(Fader));
		_camera = (SidescrollingCamera) FindObjectOfType(typeof(SidescrollingCamera));
		_sceneChange = TransitionManager.Instance;
	}
	
	public void Update()
	{
		if(DebugMode)
			Debug.Log("Player " + (_camera.PlayerNoLongerExists ? "does not " : "does ") + "exist." );
		
		if(_camera.PlayerNoLongerExists
		   && ! _reloading)
		{
			_reloading = true;
			StartCoroutine(ReloadLevelSequence());
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private IEnumerator ReloadLevelSequence()
	{
		if(DebugMode)
			Debug.Log("Performing level reload...");
		
		_fader.FadeOut();
		while(_fader.ScreenShown)
		{
			yield return 0;
		}
		
		_sceneChange.ChangeScenes();
		_reloading = false;
	}
	
	#endregion Methods
}
