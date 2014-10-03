using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class WebbingEffect : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = true;
	public float LifeTime = 2.0f;
	public List<string> AffectedTags = new List<string>{ "Enemy", "Player" };
	
	private GameObject _affectedObject;
	private SidescrollingMovement _moveSystem;
	private float _expirationTime;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_expirationTime = Time.time + LifeTime;
	}
	
	public void Update()
	{
		if(Time.time < _expirationTime)
			return;
		
		if(DebugMode)
			Debug.Log("This webbing has faded.  Restoring control to the entity.");
		
		RestoreMovement();
		Destroy(gameObject);
	}
	
	public void OnTriggerStay(Collider who)
	{
		if(! AffectedTags.Contains(who.tag)
		   || _affectedObject != null)
			return;
		
		_affectedObject = who.gameObject;
		_moveSystem = _affectedObject.GetComponent<SidescrollingMovement>();
		if(_moveSystem != null)
		{
			if(DebugMode)
				Debug.Log("Webbing has snared " + _affectedObject.name);
			
			_moveSystem.AllowHorizontalMovement = false;
		}
		else
		{
			if(DebugMode)
				Debug.Log(_affectedObject.name + " has no Sidescrolling Movement to restrain...");
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void RestoreMovement()
	{
		if(_moveSystem != null)
			_moveSystem.AllowHorizontalMovement = true;
	}
	
	#endregion Methods
}
