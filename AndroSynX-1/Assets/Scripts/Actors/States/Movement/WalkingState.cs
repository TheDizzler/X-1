using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class WalkingState : MonoBehaviour, IMovementState
	{
		private const float TimeToStopMomentum = .15f;

		public MovementStateType movementStateType
		{
			get => MovementStateType.WALKING;
			set => throw new System.NotImplementedException();
		}

		private Actor actor;
		private float timeSinceNoMovement;



		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(MovementStateType previousState)
		{
			actor.animator.SetBool(Actor.IsWalkingHash, true);
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			actor.animator.SetBool(Actor.IsWalkingHash, false);
			return movementStateType;
		}

		public MovementStateType FixedUpdateState()
		{
			if (!actor.actorPhysics.isGrounded)
				return MovementStateType.FALLING;

			if (actor.commandList[CommandType.Duck])
			{
				actor.commandList[CommandType.Duck] = false;
				return MovementStateType.KNEELING;
			}

			if (actor.commandList[CommandType.Jetpack])
			{
				return MovementStateType.JETPACK;
			}

			if (actor.commandList[CommandType.MoveLeft]
				|| actor.commandList[CommandType.MoveRight])
			{
				Vector2 inputVelocity = actor.inputVelocity;
				inputVelocity.x *= actor.groundMovementSpeed;
				actor.actorPhysics.desiredVelocity = inputVelocity;
				bool wasFacingRight = actor.actorPhysics.isFacingRight;
				bool isFacingRight = inputVelocity.x > 0;
				if (wasFacingRight != isFacingRight)
					actor.Flip();
				timeSinceNoMovement = 0;
				actor.animator.SetFloat(Actor.WalkSpeedHash, inputVelocity.x);
			}
			else
			{ // this prevents the standing animation from starting when switching directions
				timeSinceNoMovement += Time.deltaTime;
				if (timeSinceNoMovement > TimeToStopMomentum)
					return MovementStateType.STANDING;
			}

			return MovementStateType.NONE;
		}
	}
}