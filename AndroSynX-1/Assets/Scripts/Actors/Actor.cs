using System;
using System.Collections.Generic;
using AtomosZ.AndroSyn.Actors.State;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class Actor : MonoBehaviour
	{
		public static readonly int IsDuckingHash = Animator.StringToHash("isDucking");

		public float groundMovementSpeed = 2.4f;
		public float airMovementSpeed = 3.3f;
		/// <summary>
		/// List of Controller device inputs to consume on an actor's movement updates.
		/// </summary>
		[NonSerialized] public Dictionary<CommandType, bool> commandList = new Dictionary<CommandType, bool>();
		[NonSerialized] public Vector2 inputVelocity = Vector2.zero;
		[NonSerialized] public Animator animator;
		/// <summary>
		/// Public for debugging purposes.
		/// </summary>
		public MovementStateType currentState;

		internal ActorPhysics actorPhysics;
		[SerializeField] private GroundedState groundedState = null;
		[SerializeField] private DuckingState duckingState = null;
		[SerializeField] private AirbornState airbornState = null;
		[SerializeField] private JetpackState jetpackState = null;
		private IActorController actorController;
		private IMovementState movementState;

		private Dictionary<MovementStateType, IMovementState> movementStateLookup;


		public void Awake()
		{
			actorPhysics = GetComponent<ActorPhysics>();
			movementStateLookup = new Dictionary<MovementStateType, IMovementState>();
			animator = GetComponent<Animator>();

			if (groundedState)
			{
				groundedState.SetActor(this);
				movementStateLookup.Add(MovementStateType.GROUNDED, groundedState);
			}

			if (duckingState)
			{
				duckingState.SetActor(this);
				movementStateLookup.Add(MovementStateType.DUCK, duckingState);
			}

			if (airbornState)
			{
				airbornState.SetActor(this);
				movementStateLookup.Add(MovementStateType.AIRBORN, airbornState);
			}

			if (jetpackState)
			{
				jetpackState.SetActor(this);
				movementStateLookup.Add(MovementStateType.JETPACK, jetpackState);
			}
			
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
					Debug.Log(this.name + " could not find movementState for " + nextState.ToString());
				else
				{
					MovementStateType previousState = movementState.ExitState(nextState);
					movementState = newMovement;
					movementState.EnterState(previousState);
					currentState = nextState;
				}
			}

			actorPhysics.ApplyToPhysics();
		}
	}
}