using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public enum AnimationMode
{
	Loop,
	OneShot
}

public abstract class AnimationSystemBase<T> : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public float AnimationRate = 0.1f;
	
	public AnimationMode AnimationMode;
	public List<AnimationBase<T>> Animations;
	public AnimationBase<T> CurrentAnimation;
	public AnimationBase<T> PreviousAnimation;
	
	protected bool _autoPlay = true;
	protected bool _isLocked = false;
	protected float _nextFrame;

	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public virtual void Start() 
	{
		if(Animations == null
		   || Animations.Count == 0
		   || Animations[0] == null)
			return;
		
		CurrentAnimation = Animations[0];
	}
	
	public virtual void Update () 
	{
		if(! _autoPlay)
			return;
		
		AdvanceCurrentAnimationOne();
	}
	
	#endregion Engine Hooks
	
	#region Methods

	public bool AnimationIsComplete()
	{
		if(DebugMode)
			Debug.Log("(" + gameObject.name + ") Animation " + CurrentAnimation.Name + " is " 
			          + (CurrentAnimation.IsAnimationCycleDone ? "" : "not") + " done.");

		return CurrentAnimation.IsAnimationCycleDone;
	}
	
	protected abstract void AdvanceAnimation();
	
	protected virtual void EvaluateAnimationMode()
	{	
		switch(AnimationMode)
		{
			case AnimationMode.OneShot:
				if(CurrentAnimation.Current == (CurrentAnimation.Frames.Count - 1))
				{
					SetAnimation(PreviousAnimation.Name);
					AnimationMode = AnimationMode.Loop;
				}
				break;
			
			// Default assumes 'loop', as that is anticipated to be the most heavily used mode.
			default:
				break;
		}
	}
	
	protected virtual void AdvanceCurrentAnimationOne()
	{
		if(Time.time < _nextFrame)
			return;
		
		if(CurrentAnimation == null)
			return;
		
		AdvanceAnimation();
		_nextFrame = Time.time + AnimationRate;
		
		EvaluateAnimationMode();
		
		// if the animation is complete, release the animation lock.
		if(CurrentAnimation.IsAnimationCycleDone)
			_isLocked = false;
	}
	
	public virtual void SetAnimation(string animation)
	{
		if(string.IsNullOrEmpty(animation))
			throw new ArgumentNullException("animation");
		
		AnimationBase<T> newAnimation = Animations.FirstOrDefault(a => a.Name == animation);
		if(newAnimation == default(AnimationBase<T>))
			return;
		
		CurrentAnimation = newAnimation;
		CurrentAnimation.Current = 0;
	}
	
	public virtual void Play(string animation, AnimationMode mode = AnimationMode.Loop)
	{
		_autoPlay = true;
		
		if(CurrentAnimation == null
		   || CurrentAnimation.Frames.Count == 0)
		{
			if(DebugMode)
				Debug.LogWarning("There is no current animation to play, test, advance against!");
			
			return;
		}
		
		if(string.IsNullOrEmpty(animation))
			animation = CurrentAnimation.Name;
		
		if(animation == CurrentAnimation.Name)
			return;
		
		AnimationMode = mode;
		PreviousAnimation = CurrentAnimation;
		
		AdvanceCurrentAnimationOne();
	}
	
	public virtual void PlaySingleFrame(string animation, bool lockAnimation = false, AnimationMode mode = AnimationMode.Loop)
	{
		_autoPlay = false;
		
		if(CurrentAnimation == null
		   || CurrentAnimation.Frames.Count == 0)
		{
			if(DebugMode)
				Debug.LogWarning("There is no current animation to play, test, advance against!");
			
			return;
		}
		
		if(string.IsNullOrEmpty(animation))
			animation = CurrentAnimation.Name;
		
		// If the new animation does not match the current animation,
		// and there is no lock asserted, change animations.
		if(animation != CurrentAnimation.Name 
		   && ! _isLocked)
		{
			if(DebugMode)
				Debug.Log("No lock; setting lock and animation state.");
			
			_isLocked = lockAnimation;
			AnimationMode = mode;
			PreviousAnimation = CurrentAnimation;
			SetAnimation(animation);
		}
		
		if(DebugMode)
			Debug.Log(String.Format("Advancing animation {0} to frame {1}...", CurrentAnimation.Name, CurrentAnimation.Current));
		
		AdvanceCurrentAnimationOne();
	}
	
	#endregion Methods
}

[Serializable]
public class AnimationBase<T>
{
	#region Variables / Properties
	
	public string Name;
	public int Current;
	public List<T> Frames;
	
	public bool IsAnimationCycleDone
	{
		get { return Current >= Frames.Count - 1; }
	}
	
	public T CurrentFrame
	{
		get 
		{ 
			if(Current > Frames.Count)
				Current = 0;
			
			if(Frames == null
			   || Frames.Count == 0
			   || Frames[Current] == null)
				throw new NullReferenceException("Animations need at least one frame to animate.");
			
			return Frames[Current]; 
		}
	}
	
	#endregion Variables / Properties
	
	#region Methods
	
	public virtual void UseCurrentFrame()
	{
	}
	
	public void SwitchToNextFrame()
	{
		Current = (Current + 1) % Frames.Count;
	}
	
	#endregion Methods
}
