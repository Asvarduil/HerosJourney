using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {

	#region Variables / Properties
	
	public bool DebugMode = false;
	public string MessageOnHit;
	public GameObject SpawnOnHit;
	
	public List<string> AffectedTags = new List<string>
	{
		"Player"
	};
	
	public Vector3 Velocity;
	public float LifeTime = 10.0f;
	
	private float _expireTime;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_expireTime = Time.time + LifeTime;
	}
	
	public void FixedUpdate()
	{
		CheckExpiration();
		transform.Translate(Velocity * Time.deltaTime);
	}
	
	public void OnTriggerEnter(Collider collider)
	{	
		if(DebugMode)
			Debug.Log("Struck a trigger!");
		
		ProjectileHit(collider);
	}
	
	public void OnCollisionEnter(Collision collision)
	{
		if(DebugMode)
			Debug.Log("Struck a collider!");
		
		Collider who = collision.collider;
		ProjectileHit(who);
	}
	
	#endregion Engine Hooks
	
	#region Methods

	public void RepelFrom(Vector3 point, float speed)
	{
		Vector3 newVelocity = point - transform.position;
		
		if(DebugMode)
			Debug.Log ("Velocity on repel: " + newVelocity);
		
		Velocity = newVelocity * speed;
	}
	
	private void ProjectileHit(Collider collider)
	{
		if(DebugMode)
		{
			string debugMessage = String.Format("Affected Entity: {0} [{1}]", collider.name, collider.tag);
			Debug.Log(debugMessage);
		}
			
		if(! AffectedTags.Contains(collider.tag))
			return;
		
		SendCustomMessageToHitEntity(collider.gameObject);
		SpawnSecondaryObject();
		
		Destroy(gameObject);
	}
	
	private void CheckExpiration()
	{
		if(LifeTime == 0)
			return;
		
		if(Time.time < _expireTime)
			return;

		if(DebugMode)
			Debug.Log("Projectile has expired.  Self-destructing.");
		
		Destroy(gameObject);
	}
	
	private void SendCustomMessageToHitEntity(GameObject hitObject)
	{
		if(string.IsNullOrEmpty(MessageOnHit))
			return;
	
		if(DebugMode)
			Debug.Log("Sending message [ " + MessageOnHit + " ] to " + hitObject.name);
		
		hitObject.SendMessage(MessageOnHit, SendMessageOptions.DontRequireReceiver);
	}
	
	private void SpawnSecondaryObject()
	{
		if(SpawnOnHit == null)
			return;
		
		if(DebugMode)
			Debug.Log("Spawning secondary object " + SpawnOnHit.name);
		
		GameObject.Instantiate(SpawnOnHit, transform.position, transform.rotation);
	}
	
	#endregion Methods
}
