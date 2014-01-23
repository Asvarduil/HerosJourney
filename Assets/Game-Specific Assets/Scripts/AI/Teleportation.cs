using UnityEngine;
using System.Collections;

public class Teleportation : MonoBehaviour 
{
	#region Variables / Properties

	public GameObject TeleportEffect;

	#endregion Variables / Properties

	#region Actions

	public static Vector3 ConstraintPointToRectangle(Vector3 target, Rect rectangle)
	{
		if(target.x < rectangle.xMin)
			target.x = rectangle.xMin;
		else if(target.x > rectangle.xMax)
			target.x = rectangle.xMax;
		
		if(target.y < rectangle.yMin)
			target.y = rectangle.yMin;
		else if(target.y > rectangle.yMax)
			target.y = rectangle.yMax;

		return target;
	}

	public void TeleportToPoint(Vector3 position)
	{
		GameObject.Instantiate(TeleportEffect, transform.position, Quaternion.identity);
		GameObject.Instantiate(TeleportEffect, position, Quaternion.identity);
		
		transform.position = position;
	}

	#endregion Actions
}
