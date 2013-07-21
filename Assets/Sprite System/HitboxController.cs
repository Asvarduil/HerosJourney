using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class HitboxController : AnimationSystemBase<CompoundHitboxPosition>
{
	#region Variables / Properties
	
	public List<HitboxAnimation> AllAnimations;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public override void Start()
	{
		Animations = AllAnimations.Select(x => x as AnimationBase<CompoundHitboxPosition>).ToList();
		base.Start();
		
		if(DebugMode)
			Debug.Log(String.Format("For Hitbox Controller on {0}, there are {1} animations!", gameObject.transform.root.name, Animations.Count));
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	protected override void AdvanceAnimation()
	{
		CurrentAnimation.UseCurrentFrame();
		CurrentAnimation.SwitchToNextFrame();
	}
	
	#endregion Methods
}

[Serializable]
public class HitboxAnimation : AnimationBase<CompoundHitboxPosition>
{
	#region Variables / Properties
	
	#endregion Variables / Properties
	
	#region Methods
	
	public override void UseCurrentFrame()
	{
		CurrentFrame.SetPositions();
	}
	
	#endregion Methods
}

[Serializable]
public class CompoundHitboxPosition
{
	#region Variables / Properties
	
	public string Name;
	public List<HitboxPosition> Positions;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void SetPositions()
	{
		foreach(var item in Positions)
		{
			item.SetPosition();
		}
	}
	
	#endregion Methods
}

[Serializable]
public class HitboxPosition
{
	#region Variables / Properties
	
	public GameObject Hitbox;
	public Vector3 LocalPosition;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void SetPosition()
	{
		Hitbox.transform.localPosition = LocalPosition;
	}
	
	#endregion Methods
}