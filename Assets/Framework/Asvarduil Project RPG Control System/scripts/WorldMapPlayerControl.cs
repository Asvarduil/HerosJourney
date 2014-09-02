using UnityEngine;
using System.Collections;

public class WorldMapPlayerControl : MonoBehaviour 
{
	#region Variables / Properties
	
	public float moveSpeed = 18.0f;
	public float turnSpeed = 25.0f;
	public float rotationMargin = 0.1f;
	public float heightWindow = 3.0f;
	public float closeEnough = 1.1f;
	
	public string idleAnimation;
	public string moveAnimation;
	
	public ParticleSystem touchFX;
	
	private bool _Idle = true;
	
	private Transform _Target;
	private Vector3 _Destination;
	private RaycastHit _Hit;
	
	private readonly Vector3 _Up = new Vector3(0, 90, 0);
	private readonly Vector3 _Down = new Vector3(0, 270, 0);
	private readonly Vector3 _Left = new Vector3(0, 180, 0);
	private readonly Vector3 _Right = new Vector3(0, 0, 0);
	
	private readonly Vector3 _UpLeft = new Vector3(0,135,0);
	private readonly Vector3 _UpRight = new Vector3(0, 45, 0);
	private readonly Vector3 _DownLeft = new Vector3(0, 225, 0);
	private readonly Vector3 _DownRight = new Vector3(0, 315, 0);
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		MouseControl();
		ArrowControl();
		PerformAnimations();
	}

	#endregion Engine Hooks
	
	#region Methods
	
	public void PerformAnimations()
	{
		if( _Idle )
		{
			// Continue being idle. :D
			//animation.CrossFade( idleAnimation, 0.2 );
		}
		else
		{
			// Perform the walking animation...
			//animation.CrossFade( moveAnimation, 0.2 );
		}
	}
	
	public void MouseControl()
	{	
		//if(Settings.useFourAxisControl)
		//	return;
		
		// To control our character, we need to detect
		// RMB clicks.
		if( Input.GetAxis( "Fire2" ) > 0 )
		{
			// Do a raycast.
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			if( Physics.Raycast( ray, out _Hit, 1000 ) )
			{
				// We hit the terrain...
				if( _Hit.collider.tag == "Terrain" )
				{    
					// If the point is too high, we don't want to move there.
					// This is set by the "too tall" value we want.
					if( Mathf.Abs(_Hit.point.y - transform.position.y) >= heightWindow )
					{
						// Fire the GFX for touching the terrain.
						Vector3 fxLocation = new Vector3(_Hit.point.x, _Hit.point.y + 1.5f, _Hit.point.z);
						touchFX.transform.position = fxLocation;
						touchFX.Emit(1);
						
						_Target = null;
						_Destination = _Hit.point;
						_Idle = false;
					}
				}
					
				// We hit an enemy...
				if( _Hit.collider.tag == "Enemy" 
				    || _Hit.collider.tag == "NPC" )
				{
					// If we're close enough, attack.  Otherwise,
					// move toward them.
					_Target = _Hit.collider.transform;
					_Idle = false;
				}
			}
		}
		
		if(! _Idle)
		{
			// Pre-check - if we're moving toward an entity that
			// moves, get their position.  Otherwise, move toward
			// the destination.
			if( _Target )
			{
				_Destination = _Target.position;
			}
		
			// Pre-move check - if we've come to our destination,
			// we no longer need to move.
			if( Vector3.Distance( transform.position, _Destination ) <= closeEnough )
			{
				_Idle = true;
			}
			else
			{
				if(! string.IsNullOrEmpty(moveAnimation))
					animation.CrossFade(moveAnimation, 0.2f);
				
				MoveTowards( _Destination );
			}
		}
		else
		{
			if(! string.IsNullOrEmpty(idleAnimation))
				animation.CrossFade(idleAnimation, 0.2f);
		}
	}
	
	public void ArrowControl()
	{
		//if(! Settings.useFourAxisControl)
		//	return;
		
		bool doMove = false;
		
		#region Ordinal Directions
		
		if( Input.GetAxis( "Horizontal" ) > 0 )
		{
			transform.rotation = Quaternion.Euler(  _Up );
			doMove = true;
			_Idle = false;
		}
		
		if( Input.GetAxis( "Horizontal" ) < 0 )
		{
			transform.rotation = Quaternion.Euler( _Down );
			doMove = true;
			_Idle = false;
		}
		
		if( Input.GetAxis( "Vertical" ) > 0 )
		{
			transform.rotation = Quaternion.Euler( _Right );
			doMove = true;
			_Idle = false;
		}
		
		if( Input.GetAxis( "Vertical" ) < 0 )
		{
			transform.rotation = Quaternion.Euler( _Left );
			doMove = true;
			_Idle = false;
		}
		
		#endregion Ordinal Directions
		
		#region Cardinal Directions
		
		if(Input.GetAxis ("Horizontal") > 0 && Input.GetAxis ("Vertical") < 0)
		{
			transform.rotation = Quaternion.Euler ( _UpLeft );
			doMove = true;
			_Idle = false;
		}
		
		if(Input.GetAxis ("Horizontal") > 0 && Input.GetAxis ("Vertical") > 0)
		{
			transform.rotation = Quaternion.Euler ( _UpRight );
			doMove = true;
			_Idle = false;
		}
		
		if(Input.GetAxis ("Horizontal") < 0 && Input.GetAxis ("Vertical") < 0)
		{
			transform.rotation = Quaternion.Euler ( _DownLeft );
			doMove = true;
			_Idle = false;
		}
		
		if(Input.GetAxis ("Horizontal") < 0 && Input.GetAxis ("Vertical") > 0)
		{
			transform.rotation = Quaternion.Euler ( _DownRight );
			doMove = true;
			_Idle = false;
		}
		
		#endregion Cardinal Directions
		
		if(_Idle)
		{
			if(! string.IsNullOrEmpty(idleAnimation))
				animation.CrossFade(idleAnimation, 0.2f);
		}
		
		if(doMove)
		{
			// Cancel the mouse movement...
			_Target = null;
			_Destination = transform.position;
			
			_Idle = true;
			
			if(! string.IsNullOrEmpty(moveAnimation))
				animation.CrossFade (moveAnimation, 0.2f);
			
			transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		}
	}
	
	public void RotateTowards(Vector3 position)
	{
		Vector3 direction = position - transform.position;
		if( direction.magnitude < rotationMargin )
			return;
		
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
	}
	
	public void MoveTowards(Vector3 position)
	{
		RotateTowards( position );
		transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime );
	}
	
	#endregion Methods
}
