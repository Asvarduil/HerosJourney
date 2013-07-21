using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LogoPresenter : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public string NextSceneName;
	public float LogoPersistTime = 2.0f;
	public List<AsvarduilImage> Logos;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
	}
	
	public void OnGUI()
	{
		foreach(AsvarduilImage current in Logos)
		{
			current.DrawMe();
		}
	}
	
	public void Update()
	{
		foreach(AsvarduilImage current in Logos)
		{
			current.Tween();
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	#endregion Methods
}
