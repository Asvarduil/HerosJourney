using UnityEngine;
using System;

public class ProjectileProjector : MonoBehaviour 
{
	#region Variables / Properties
	
	public GameObject Projectile;
	public Vector3 ProjectileVelocity;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void Fire()
	{
		if(Projectile == null)
			throw new NullReferenceException("You need to assign a projectile for a Projectile Projector!");
		
		GameObject projectile = (GameObject) GameObject.Instantiate(Projectile, transform.position, Quaternion.Euler(Vector3.zero));
		Projectile control = projectile.GetComponent<Projectile>();
		control.Velocity = ProjectileVelocity;
	}
	
	#endregion Methods
}
