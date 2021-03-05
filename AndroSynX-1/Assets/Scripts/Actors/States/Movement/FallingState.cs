using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	/// <summary>
	/// A free-falling state with no means of vertical propulsion.
	/// </summary>
	public class FallingState : MonoBehaviour, IMovementState
	{
		private const float TimeToFallAnimation = 2.5f;

		private Actor actor;
		private float timeFalling;
		


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
			timeFalling = 0;
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			actor.animator.SetBool(Actor.IsFallingHash, false);
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

			timeFalling += Time.deltaTime;
			if (timeFalling > TimeToFallAnimation)
			{
				actor.animator.SetBool(Actor.IsFallingHash, true);
			}

			return MovementStateType.NONE;
		}
	}
}