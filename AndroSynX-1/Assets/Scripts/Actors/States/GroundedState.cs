using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class GroundedState : MonoBehaviour, IMovementState
	{
		private Actor actor;

		public MovementStateType movementStateType
		{
			get => MovementStateType.GROUNDED;
			set => throw new System.NotImplementedException();
		}

		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(MovementStateType previousState)
		{
			//Debug.Log("Entering StandingState");
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			//Debug.Log("Exiting StandingState");
			return movementStateType;
		}

		public MovementStateType FixedUpdateState()
		{
			if (!actor.actorPhysics.isGrounded)
				return MovementStateType.AIRBORN;

			if (actor.commandList[CommandType.Duck])
			{
				actor.commandList[CommandType.Duck] = false;
				return MovementStateType.DUCK;
			}

			if (actor.commandList[CommandType.MoveLeft]
				|| actor.commandList[CommandType.MoveRight])
			{
				Vector2 inputVelocity = actor.inputVelocity;
				inputVelocity.x *= actor.groundMovementSpeed;
				actor.actorPhysics.desiredVelocity = inputVelocity;
			}

			if (actor.commandList[CommandType.Jetpack])
			{
				return MovementStateType.JETPACK;
			}

			return MovementStateType.NONE;
		}
	}
}