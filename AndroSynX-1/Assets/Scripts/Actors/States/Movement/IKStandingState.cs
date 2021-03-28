using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class IKStandingState : MonoBehaviour, IMovementState
	{
		private Actor actor;
		private float timeInStandingState;
		private float timeUntilIdle = 2f;
		private bool isIdle;
		private IKLegSettings ikLegs;


		public MovementStateType movementStateType
		{
			get => MovementStateType.STANDING;
			set => throw new System.NotImplementedException();
		}

		public void SetActor(Actor owner)
		{
			actor = owner;
			ikLegs = actor.GetComponent<IKLegSettings>();
		}

		public void EnterState(MovementStateType previousState)
		{
			timeInStandingState = 0;
			if (previousState == MovementStateType.FALLING)
				ikLegs.TryPutFeetOnGround();
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			actor.SetAnimator(Actor.IsIdlingHash, false);
			actor.SetAnimator(Actor.IsLongIdlingHash, false);
			isIdle = false;
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

			if (actor.commandList[CommandType.MoveLeft]
				|| actor.commandList[CommandType.MoveRight])
			{
				return MovementStateType.WALKING;
			}

			if (actor.commandList[CommandType.Jetpack])
			{
				return MovementStateType.JETPACK;
			}

			if (actor.currentActionState == ActionStateType.AwaitingAction)
			{
				timeInStandingState += Time.deltaTime;
				if (timeInStandingState >= timeUntilIdle)
				{
					if (isIdle)
					{
						actor.SetAnimator(Actor.IsLongIdlingHash, true);
					}
					else
					{
						isIdle = true;
						timeInStandingState = -timeInStandingState;
						actor.SetAnimator(Actor.IsIdlingHash, true);
					}
				}
			}
			else
			{
				isIdle = false;
				timeInStandingState = 0;
				actor.SetAnimator(Actor.IsIdlingHash, false);
				actor.SetAnimator(Actor.IsLongIdlingHash, false);
			}
			return MovementStateType.NONE;
		}
	}
}