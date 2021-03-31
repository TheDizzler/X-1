namespace AtomosZ.AndroSyn.Actors.State
{
	public enum MovementStateType
	{
		NONE,
		STANDING, KNEELING,
		WALKING, STAIRS,
		ELEVATOR,
		FALLING, JETPACK,
	}

	public interface IMovementState
	{
		MovementStateType movementStateType { get; set; }
		void SetActor(Actor owner);
		void EnterState(MovementStateType previousState);
		/// <summary>
		/// Called in FixedUpdate().
		/// </summary>
		/// <returns>MovementStateType to transition to or None if no change.</returns>
		MovementStateType FixedUpdateState();
		MovementStateType ExitState(MovementStateType nextState);
	}
}