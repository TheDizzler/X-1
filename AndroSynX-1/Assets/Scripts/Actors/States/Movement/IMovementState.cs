﻿namespace AtomosZ.AndroSyn.Actors.State
{
	public enum MovementStateType
	{
		NONE,
		GROUNDED, DUCK,
		AIRBORN, JETPACK
	}

	public interface IMovementState
	{
		MovementStateType movementStateType { get; set; }
		void SetActor(Actor owner);
		void EnterState(MovementStateType previousState);
		/// <summary>
		/// The next state to transition to.
		/// </summary>
		/// <returns>Current MovementStateType</returns>
		MovementStateType FixedUpdateState();
		MovementStateType ExitState(MovementStateType nextState);
	}
}