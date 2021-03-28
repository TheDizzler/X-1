using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class IKWalkingState : MonoBehaviour, IMovementState
	{
		private const float TimeToStopMomentum = .15f;

		public MovementStateType movementStateType
		{
			get => MovementStateType.WALKING;
			set => throw new System.NotImplementedException();
		}

		public bool canClimbStairs = true;

		[SerializeField] private Transform body = null;
		[SerializeField] private IKLegSettings ikLegSettings = null;

		private Actor actor;
		private IActorPhysics actorPhysics;
		private float timeSinceNoMovement;


		private int lastFootToMove;
		private Vector3 stepBodyTarget;
		private Vector3 originalStepPosition;
		private float stepLerp;



		public void SetActor(Actor owner)
		{
			actor = owner;
			actorPhysics = actor.actorPhysics;
			if (canClimbStairs && ikLegSettings == null)
				ikLegSettings = actor.GetComponent<IKLegSettings>();
		}

		public void EnterState(MovementStateType previousState)
		{
			actor.SetAnimator(Actor.IsWalkingHash, true);
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			actor.SetAnimator(Actor.IsWalkingHash, false);
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

				if (canClimbStairs && CheckForObstacle(inputVelocity))
				{
					return MovementStateType.STAIRS;
				}

				actor.actorPhysics.desiredVelocity = inputVelocity;
				bool wasFacingRight = actor.actorPhysics.isFacingRight;
				bool isFacingRight = inputVelocity.x > 0;
				if (wasFacingRight != isFacingRight)
					actor.Flip();
				timeSinceNoMovement = 0;
				actor.SetAnimator(Actor.WalkSpeedHash, Mathf.Abs(inputVelocity.x));
			}
			else
			{ // this prevents the standing animation from starting when switching directions
				timeSinceNoMovement += Time.deltaTime;
				if (timeSinceNoMovement > TimeToStopMomentum)
					return MovementStateType.STANDING;
			}

			return MovementStateType.NONE;
		}

		private Vector3 ForwardCast() => -new Vector3(actorPhysics.up.x, actorPhysics.up.y)
			+ transform.right * ikLegSettings.stepLength * (actorPhysics.isFacingRight ? 1 : -1);

		private bool CheckForObstacle(Vector2 inputVelocity)
		{
			inputVelocity.y = 0;
			if (actorPhysics.CheckForCollision(inputVelocity))
			{
				inputVelocity.x = 0;
				inputVelocity.y = 0;

				// check to see if can step on
				RaycastHit2D hit = Physics2D.Raycast(body.position + new Vector3(ikLegSettings.stepLength, 0, 0),
					-actorPhysics.up, ikLegSettings.raycastDistance, ikLegSettings.probeMask);
				if (hit.collider == null)
				{
					Debug.LogError("huh?");
				}

				var dot = Vector2.Dot(hit.normal, actorPhysics.up);
				if (dot <= IKActorPhysics.Cos45 || hit.point.y > ikLegSettings.MaxStepHeight())
				{
					Debug.Log("too high/steep");
				}
				else
				{
					return true;
				}
			}

			return false;
		}
	}
}