namespace AtomosZ.AndroSyn.Actors
{
	public enum ActionStateType
	{
		NONE,
		AWAITING_ACTION,
		SHOOT,
		ELEVATOR,
	}


	public interface IActionState
	{
		ActionStateType actionStateType { get; set; }
		void SetActor(Actor owner);
		void EnterState(ActionStateType previousState);
		/// <summary>
		/// Called in Update().
		/// </summary>
		/// <returns>MovementStateType to transition to or None if no change.</returns>
		ActionStateType UpdateState();
		ActionStateType ExitState(ActionStateType nextState);
	}
}