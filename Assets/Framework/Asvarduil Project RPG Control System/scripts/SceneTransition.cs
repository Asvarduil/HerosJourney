using UnityEngine;
using System.Collections.Generic;

public class SceneTransition : MonoBehaviour 
{
	#region Variables / Properties
	
	public List<string> recognizedTags;
	public Vector3 targetPosition;
	public Vector3 targetRotation;
	public int targetSceneID;
	
	private bool _TransitionInitiated = false;
	private Fader _Fader;
	private Maestro _Maestro;
	private TransitionManager _TransitionManager;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	void Start()
	{
		_Fader = (Fader) GameObject.FindObjectOfType(typeof(Fader));
		_Maestro = Maestro.DetectLastInstance();
		_TransitionManager = TransitionManager.Instance;
	}
	
	void FixedUpdate()
	{
		if(_Fader.ScreenHidden
		   && _TransitionInitiated)
		{
			_TransitionManager.ChangeScenes();
		}
	}
	
	void OnTriggerEnter(Collider who)
	{
		if(_TransitionInitiated)
			return;
		
		if(! recognizedTags.Contains(who.tag))
			return;
		
		PlayerControl controlSystem = (PlayerControl) who.GetComponent("PlayerControl");	
		controlSystem.enabled = false;
		
		_TransitionManager.AcquirePlayerPiece(who.gameObject);
		_TransitionManager.PrepareTransition(targetPosition, targetRotation, targetSceneID);
		
		_Fader.FadeOut();
		_TransitionInitiated = true;
	}
	
	#endregion Engine Hooks
}
