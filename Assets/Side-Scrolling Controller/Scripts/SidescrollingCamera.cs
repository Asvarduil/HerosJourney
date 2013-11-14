using UnityEngine;
using System;
using System.Collections;

public class SidescrollingCamera : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	
	public bool FollowEntity = true;
	public GameObject TrackedEntity;
	public string TrackedTag = "Player";
	public Vector3 offset;
	public Vector3 floatSpeed;
	public Vector3 rotation;
	
	public bool PlayerNoLongerExists 
	{ 
		get 
		{ 
			bool result = false; 
			
			try
			{
				result = TrackedEntity == null;
			}
			catch(MissingReferenceException)
			{
				result = true;
			}
			
			return result;	
		}
	}

	private AudioListener _listener;
	
	#endregion Variables / Properties
	
	#region Engine Hooks

	public void Awake()
	{
		// Enforces Master Volume.
		AudioListener.volume = Settings.soundEnabled 
			                       ? Settings.masterVolume
				                   : 0.0f;
	}
	
	public void Start()
	{
		if(DebugMode)
			Debug.Log("Initial Rotation: " + transform.rotation.eulerAngles + Environment.NewLine);
		
		TrackedEntity = GameObject.FindGameObjectWithTag(TrackedTag);
		float startX = TrackedEntity.transform.position.x + offset.x;
		float startY = TrackedEntity.transform.position.y + offset.y;
		float startZ = TrackedEntity.transform.position.z + offset.z;
		
		transform.position = new Vector3(startX, startY, startZ);
		transform.rotation = Quaternion.Euler(rotation);
		
		if(rigidbody)
			rigidbody.freezeRotation = true;
		
		if(DebugMode)
			Debug.Log("Post Setup Rotation: " + transform.rotation.eulerAngles + Environment.NewLine);
	}
	
	public void Update()
	{
		AudioListener.volume = Settings.soundEnabled ? Settings.masterVolume : 0.0f;
		if(DebugMode)
		{
			Debug.Log("Sound " + (Settings.soundEnabled ? "is" : "is not") + " enabled.\r\n"
			          + "Volume is: " + AudioListener.volume);
		}

		if(! FollowEntity)
			return;
		
		if(PlayerNoLongerExists)
			return;
		
		float newX = Mathf.Lerp(transform.position.x, 
			                    TrackedEntity.transform.position.x + offset.x, 
			                    Time.deltaTime * floatSpeed.x);
		float newY = Mathf.Lerp(transform.position.y, 
			                    TrackedEntity.transform.position.y + offset.y, 
			                    Time.deltaTime * floatSpeed.y);
		float newZ = Mathf.Lerp(transform.position.z,
			                    TrackedEntity.transform.position.z + offset.z,
								Time.deltaTime * floatSpeed.z);
		Vector3 target = new Vector3(newX, newY, newZ);
		
		transform.position = target;
		
		if(DebugMode)
		{
			Debug.Log("Rotation: " + transform.rotation.eulerAngles + Environment.NewLine
				      + "Position: " + transform.position + Environment.NewLine
				      + "Distance From Player: " + Vector3.Distance(transform.position, TrackedEntity.transform.position));
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	#endregion Methods
}
