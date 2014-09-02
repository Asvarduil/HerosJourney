using UnityEngine;
using System.Collections;

public class Settings
{
	#region Variables
	
	public static float graphicsLevel = 3;
	
	public static bool soundEnabled = true;
	public static float masterVolume = 0.5f;
	public static float sfxVolume = 0.1f;
	public static float musVolume = 0.1f;
	
	public static bool useMouseControl = true;
	public static bool useFourAxisControl = true;
	
	#endregion Variables
	
	#region Singleton Implementation
	
	// This architecture sets up the singleton the first
	// time a member is referenced.
	public static readonly Settings instance = new Settings();
	public static Settings Instance
	{
		get { return instance; }
	}
	
	private Settings() {  }
	
	#endregion Singleton Implementation
	
	#region Methods
	#endregion Methods
}
