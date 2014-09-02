using System;
using System.Collections.Generic;

[Serializable]
public struct State : ICheckableState
{
	#region Variables / Properties
	
	public Func<bool> Condition { get; set; }
	public Action Behavior { get; set; }
	
	#endregion Variables / Properties
	
	#region Constructor
	
	public State(Func<bool> condition, Action behavior)
	{
		Condition = condition;
		Behavior = behavior;
	}
	
	#endregion Constructor
	
	#region Methods
	
	public Action CheckState()
	{
		return Condition()
			? Behavior
			: null;
	}
	
	#endregion Methods
}
