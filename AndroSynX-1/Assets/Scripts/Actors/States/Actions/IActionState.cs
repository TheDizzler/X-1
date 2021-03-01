namespace AtomosZ.AndroSyn.Actors
{
	public enum ActionStateType
	{
		None,
		Shoot,
	}


	public interface IActionState
	{
		ActionStateType actionStateType { get; set; }
		void SetActor(Actor owner);
		void EnterState(ActionStateType previousState);
		/// <summary>
		/// The next state to transition to.
		/// </summary>
		/// <returns>Current MovementStateType</returns>
		ActionStateType FixedUpdateState();
		ActionStateType ExitState(ActionStateType nextState);
	}
}