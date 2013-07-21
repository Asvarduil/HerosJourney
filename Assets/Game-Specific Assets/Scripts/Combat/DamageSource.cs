using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DamageSource : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public AudioClip DamageSound;
	public int AttackPower = 1;
	public float DamageForce = 30f;
	public Vector3 Constraints = new Vector3(1, 1, 0);
	public List<string> AffectedTags = new List<string>{"Enemy"};
	
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_maestro = Maestro.DetectLastInstance();
	}
	
	public void OnTriggerEnter(Collider collisionEvent)
	{
		if(! AffectedTags.Contains(collisionEvent.collider.tag))
			return;
		
		if(DamageSound != null)
			_maestro.PlaySoundEffect(DamageSound);
		
		if(DebugMode)
			Debug.Log("Game Object: " + collisionEvent.gameObject.name + " was in the hitbox.");
		
		RepelDamagedEntity(collisionEvent.gameObject);
		collisionEvent.gameObject.SendMessage("TakeDamage", AttackPower, SendMessageOptions.DontRequireReceiver);
		collisionEvent.gameObject.SendMessage("RepelAttacker", gameObject, SendMessageOptions.DontRequireReceiver);
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void RepelDamagedEntity(GameObject target)
	{
		GameObject root = target;
		if(root == null)
		{
			if(DebugMode)
				Debug.LogWarning("No root object was found for " + target.name);
			
			return;
		}
		
		SidescrollingMovement move = root.GetComponent<SidescrollingMovement>();
		if(move == null)
		{
			if(DebugMode)
				Debug.LogWarning("No sidescrolling movement was found for " + root.name);
			
			return;
		}
		
		Vector3 repelForce = (root.transform.position - transform.position).normalized * DamageForce;
		repelForce.Scale(Constraints);
		
		if(DebugMode)
			Debug.Log("Repelling " + root.name + " at force: " + repelForce);
		
		move.AddForce(repelForce);
	}
	
	#endregion Methods
}
