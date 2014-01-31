using UnityEngine;
using System.Collections;

public class RepellingObject : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = true;
	public float RepulsionForce = 2.5f;
	public Vector3 Constraints = new Vector3(1, 1, 0);
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void RepelAttacker(GameObject source)
	{
		GameObject root = source.transform.root.gameObject;
		if(root == null)
		{
			if(DebugMode)
				Debug.LogWarning("There was no damage source detected on the colliding object!");
			
			return;
		}
		
		SidescrollingMovement movement = root.GetComponent<SidescrollingMovement>();
		if(movement == null)
		{
			if(DebugMode)
				Debug.LogWarning("There was no Sidescrolling Movement detected on the colliding object.");
			
			return;
		}
		
		Vector3 repelForce = (root.transform.position - transform.position).normalized * RepulsionForce;
		repelForce.Scale(Constraints);
		
		if(DebugMode)
			Debug.Log(gameObject.name + " is repelling " + root.name + " with force: " + repelForce);
		
		movement.AddForce(repelForce);
	}
	
	#endregion Methods
}
