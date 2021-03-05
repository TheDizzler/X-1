﻿using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	/// <summary>
	/// Sub-state of Grounded.
	/// </summary>
	public class DuckingState : MonoBehaviour, IMovementState
	{
		private Actor actor;

		public MovementStateType movementStateType
		{
			get => MovementStateType.KNEELING;
			set => throw new System.NotImplementedException();
		}


		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(MovementStateType previousState)
		{
			actor.animator.SetBool(Actor.IsDuckingHash, true);
			Debug.Log("Entering DuckingState");
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			actor.animator.SetBool(Actor.IsDuckingHash, false);
			Debug.Log("Exiting DuckingState");
			return movementStateType;
		}

		public MovementStateType FixedUpdateState()
		{
			if (!actor.commandList[CommandType.Duck])
				return MovementStateType.STANDING;

			return MovementStateType.NONE;
		}
	}
}