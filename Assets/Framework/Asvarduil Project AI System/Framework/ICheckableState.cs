using System;

public interface ICheckableState
{
	#region Required Properties
	
	Func<bool> Condition { get; set; }
	Action Behavior { get; set; }
	
	#endregion Required Properties
	
	#region Required Methods
	
	Action CheckState();
	
	#endregion Required Methods
}