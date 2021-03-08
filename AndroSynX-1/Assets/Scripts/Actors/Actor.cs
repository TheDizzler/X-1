using System;
using System.Collections.Generic;
using AtomosZ.AndroSyn.Actors.State;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class Actor : MonoBehaviour
	{
		public static readonly int IsLongIdlingHash = Animator.StringToHash("isLongIdling");
		public static readonly int IsShootingHash = Animator.StringToHash("isShooting");
		public static readonly int IsKneelingHash = Animator.StringToHash("isKneeling");
		public static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
		public static readonly int WalkSpeedHash = Animator.StringToHash("walkSpeed");
		public static readonly int IsFallingHash = Animator.StringToHash("isFalling");
		public static readonly int IsIdlingHash = Animator.StringToHash("isIdling");
		public static readonly int ShootHash = Animator.StringToHash("Shoot");


		[Tooltip("This actor is for debugging and should not run any logic" +
			" at runtime.")]
		public bool isDummy = false;

		public float groundMovementSpeed = 2.4f;
		public float airMovementSpeed = 3.3f;

		public Animator animator;
		/// <summary>
		/// Public for debugging purposes.
		/// </summary>
		public MovementStateType currentMovementState;
		/// <summary>
		/// Public for debugging purposes.
		/// </summary>
		public ActionStateType currentActionState;
		public ActorPhysics actorPhysics;

		/// <summary>
		/// List of Controller device inputs to consume on an actor's movement updates.
		/// </summary>
		[NonSerialized] public Dictionary<CommandType, bool> commandList = new Dictionary<CommandType, bool>();
		[NonSerialized] public Vector2 inputVelocity = Vector2.zero;

		[SerializeField] private StandingState standingState = null;
		[SerializeField] private WalkingState walkingState = null;
		[SerializeField] private KneelingState duckingState = null;
		[SerializeField] private FallingState airbornState = null;
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
			if (!animator)
			{
				animator = GetComponent<Animator>();
				if (!animator)
					Debug.LogError("No animator found for " + gameObject.name);
			}
			movementStateLookup = new Dictionary<MovementStateType, IMovementState>();

			if (standingState)
			{
				standingState.SetActor(this);
				movementStateLookup.Add(MovementStateType.STANDING, standingState);
			}

			if (duckingState)
			{
				duckingState.SetActor(this);
				movementStateLookup.Add(MovementStateType.KNEELING, duckingState);
			}

			if (walkingState)
			{
				walkingState.SetActor(this);
				movementStateLookup.Add(MovementStateType.WALKING, walkingState);
			}

			if (airbornState)
			{
				airbornState.SetActor(this);
				movementStateLookup.Add(MovementStateType.FALLING, airbornState);
			}

			if (jetpackState)
			{
				jetpackState.SetActor(this);
				movementStateLookup.Add(MovementStateType.JETPACK, jetpackState);
			}

			actionStateLookup = new Dictionary<ActionStateType, IActionState>();


			NoActionState awaitingState = new NoActionState();
			awaitingState.SetActor(this);
			actionStateLookup.Add(ActionStateType.AwaitingAction, awaitingState);


			if (shootingState)
			{
				shootingState.SetActor(this);
				actionStateLookup.Add(ActionStateType.Shoot, shootingState);
			}

			currentMovementState = MovementStateType.FALLING;
			movementState = airbornState;
			currentActionState = ActionStateType.None;
			actionState = actionStateLookup[ActionStateType.AwaitingAction];
		}


		public void SetActorController(IActorController icontroller)
		{
			actorController = icontroller;
			actorController.OnActorControl(this);
		}


		public void Flip()
		{
			actorPhysics.isFacingRight = !actorPhysics.isFacingRight;
			Vector3 newScale = transform.localScale;
			newScale.x *= -1;
			transform.localScale = newScale;
		}


		public void Update()
		{
#if UNITY_EDITOR
			if (isDummy)
				return;
			if (actorController == null)
				return; // prevents error spam after editing a script
#endif

			actorController.UpdateCommands();

			ActionStateType nextAction = actionState.FixedUpdateState();
			if (nextAction != ActionStateType.None)
			{
				if (!actionStateLookup.TryGetValue(nextAction, out IActionState newAction))
					Debug.Log(this.name + " could not find actionState for " + nextAction.ToString());
				else
				{
					ActionStateType prevAction = actionState.ExitState(nextAction);
					actionState = newAction;
					actionState.EnterState(prevAction);
					currentActionState = nextAction;
				}
			}

		}


		public void FixedUpdate()
		{
#if UNITY_EDITOR
			if (isDummy)
				return;
			if (actorController == null)
				return; // prevents error spam after editing a script
#endif
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

			actorPhysics.ApplyToPhysics();
		}
	}
}