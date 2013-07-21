using UnityEngine;
using System.Collections;

public class SceneChangeTrigger : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool debugMode = false;
	public string detectTag = "Player";
	public string sceneName;
	public Vector3 spawnLocation = new Vector3(0, 0, 0);
	public Vector3 spawnRotation = new Vector3(0, 0, 0);
	
	private Fader _fader;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_fader = (Fader) FindObjectOfType(typeof(Fader));
	}
	
	public IEnumerator OnTriggerEnter(Collider who)
	{
		if(who.tag != "Player")
			yield break;
		
		WorldMapControl controls = who.GetComponent<WorldMapControl>();
		if(controls != null)
			controls.enabled = false;
		
		PlayerControl sideControls = who.GetComponent<PlayerControl>();
		if(sideControls != null)
			sideControls.enabled = false;
		
		TransitionManager.Instance.PrepareTransition(spawnLocation, spawnRotation, sceneName);
		
		_fader.FadeOut();
		do
		{
			yield return 1;
		} while (! _fader.ScreenHidden);
		
		TransitionManager.Instance.ChangeScenes();
	}
	
	#endregion Engine Hooks
}

public class SceneChangeManager
{
	#region Variables / Properties
	
	public string sceneName;
	public Vector3 spawnLocation = new Vector3(0, 0, 0);
	public Vector3 spawnRotation = new Vector3(0, 0, 0);
	
	#endregion Variables / Properties
	
	#region Singleton Implementation
	
	private static SceneChangeManager _instance;
	public static SceneChangeManager Instance
	{
		get
		{
			return _instance
				   ?? (_instance = new SceneChangeManager());
		}
	}
	
	protected SceneChangeManager()
	{
	}
	
	#endregion Singleton Implementation
	
	#region Methods
	
	public void PrepareSpawn(string scene, Vector3 location, Vector3 rotation)
	{
		sceneName = scene;
		spawnLocation = location;
		spawnRotation = rotation;
	}
	
	public void ChangeScene()
	{		
		Application.LoadLevel(sceneName);
	}
	
	public IEnumerator SetupPlayer()
	{
		Fader sceneFader = (Fader) GameObject.FindObjectOfType(typeof(Fader));
		
		GameObject player = GameObject.FindWithTag("Player");
		player.transform.position = spawnLocation;
		player.transform.rotation = Quaternion.Euler(spawnRotation);
		
		sceneFader.FadeIn();
		yield return sceneFader.ScreenShown;
	}
	
	#endregion Methods
}