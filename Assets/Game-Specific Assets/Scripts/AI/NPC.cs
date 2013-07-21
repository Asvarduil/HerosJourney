using UnityEngine;
using System.Collections.Generic;

public class NPC : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool SensesPlayer;
	public List<string> AllowedTags;
	public string LeftAnimation;
	public string RightAnimation;
	public SpriteSystem Sprite;
	
	public GameObject SensedEntity;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void FixedUpdate()
	{
		UpdateAnimation();
	}
	
	public void OnTriggerEnter(Collider who)
	{
		if(! AllowedTags.Contains(who.tag))
			return;
		
		SensedEntity = who.gameObject;
		UpdateAnimation();
	}
	
	public void OnTriggerExit(Collider who)
	{
		if(! AllowedTags.Contains(who.tag))
			return;
		
		SensedEntity = null;
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void UpdateAnimation()
	{
		if(SensedEntity == null)
			return;
		
		string animation = (SensedEntity.transform.position.x < transform.position.x)
			? LeftAnimation
			: RightAnimation;
		
		Sprite.Play(animation);
	}
	
	#endregion Methods
}
