namespace AtomosZ.AndroSyn.Actors.State
{
	public enum MovementStateType
	{
		NONE,
		GROUNDED, DUCK, AIRBORN
	}

	public interface IMovementState
	{
		void SetActor(Actor owner);
		void EnterState(MovementStateType previousState);
		MovementStateType FixedUpdateState();
		MovementStateType ExitState(MovementStateType nextState);
	}
}