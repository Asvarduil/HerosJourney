using UnityEngine;
using System.Collections.Generic;

public class ActivateConversationObjects : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public List<GameObject> Objects;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void ActivateGameObjects()
	{
		Objects.ForEach(o => o.SetActive(true));
	}
	
	public void DeactivateGameObjects()
	{
		Objects.ForEach(o => o.SetActive(false));
	}
	
	#endregion Methods
}
