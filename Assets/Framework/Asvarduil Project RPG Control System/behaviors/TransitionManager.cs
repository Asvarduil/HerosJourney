using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class TransitionManager : ManagerMonoBehavior
{
	#region Variables / Properties

	public string DebugCreateTime;
	
	public Vector3 spawnPosition;
	public Vector3 spawnRotation;
	public int targetSceneID = -1;
	public string targetSceneName = string.Empty;
	public GameObject playerPiece;
	
	public DateTime CreateTime { get; private set; }
	
    public static TransitionManager Instance
	{
		get { return TransitionManager.FindOldestInstance(); }
	}
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	#endregion Engine Hooks
	
	#region Base Class Overrides
	
	public override void SelfDestructIfOthersExist()
	{
		TransitionManager[] objects = (TransitionManager[]) FindObjectsOfType(typeof(TransitionManager));
		IEnumerable<TransitionManager> destructables = objects.Where(o => o.IsInitialInstance == false);
		
		if(DebugMode)
			Debug.LogWarning(destructables.Count() + " other managers were found!  Destroying them.");
		
		foreach(TransitionManager current in destructables)
		{
			Destroy(current.gameObject);
		}
	}
	
	#endregion Base Class Overrides
	
	#region Methods
	
	public static TransitionManager FindOldestInstance()
	{	
		TransitionManager[] objects = (TransitionManager[]) FindObjectsOfType(typeof(TransitionManager));
		if(objects.Length == 0)
			throw new Exception("TransitionManager could not find any Transition Managers in the scene!");
		
		TransitionManager oldest = objects.FirstOrDefault(a => a.IsInitialInstance == true);
		if(oldest == default(TransitionManager))
		{
			oldest = objects.First();
			oldest.IsInitialInstance = true;
		}
		
		return oldest;
	}
	
	public void ChangeScenes()
	{	
		if(targetSceneID == -1)
		{
			Application.LoadLevel(targetSceneName);
		}
		else
		{
			Application.LoadLevel(targetSceneID);
		}
	}
	
	// Information:
	// -------------------------------------------------------------------------------
	// The following methods are responsible for the player piece being copied to the 
	// destination scene when it has loaded, via the SceneSpawner script (this script
	// is pre-set on the GameCamera prefab!)
	//
	// In the prior scene, the player's piece is acquired by the manager, which means 
	// a clone is saved to the class.
	//
	// In the target scene, when it is loaded, the SceneSpawner will Instantiate the 
	// piece, then lock the camera to it.
	
	public void AcquirePlayerPiece(GameObject piece)
	{
		if(piece == null)
			throw new ArgumentNullException("Must specify a game object to copy and transition to the new scene.");
		
		playerPiece = (GameObject) GameObject.Instantiate(piece, piece.transform.position, piece.transform.rotation);
		playerPiece.name = piece.name;
		
		UnityEngine.Object.Destroy(piece);
		UnityEngine.Object.DontDestroyOnLoad(playerPiece);
	}
	
	// Information:
	// -------------------------------------------------------------------------------
	// The following methods are used when the transition is ready, for instance, when
	// the screen is fully faded and the player cannot see it.
	//
	// You give these methods the position, rotation, and scene info for the scene you
	// want to perform the transition to.  ChangeScene() will automatically call that
	// information and perform the scene swap.
	
	public void PrepareTransition(Vector3 targetPos, Vector3 targetRot, int sceneID)
	{
		if(DebugMode)
			Debug.Log("Prepping a transition to: " + Environment.NewLine
				      + "Location: " + targetPos + Environment.NewLine
				      + "Rotation: " + targetRot + Environment.NewLine
				      + "Scene ID: " + sceneID);
		
		SetLocationRotation(targetPos, targetRot);
		targetSceneID = sceneID;
		targetSceneName = string.Empty;
	}
	
	public void PrepareTransition(Vector3 targetPos, Vector3 targetRot, string sceneName)
	{
		if(DebugMode)
			Debug.Log("Prepping a transition to: " + Environment.NewLine
				      + "Location: " + targetPos + Environment.NewLine
				      + "Rotation: " + targetRot + Environment.NewLine
				      + "Scene: " + sceneName);
		
		SetLocationRotation(targetPos, targetRot);
		targetSceneName = sceneName;
		targetSceneID = -1;
	}
	
	private void SetLocationRotation(Vector3 targetPos, Vector3 targetRot)
	{
		spawnPosition = targetPos;
		spawnRotation = targetRot;
	}
	
	#endregion Methods
}
