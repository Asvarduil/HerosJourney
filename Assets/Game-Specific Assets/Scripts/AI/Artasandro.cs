using UnityEngine;
using System.Collections;

public class Artasandro : AIBase
{
	#region Variables / Properties



	#endregion Variables / Properties

	#region Engine Hooks

	public void FixedUpdate()
	{
		if(_isPaused)
			return;


		PlayAnimations();
	}

	#endregion Engine Hooks

	#region Methods

	#endregion Methods
}
