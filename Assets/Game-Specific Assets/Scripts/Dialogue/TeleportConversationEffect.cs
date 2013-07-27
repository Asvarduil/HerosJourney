using UnityEngine;
using System.Collections;

public class TeleportConversationEffect : MonoBehaviour 
{
	#region Variables / Properties
	
	public GameObject Character;
	public GameObject TeleportEffect;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	#endregion Engine Hooks
	
	#region Messages
		
	public void TeleportCharacterAway()
	{
		TeleportCharacter(Character);
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
