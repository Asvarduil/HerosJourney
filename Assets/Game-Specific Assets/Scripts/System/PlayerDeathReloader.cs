using UnityEngine;
using System;
using System.Collections;

public class PlayerDeathReloader : DebuggableBehavior 
{
	#region Variables / Properties

	public string observedTag = "Player";

	private Fader _fader;
	private HealthSystem _health;
	private TransitionManager _sceneChange;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_sceneChange = TransitionManager.Instance;
		_fader = (Fader) FindObjectOfType(typeof(Fader));
	}

	public void OnLevelWasLoaded()
	{
		DebugMessage("Level was successfully loaded.");
		_fader = (Fader) FindObjectOfType(typeof(Fader));
	}

	#endregion Engine Hooks
	
	#region Methods

	public void OnHealthChanged(HealthEventArgs args)
	{
		if(args.Tag != observedTag)
			return;

		DebugMessage("HP has changed!");

		if(args.HP > 0)
			return;

		StartCoroutine(ReloadLevelSequence());
	}

	private IEnumerator ReloadLevelSequence()
	{
		DebugMessage("Reloading level...");
		
		_fader.FadeOut();
		while(_fader.ScreenShown)
		{
			DebugMessage("Fading screen out...");
			yield return 0;
		}
		
		_sceneChange.ChangeScenes();
		yield return null;
	}

	#endregion Methods
}
