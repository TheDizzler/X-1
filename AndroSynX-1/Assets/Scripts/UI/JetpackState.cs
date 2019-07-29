﻿using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class JetpackState : MonoBehaviour, IMovementState
	{
		[SerializeField] private int maxJetpackEnergy = 10;
		public float currentEnergy = 10;
		[SerializeField] private float jetpackPower = 3.3f;
		/// <summary>
		/// Time since used jetpack that starts recharging.
		/// </summary>
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
			//Debug.Log("Entering JetpackState");
			jetpackOn = true;
			Vector2 inputVelocity = Vector2.zero;
			inputVelocity.y = actor.inputVelocity.y * jetpackPower;
			actor.actorPhysics.desiredVelocity = inputVelocity;
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			//Debug.Log("Exiting JetpackState");
			jetpackOn = false;
			return movementStateType;
		}

		public void Update()
		{
			if (jetpackOn)
			{
				currentEnergy -= 2 * Time.deltaTime;
				if (currentEnergy <= 0)
					currentEnergy = 0;
			}
			else
			{
				timeUntilRecharge -= Time.deltaTime;
				if (timeUntilRecharge <= 0)
				{
					currentEnergy += 2 * Time.deltaTime;
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
			}

			if (!jetpackOn)
			{
				timeUntilRecharge = rechargeDelay;
				actor.actorPhysics.desiredVelocity = inputVelocity;
				return MovementStateType.AIRBORN;
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