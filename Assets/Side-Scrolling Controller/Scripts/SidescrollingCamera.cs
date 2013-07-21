using UnityEngine;
using System;
using System.Collections;

public class SidescrollingCamera : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	
	public bool followEntity = true;
	public GameObject trackedEntity;
	public Vector3 offset;
	public Vector3 floatSpeed;
	public Vector3 rotation;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		if(DebugMode)
			Debug.Log("Initial Rotation: " + transform.rotation.eulerAngles + Environment.NewLine);
		
		float startX = trackedEntity.transform.position.x + offset.x;
		float startY = trackedEntity.transform.position.y + offset.y;
		float startZ = trackedEntity.transform.position.z + offset.z;
		
		transform.position = new Vector3(startX, startY, startZ);
		transform.rotation = Quaternion.Euler(rotation);
		
		if(rigidbody)
			rigidbody.freezeRotation = true;
		
		if(DebugMode)
			Debug.Log("Post Setup Rotation: " + transform.rotation.eulerAngles + Environment.NewLine);
	}
	
	public void Update()
	{
		if(! followEntity)
			return;
		
		if(trackedEntity == null)
			return;
		
		float newX = Mathf.Lerp(transform.position.x, 
			                    trackedEntity.transform.position.x + offset.x, 
			                    Time.deltaTime * floatSpeed.x);
		float newY = Mathf.Lerp(transform.position.y, 
			                    trackedEntity.transform.position.y + offset.y, 
			                    Time.deltaTime * floatSpeed.y);
		float newZ = Mathf.Lerp(transform.position.z,
			                    trackedEntity.transform.position.z + offset.z,
								Time.deltaTime * floatSpeed.z);
		Vector3 target = new Vector3(newX, newY, newZ);
		
		transform.position = target;
		
		if(DebugMode)
		{
			Debug.Log("Rotation: " + transform.rotation.eulerAngles + Environment.NewLine
				      + "Position: " + transform.position + Environment.NewLine
				      + "Distance From Player: " + Vector3.Distance(transform.position, trackedEntity.transform.position));
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	#endregion Methods
}
