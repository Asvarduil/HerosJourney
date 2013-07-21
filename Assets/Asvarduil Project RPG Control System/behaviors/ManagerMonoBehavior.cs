using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public abstract class ManagerMonoBehavior : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = true;
	public bool IsInitialInstance = false;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public abstract void SelfDestructIfOthersExist();
	
	#endregion Methods
}
