using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = true;
	public string AffectedTag = "Player";
	public GUISkin Skin;	
	public FloatingButton EnterButton;
	public string SceneName;
	public Vector3 ScenePosition;
	
	private bool _showGUI = false;
	private TransitionManager _sceneChange;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_sceneChange = TransitionManager.Instance;
	}
	
	public void OnGUI()
	{
		if(! _showGUI)
			return;
		
		GUI.skin = Skin;
		if(EnterButton.IsClicked())
		{
			if(DebugMode)
				Debug.Log("Enter button was clicked for Door: " + gameObject.name);
			
			CheckIfTransitionOccurred();
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
		
		_showGUI = true;
	}
	
	public void OnTriggerExit(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		_showGUI = false;
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private void CheckIfTransitionOccurred()
	{	
		if(DebugMode)
		{
			Debug.Log("Executing scene transition from Door: " + gameObject.name);
			Debug.Log("Transitioning to: " + SceneName + " at " + ScenePosition);
		}
		
		_sceneChange.PrepareTransition(ScenePosition, Vector3.zero, SceneName);
		_sceneChange.ChangeScenes();
	}
	
	#endregion Methods
}
