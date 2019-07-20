using System.Collections.Generic;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class GroundedState : MonoBehaviour, IMovementState
	{
		private const MovementStateType movementStateType = MovementStateType.GROUNDED;
		[SerializeField] private float movementSpeed = 2.4f;
		private Actor actor;


		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(MovementStateType previousState)
		{
			Debug.Log("Entering StandingState");
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			Debug.Log("Exiting StandingState");
			return movementStateType;
		}

		public MovementStateType FixedUpdateState()
		{
			if (!actor.actorPhysics.isGrounded)
				return MovementStateType.AIRBORN;

			Vector2 inputVelocity = actor.inputVelocity;
			inputVelocity.x *= movementSpeed;
			actor.actorPhysics.desiredVelocity = inputVelocity;
			if (inputVelocity.y < 0)
				return MovementStateType.DUCK;
			if (inputVelocity.y > 0)
				return MovementStateType.AIRBORN;


			//jetpackOn = inputVelocity.y >= jetpackThreshold;

			//inputVelocity.x *= currentHorizontalSpeed;
			//if (jetpackOn)
			//	inputVelocity.y *= jetpackPower;
			//else
			//	inputVelocity.y = 0;
			//actorPhysics.desiredVelocity = inputVelocity;

			return MovementStateType.NONE;
		}
	}
}