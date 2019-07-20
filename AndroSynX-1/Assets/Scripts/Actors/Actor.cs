using System;
using System.Collections.Generic;
using AtomosZ.AndroSyn.Actors.State;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class Actor : MonoBehaviour
	{
		[NonSerialized] public List<CommandType> commandList = new List<CommandType>();
		public Vector2 inputVelocity = Vector2.zero;
		public MovementStateType currentState;

		internal ActorPhysics actorPhysics;
		[SerializeField] private GroundedState groundedState = null;
		[SerializeField] private DuckingState duckingState = null;
		[SerializeField] private AirbornState airbornState = null;
		private IActorController actorController;
		//Stack<IMovementState> movementStates;
		private IMovementState movementState;
		
		private Dictionary<MovementStateType, IMovementState> movementStateLookup;

		public void Awake()
		{
			actorPhysics = GetComponent<ActorPhysics>();

			groundedState.SetActor(this);
			duckingState.SetActor(this);
			airbornState.SetActor(this);

			movementStateLookup = new Dictionary<MovementStateType, IMovementState>();
			movementStateLookup.Add(MovementStateType.AIRBORN, airbornState);
			movementStateLookup.Add(MovementStateType.DUCK, duckingState);
			movementStateLookup.Add(MovementStateType.GROUNDED, groundedState);

			currentState = MovementStateType.AIRBORN;
			movementState = airbornState;
		}


		public void SetActorController(IActorController icontroller)
		{
			actorController = icontroller;
			actorController.OnActorControl(this);
		}


		public void Update()
		{
			// update commandqueue commands
			actorController.UpdateCommands();
		}


		public void FixedUpdate()
		{
			actorPhysics.UpdateInternalStateFromPhysicsResult();
			actorController.FixedUpdateCommands();
			MovementStateType nextState = movementState.FixedUpdateState();
			if (nextState != MovementStateType.NONE)
			{
				if (!movementStateLookup.TryGetValue(nextState, out IMovementState newMovement))
					Debug.LogWarning("Could not find movementState for " + nextState.ToString());
				else
				{
					MovementStateType previousState = movementState.ExitState(nextState);
					movementState = newMovement;
					movementState.EnterState(previousState);
					currentState = nextState;
				}
			}

			//currentHorizontalSpeed = actorPhysics.isGrounded ? groundHorizontalSpeed : airHorizontalSpeed;
			//Vector2 inputVelocity = input.GetLeftAnalogue();
			//jetpackOn = inputVelocity.y >= jetpackThreshold;

			//inputVelocity.x *= currentHorizontalSpeed;
			//if (jetpackOn)
			//	inputVelocity.y *= jetpackPower;
			//else
			//	inputVelocity.y = 0;
			//actorPhysics.desiredVelocity = inputVelocity;

			actorPhysics.ApplyToPhysics();
		}
	}
}