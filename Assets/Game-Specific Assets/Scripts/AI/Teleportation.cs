using UnityEngine;
using System.Collections;

public class Teleportation : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;
	public Rect TeleportRectangle;
	public GameObject TeleportEffect;

	#endregion Variables / Properties

	#region Actions

	public Vector3 ConstraintPointToRectangle(Vector3 target)
	{
		if(target.x < TeleportRectangle.xMin)
			target.x = TeleportRectangle.xMin;
		else if(target.x > TeleportRectangle.xMax)
			target.x = TeleportRectangle.xMax;
		
		if(target.y < TeleportRectangle.yMin)
			target.y = TeleportRectangle.yMin;
		else if(target.y > TeleportRectangle.yMax)
			target.y = TeleportRectangle.yMax;

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
