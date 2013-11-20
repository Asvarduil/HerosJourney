using UnityEngine;
using System.Collections;

public class PlayerDeathReloader : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public GameObject Player;
	public int LastHP;

	private Fader _fader;
	private HealthSystem _health;
	private TransitionManager _sceneChange;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_sceneChange = TransitionManager.Instance;
		_fader = (Fader) FindObjectOfType(typeof(Fader));

		AcquirePlayer();
	}
	
	public void Update()
	{
		if(_health.HP == LastHP)
			return;

		if(DebugMode)
			Debug.Log("The player's HP has changed from " + LastHP + " to " + _health.HP);

		LastHP = _health.HP;
		if(_health.HP > 0)
			return;

		if(DebugMode)
			Debug.Log("The player has died!");

		StartCoroutine(ReloadLevelSequence());
	}

	public void OnLevelWasLoaded()
	{
		AcquirePlayer();
	}

	#endregion Engine Hooks
	
	#region Methods

	private void AcquirePlayer()
	{
		Player = GameObject.FindWithTag("Player");
		_health = Player.GetComponent<HealthSystem>();
		LastHP = _health.HP;
	}
	
	private IEnumerator ReloadLevelSequence()
	{
		if(DebugMode)
			Debug.Log("Performing level reload...");
		
		_fader.FadeOut();
		while(_fader.ScreenShown)
		{
			if(DebugMode)
				Debug.Log("Fading the screen out...");

			yield return 0;
		}
		
		_sceneChange.ChangeScenes();
	}
	
	#endregion Methods
}
