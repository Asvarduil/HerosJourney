public interface IPausableEntity
{
	/// <summary>
	/// This method receives pause commands radiated from a source.
	/// This method should do things like set the IsPaused flag, 
	/// cause temporary disability of AI scripts, or other pause actions.
	/// </summary>
	void PauseThisEntity();
	
	/// <summary>
	/// This method receives resume commands radiated from a source.
	/// This method should do things like clear the IsPaused flag,
	/// or remove disabling conditions from game scripts.
	/// </summary>
	void ResumeThisEntity();
}
