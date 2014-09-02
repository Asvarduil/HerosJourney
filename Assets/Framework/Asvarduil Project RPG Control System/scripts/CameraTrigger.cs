using UnityEngine;
using System;
using System.Collections;

public class CameraTrigger : MonoBehaviour {
	
	#region Variables / Properties
	
	public Vector3 newRotation = new Vector3(25.0f, 0.0f, 0.0f);
	public float newDistance = 10.0f;
	
	private RPGCamera _Camera;
	private Vector3 _OriginalRotation;
	private float _OriginalDistance;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	// Use this for initialization
	void Start () 
	{
		_Camera = (RPGCamera) GameObject.FindObjectOfType(typeof(RPGCamera));
		if(_Camera == null)
		{
			throw new Exception("Could not find an RPG Camera in the scene!");
		}
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider who)
	{
		if(who.tag == "Player")
		{
			Debug.Log ("A player has entered the camera trigger!");
			_OriginalRotation = _Camera.transform.rotation.eulerAngles;
			_OriginalDistance = _Camera.distance;
		
			_Camera.AlterCamera(newRotation, newDistance);
		}
	}
	
	void OnTriggerExit(Collider who)
	{
		if(who.tag == "Player")
		{
			Debug.Log ("A player has left the trigger...");
			_Camera.AlterCamera(_OriginalRotation, _OriginalDistance);
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	#endregion Methods
}
