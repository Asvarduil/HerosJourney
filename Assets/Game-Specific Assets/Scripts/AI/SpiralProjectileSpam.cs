using UnityEngine;
using System.Collections;

public class SpiralProjectileSpam : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;
	public float SpamDuration = 3.0f;
	public float SpamRate = 0.25f;
	public Vector3 SpamRotateAngle;
	public GameObject Projectile;

	private float _nextShotTime;
	private float _spamHaltTime;
	private bool _spamProjectiles;
	private Vector3 _spamEulerAngles;

	public bool IsDoneSpamming
	{
		get { return Time.time >= _spamHaltTime; }
	}

	#endregion Variables / Properties

	#region Engine Hooks

	#endregion Engine Hooks

	#region Methods

	public void ResetSpam()
	{
		_nextShotTime = Time.time;
		_spamHaltTime = Time.time + SpamDuration;
		_spamEulerAngles = Vector3.zero;
	}

	public void FireSomeSpam()
	{
		float currentTime = Time.time;
		if(currentTime < _nextShotTime)
			return;

		GameObject.Instantiate(Projectile, transform.position, Quaternion.Euler(_spamEulerAngles));
		_spamEulerAngles += SpamRotateAngle;

		_nextShotTime = currentTime + SpamRate;

		if(DebugMode)
		{
			Debug.Log("Next shot will be at " + _nextShotTime + " at angle: " + _spamEulerAngles);
			Debug.Log("Time to cease the spam is at: " + _spamHaltTime);
		}
	}

	#endregion Methods
}
