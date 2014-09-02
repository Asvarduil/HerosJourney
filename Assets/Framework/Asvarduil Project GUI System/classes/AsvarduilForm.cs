using UnityEngine;
using System;

/// <summary>
/// Asvardul Project custom form.
/// </summary>
[Serializable]
public class AsvarduilForm : IDrawable, ITweenable
{
	#region Variables / Properties

	public bool DebugMode = false;
	public AsvarduilImage Background;
	public AsvarduilLabel WindowName;
	
	#endregion Variables / Properties
	
	#region Constructor
	
	public AsvarduilForm(AsvarduilImage bg, AsvarduilLabel winName)
	{
		Background = bg;
		WindowName = winName;
	}
	
	#endregion Constructor
	
	#region Methods
	
	public virtual void DrawMe()
	{
		Background.DrawMe();
		WindowName.DrawMe();
	}
	
	public virtual void Tween()
	{
		Background.Tween();
		WindowName.Tween();
	}
	
	#endregion Methods
}
