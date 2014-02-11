using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IPausableEntity 
{
	#region Variables / Properties
	
	public bool DebugMode = true;
	public bool TransitOnTriggerEntry = false;
	public string AffectedTag = "Player";
	public GUISkin Skin;	
	public FloatingButton EnterButton;
	public string SceneName;
	public Vector3 ScenePosition;
	
	private bool _playerInTrigger = false;
	private bool _doorIsPaused = false;
	private Fader _fader;
	private TransitionManager _sceneChange;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_fader = (Fader) FindObjectOfType(typeof(Fader));
		_sceneChange = TransitionManager.Instance;
	}
	
	public void OnGUI()
	{
		if(! _playerInTrigger
		   || _doorIsPaused)
			return;

		GUI.skin = Skin;
		if(TransitOnTriggerEntry)
		{
			if(DebugMode)
				Debug.Log("Due to Transit on Trigger Entry flag, transiting from door " + gameObject.name);

			StartCoroutine(BeginTransition());
			return;
		}

		if(EnterButton.IsClicked()
		   || Input.GetButtonUp("Interact"))
		{
			if(DebugMode)
				Debug.Log("Enter button was clicked for Door: " + gameObject.name);
			
			StartCoroutine(BeginTransition());
		}
	}
	
	public void Update()
	{		
		EnterButton.CalculatePosition(transform.position);
	}
	
	public void OnTriggerStay(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		_playerInTrigger = true;
	}
	
	public void OnTriggerExit(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		_playerInTrigger = false;
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private IEnumerator BeginTransition()
	{		
		if(DebugMode)
		{
			Debug.Log("Executing scene transition from Door: " + gameObject.name);
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

	#region Implementation of IPausableEntity
	
	public void PauseThisEntity()
	{
		if(DebugMode)
			Debug.Log("Pausing Door interface!");
		
		_doorIsPaused = true;
	}
	
	public void ResumeThisEntity()
	{
		if(DebugMode)
			Debug.Log("Resuming Door interface!");
		
		_doorIsPaused = false;
	}
	
	#endregion Implementation of IPausableEntity
}
