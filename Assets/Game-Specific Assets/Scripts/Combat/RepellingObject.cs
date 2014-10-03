using UnityEngine;
using System.Collections;

public class RepellingObject : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = true;
	public float RepelForce = 26.5f;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void RepelAttacker(GameObject source)
	{
		GameObject root = source.transform.root.gameObject;
		if(root == null)
		{
			return;
		}
		
		SidescrollingMovement movement = root.GetComponent<SidescrollingMovement>();
		if(movement == null)
		{
			return;
		}

		movement.RepelFromObject(gameObject, RepelForce);
	}
	
	#endregion Methods
}
