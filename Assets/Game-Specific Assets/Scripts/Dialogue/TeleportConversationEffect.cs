using UnityEngine;
using System.Collections;

public class TeleportConversationEffect : MonoBehaviour 
{
	#region Variables / Properties
	
	public GameObject Character;
	public GameObject TeleportEffect;
	public Vector3 TeleportTarget;
	
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
		if(TeleportEffect != null)
			Instantiate(TeleportEffect, character.transform.position, character.transform.rotation);

		character.transform.position = TeleportTarget;
	}
	
	#endregion Methods
}
