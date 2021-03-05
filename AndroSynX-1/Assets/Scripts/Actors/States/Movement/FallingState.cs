using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	/// <summary>
	/// A free-falling state with no means of vertical propulsion.
	/// </summary>
	public class FallingState : MonoBehaviour, IMovementState
	{
		private Actor actor;
		public MovementStateType movementStateType
		{
			get => MovementStateType.FALLING;
			set => throw new System.NotImplementedException();
		}


		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(MovementStateType previousState)
		{
			//Debug.Log("Entering AirbornState");
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			//Debug.Log("Exiting AirbornState");
			return movementStateType;
		}


		public MovementStateType FixedUpdateState()
		{
			if (actor.actorPhysics.isGrounded)
			{ // if fall is "hard" go into a kneeling state
				return MovementStateType.STANDING;
			}

			if (actor.commandList[CommandType.MoveLeft]
				|| actor.commandList[CommandType.MoveRight])
			{
				Vector2 inputVelocity = Vector2.zero;
				inputVelocity.x = actor.inputVelocity.x * actor.airMovementSpeed;
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