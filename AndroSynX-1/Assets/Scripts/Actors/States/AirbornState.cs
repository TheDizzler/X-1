using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class AirbornState : MonoBehaviour, IMovementState
	{
		private const MovementStateType movementStateType = MovementStateType.AIRBORN;
		[SerializeField] private int maxJetpackEnergy = 10;
		[SerializeField] private float jetpackEnergy = 10;
		[SerializeField] private float jetpackPower = 3.3f;
		[SerializeField] private float movementSpeed = 3.3f;
		private Actor actor;
		private bool jetpackOn = false;


		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(MovementStateType previousState)
		{
			Debug.Log("Entering AirbornState");
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			Debug.Log("Exiting AirbornState");
			return movementStateType;
		}

		public void Update()
		{
			if (jetpackOn)
				jetpackEnergy -= 2 * Time.deltaTime;
			else
			{
				jetpackEnergy += 2 * Time.deltaTime;
				if (jetpackEnergy > maxJetpackEnergy)
					jetpackEnergy = maxJetpackEnergy;
			}
		}

		public MovementStateType FixedUpdateState()
		{
			if (actor.actorPhysics.isGrounded)
			{
				jetpackOn = false;
				return MovementStateType.GROUNDED;
			}

			Vector2 inputVelocity = actor.inputVelocity;
			jetpackOn = inputVelocity.y > 0;
			inputVelocity.x *= movementSpeed;
			if (jetpackOn)
			{
				if (jetpackEnergy >= 1)
					inputVelocity.y *= jetpackPower;
			}
			else
				inputVelocity.y = 0;
			actor.actorPhysics.desiredVelocity = inputVelocity;

			return MovementStateType.NONE;
		}
	}
}