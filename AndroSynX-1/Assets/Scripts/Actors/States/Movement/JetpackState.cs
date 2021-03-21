using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class JetpackState : MonoBehaviour, IMovementState
	{
		/// <summary>
		/// How much energy is drained per second of jetpack operation
		/// </summary>
		[Tooltip("How much energy is drained per second of jetpack operation")]
		[SerializeField] private int jetpackDrain = 5;
		/// <summary>
		/// How much energy is recharged per second when not in operation.
		/// </summary>
		[Tooltip("How much energy is recharged per second when not in operation.")]
		[SerializeField] private int jetpackRecharge = 5;
		[SerializeField] private int maxJetpackEnergy = 100;
		public float currentEnergy = 10;
		[SerializeField] private float jetpackPower = 3.3f;
		/// <summary>
		/// Time since used jetpack before recharging starts.
		/// </summary>
		[Tooltip("Time since used jetpack before recharging starts.")]
		[SerializeField] private float rechargeDelay = 1f;
		private float timeUntilRecharge = 0;

		private bool jetpackOn = false;
		private Actor actor;


		public MovementStateType movementStateType
		{
			get => MovementStateType.JETPACK;
			set => throw new System.NotImplementedException();
		}


		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(MovementStateType previousState)
		{
			jetpackOn = true;
			Vector2 inputVelocity = Vector2.zero;
			inputVelocity.y = actor.inputVelocity.y * jetpackPower;
			actor.actorPhysics.desiredVelocity = inputVelocity;
			actor.SetAnimator(Actor.IsJetpackHash, true);
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			jetpackOn = false;
			actor.SetAnimator(Actor.IsJetpackHash, false);
			return movementStateType;
		}

		public void Update()
		{
			if (jetpackOn)
			{
				currentEnergy -= jetpackDrain * Time.deltaTime;
				if (currentEnergy <= 0)
					currentEnergy = 0;
			}
			else
			{
				timeUntilRecharge -= Time.deltaTime;
				if (timeUntilRecharge <= 0)
				{
					currentEnergy += jetpackRecharge * Time.deltaTime;
					if (currentEnergy > maxJetpackEnergy)
						currentEnergy = maxJetpackEnergy;
				}
			}
		}


		public MovementStateType FixedUpdateState()
		{
			Vector2 inputVelocity = Vector2.zero;
			bool jetpackWasOn = jetpackOn;
			jetpackOn = actor.commandList[CommandType.Jetpack];

			if (actor.commandList[CommandType.MoveLeft]
				|| actor.commandList[CommandType.MoveRight])
			{
				inputVelocity.x = actor.inputVelocity.x * actor.airMovementSpeed;
				bool wasFacingRight = actor.actorPhysics.isFacingRight;
				bool isFacingRight = inputVelocity.x > 0;
				if (wasFacingRight != isFacingRight)
					actor.Flip();
			}

			if (!jetpackOn)
			{
				timeUntilRecharge = rechargeDelay;
				actor.actorPhysics.desiredVelocity = inputVelocity;
				return MovementStateType.FALLING;
			}

			if (currentEnergy <= 0)
			{
				inputVelocity.y = 0;
				jetpackOn = false;
			}
			else if (currentEnergy < 1 && !jetpackWasOn)
			{
				inputVelocity.y = 0;
				jetpackOn = false;
			}
			else if (currentEnergy >= 0)
			{
				inputVelocity.y = actor.inputVelocity.y * jetpackPower;
				timeUntilRecharge = rechargeDelay;
			}

			actor.actorPhysics.desiredVelocity = inputVelocity;
			return MovementStateType.NONE;
		}
	}
}