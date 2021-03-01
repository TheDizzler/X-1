using System;
using System.Collections.Generic;
using AtomosZ.AndroSyn.Actors.State;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class Actor : MonoBehaviour
	{
		public static readonly int IsDuckingHash = Animator.StringToHash("isDucking");
		public static readonly int IsShooting = Animator.StringToHash("isShooting");

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
		public MovementStateType currentMovementState;
		/// <summary>
		/// Public for debugging purposes.
		/// </summary>
		public ActionStateType currentActionState;


		public ActorPhysics actorPhysics;
		[SerializeField] private GroundedState groundedState = null;
		[SerializeField] private DuckingState duckingState = null;
		[SerializeField] private AirbornState airbornState = null;
		[SerializeField] private JetpackState jetpackState = null;
		[SerializeField] private ShootingState shootingState = null;

		private IActorController actorController;
		private IMovementState movementState;
		private Dictionary<MovementStateType, IMovementState> movementStateLookup;
		private IActionState actionState;
		private Dictionary<ActionStateType, IActionState> actionStateLookup;


		public void Awake()
		{
			actorPhysics = GetComponent<ActorPhysics>();
			animator = GetComponent<Animator>();
			movementStateLookup = new Dictionary<MovementStateType, IMovementState>();

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

			actionStateLookup = new Dictionary<ActionStateType, IActionState>();
			actionStateLookup.Add(ActionStateType.None, null);

			if (shootingState)
			{
				shootingState.SetActor(this);
				actionStateLookup.Add(ActionStateType.Shoot, shootingState);
			}

			currentMovementState = MovementStateType.AIRBORN;
			movementState = airbornState;
			currentActionState = ActionStateType.None;
			actionState = null;
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
					currentMovementState = nextState;
				}
			}

			ActionStateType nextAction = actionState.FixedUpdateState();
			if (nextAction != ActionStateType.None)
			{
				if (!actionStateLookup.TryGetValue(nextAction, out IActionState newAction))
					Debug.Log(this.name + " could not find movementState for " + nextState.ToString());
				else
				{
					ActionStateType prevAction = actionState.ExitState(nextAction);
					actionState = newAction;
					actionState.EnterState(prevAction);
					currentActionState = nextAction;
				}
			}

			actorPhysics.ApplyToPhysics();
		}
	}
}