using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class DuckingState : MonoBehaviour, IMovementState
	{
		private const MovementStateType movementStateType = MovementStateType.DUCK;
		private Actor actor;


		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(MovementStateType previousState)
		{
			Debug.Log("Entering DuckingState");
		}

		public MovementStateType ExitState(MovementStateType exitState)
		{
			Debug.Log("Exiting DuckingState");
			return movementStateType;
		}

		public MovementStateType FixedUpdateState()
		{
			if (!actor.actorPhysics.isGrounded)
				return MovementStateType.AIRBORN;
			if (actor.inputVelocity.y >= 0)
				return MovementStateType.GROUNDED; // I feel this should change to Standing and Grounded is the superstate
			return MovementStateType.NONE;
		}
	}
}