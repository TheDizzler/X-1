using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class StandingState : MonoBehaviour, IMovementState
	{
		private Actor actor;
		private float timeInStandingState;
		private float timeUntilIdle = 2f;
		private bool isIdle;

		public MovementStateType movementStateType
		{
			get => MovementStateType.STANDING;
			set => throw new System.NotImplementedException();
		}

		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(MovementStateType previousState)
		{
			//Debug.Log("Entering StandingState");
			timeInStandingState = 0;
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			actor.animator.SetBool(Actor.IsIdlingHash, false);
			actor.animator.SetBool(Actor.IsLongIdlingHash, false);
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

			timeInStandingState += Time.deltaTime;
			if (timeInStandingState >= timeUntilIdle)
			{
				if (isIdle)
				{
					actor.animator.SetBool(Actor.IsLongIdlingHash, true);
				}
				else
				{
					isIdle = true;
					timeInStandingState = -timeInStandingState;
					actor.animator.SetBool(Actor.IsIdlingHash, true);
				}
			}


			return MovementStateType.NONE;
		}
	}
}