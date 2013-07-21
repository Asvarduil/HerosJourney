using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class SpriteSystem : AnimationSystemBase<Texture2D>
{
	#region Variables / Properties
	
	public List<SpriteAnimation> AllAnimations;
	private Renderer _renderer;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public override void Start()
	{		
		_renderer = GetComponent<Renderer>();
		Animations = AllAnimations.Select(x => x as AnimationBase<Texture2D>).ToList();
		base.Start();
		
		if(DebugMode)
			Debug.Log(String.Format("For Sprite System on {0}, there are {1} animations!", gameObject.transform.root.name, Animations.Count));
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	protected override void AdvanceAnimation()
	{
		_renderer.material.SetTexture("_MainTex", CurrentAnimation.CurrentFrame);
		CurrentAnimation.SwitchToNextFrame();
	}
	
	#endregion Methods
}

[Serializable]
public class SpriteAnimation : AnimationBase<Texture2D>
{
}
