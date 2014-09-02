using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour 
{
	#region Variables / Properties
	
	public Vector3 targetPosition;
	public Vector3 targetRotation;
	
	private bool _Teleporting = false;
	private Fader _Fader;
	private GameObject _Piece;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	void Start()
	{
		_Fader = (Fader) GameObject.FindObjectOfType(typeof(Fader));
	}
	
	void FixedUpdate()
	{
		if(_Fader.ScreenHidden
		   && _Teleporting)
		{
			PerformTeleport();
			_Fader.FadeIn();
		}
	}
	
	void OnTriggerEnter(Collider who)
	{
		Debug.Log("An entity has entered the teleport trigger!");
		
		_Teleporting = true;
		_Piece = who.gameObject;
		_Fader.FadeOut();
		
		PlayerControl control = (PlayerControl) _Piece.GetComponent("PlayerControl");
		if(control != null)
			control.enabled = false;
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private void PerformTeleport()
	{
		_Piece.transform.position = targetPosition;
		_Piece.transform.rotation = Quaternion.Euler(targetRotation);
		
		PlayerControl control = (PlayerControl) _Piece.GetComponent("PlayerControl");
		if(control != null)
			control.enabled = true;
		
		_Teleporting = false;
	}
	
	#endregion Methods
}
