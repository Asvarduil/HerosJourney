using UnityEngine;
using System.Collections;

public class ConversationEvents : MonoBehaviour 
{
	#region Variables / Properties
	
	public GameObject Garlan;
	public GameObject TeleportEffect;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	#endregion Engine Hooks
	
	#region Messages
		
	public void TeleportGarlan()
	{
		TeleportCharacter(Garlan);
	}
	
	public void TeleportBarbariccia()
	{
	}
	
	public void TeleportArtasandro()
	{
	}
	
	#endregion Messages
	
	#region Methods
	
	public void TeleportCharacter(GameObject character)
	{
		Instantiate(TeleportEffect, character.transform.position, character.transform.rotation);
		Destroy(character);
	}
	
	#endregion Methods
}
