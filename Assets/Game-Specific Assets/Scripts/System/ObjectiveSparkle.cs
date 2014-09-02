using UnityEngine;
using System.Collections.Generic;

public class ObjectiveSparkle : MonoBehaviour 
{
	#region Variables / Properties

	public List<SequenceCounter> AvailableSequences;

	private Ambassador _ambassador;

	#endregion Variables / Properties

	#region Events

	public void Start()
	{
		_ambassador = Ambassador.Instance;

		EnableEffect();
	}

	#endregion Events

	#region Methods

	public void EnableEffect()
	{
		bool showParticles = false;
		foreach(SequenceCounter sequence in AvailableSequences)
		{
			showParticles = showParticles || _ambassador.CheckThreadProgress(sequence.Name, sequence.Phase);
		}
		
		particleEmitter.emit = showParticles;
	}

	#endregion Methods
}
