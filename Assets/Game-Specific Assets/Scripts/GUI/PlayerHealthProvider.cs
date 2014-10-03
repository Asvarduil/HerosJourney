using UnityEngine;
using System;
using System.Collections;

[ExecuteInEditMode()]
public class PlayerHealthProvider : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public string observedTag = "Player";
	public Texture2D FullHeart;
	public Texture2D HalfHeart;
	public Texture2D NoHeart;
	
	public AsvarduilImage UiWidget;
	public Texture2D _fullUiWidget;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		HealthSystem playerHealthSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthSystem>();
		CalculateWidget(playerHealthSystem.HP, playerHealthSystem.MaxHP);
	}
	
	public void OnGUI()
	{
		UiWidget.DrawMe();
	}
	
	public void FixedUpdate()
	{
		UiWidget.Tween();
	}
	
	#endregion Engine Hooks
	
	#region Methods

	public void OnHealthChanged(HealthEventArgs args)
	{
		if(args.Tag != observedTag)
			return;

		CalculateWidget(args.HP, args.MaxHP);
	}
	
	public void CalculateWidget(int hp, int maxhp)
	{
		// Calcluate width and values for the heart system...
		int canvasWidth = FullHeart.width * (maxhp / 2);
		int fullHearts = hp / 2;
		int halfHearts = hp % 2;
		int deadHearts = (maxhp / 2) - (int) Mathf.Ceil(((float) hp / 2));
		
		if(DebugMode)
		{
			string texRectMessage = String.Format("Canvas width: [{0}] Canvas height: [{1}]", canvasWidth, FullHeart.height);
			Debug.Log(texRectMessage);
		}
		
		// Build the dynamic texture...
		int heartX = 0;
		Texture2D tex = new Texture2D(canvasWidth, FullHeart.height);
		
		DrawFullHearts(fullHearts, tex, ref heartX);
		DrawHalfHearts(halfHearts, tex, ref heartX);
		DrawEmptyHearts(deadHearts, tex, ref heartX);
		
		// Apply the texture, update the UI widget.
		tex.Apply();
		_fullUiWidget = tex;
		UiWidget.Image = _fullUiWidget;
	}

	private void DrawFullHearts (int fullHearts, Texture2D tex, ref int heartX)
	{
		for(int counter = 0; counter < fullHearts; counter++)
		{
			tex.SetPixels(heartX, 0, FullHeart.width, FullHeart.height, FullHeart.GetPixels());
			heartX += FullHeart.width;
		}
	}

	private void DrawHalfHearts (int halfHearts, Texture2D tex, ref int heartX)
	{
		for(int counter = 0; counter < halfHearts; counter++)
		{
			tex.SetPixels(heartX, 0, HalfHeart.width, HalfHeart.height, HalfHeart.GetPixels());
			heartX += HalfHeart.width;
		}
	}
	

	private void DrawEmptyHearts (int deadHearts, Texture2D tex, ref int heartX)
	{
		for(int counter = 0; counter < deadHearts; counter++)
		{
			tex.SetPixels(heartX, 0, NoHeart.width, NoHeart.height, NoHeart.GetPixels());
			heartX += NoHeart.width;
		}
	}
	
	#endregion Methods
}
