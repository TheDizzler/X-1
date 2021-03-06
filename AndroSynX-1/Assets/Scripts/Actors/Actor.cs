﻿using System;
using System.Collections.Generic;
using AtomosZ.AndroSyn.Actors.State;
using AtomosZ.AndroSyn.Gadgets;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class Actor : MonoBehaviour
	{
		public static readonly int IsLongIdlingHash = Animator.StringToHash("isLongIdling");
		public static readonly int IsShootingHash = Animator.StringToHash("isShooting");
		public static readonly int IsKneelingHash = Animator.StringToHash("isKneeling");
		public static readonly int IsJetpackHash = Animator.StringToHash("isJetpack");
		public static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
		public static readonly int WalkSpeedHash = Animator.StringToHash("walkSpeed");
		public static readonly int IsFallingHash = Animator.StringToHash("isFalling");
		public static readonly int IsIdlingHash = Animator.StringToHash("isIdling");
		public static readonly int ShootHash = Animator.StringToHash("Shoot");


		[Tooltip("This actor is for debugging and should not run any logic" +
			" at runtime.")]
		public bool isDummy = false;

		[Tooltip("Horizontal move speed")]
		public float groundMovementSpeed = 2.4f;
		[Tooltip("Horizontal move speed")]
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
		public IActorPhysics actorPhysics;

		public bool isNearElevator = false;
		private Elevator nearElevator;

		/// <summary>
		/// List of Controller device inputs to consume on an actor's movement updates.
		/// </summary>
		[NonSerialized] public Dictionary<CommandType, bool> commandList = new Dictionary<CommandType, bool>();
		[NonSerialized] public Vector2 inputVelocity = Vector2.zero;
		[SerializeField] private StandingState standingState = null;
		[SerializeField] private WalkingState walkingState = null;
		[SerializeField] private IKClimbingStairsState stairsState = null;
		[SerializeField] private KneelingState duckingState = null;
		[SerializeField] private FallingState airbornState = null;
		[SerializeField] private JetpackState jetpackState = null;
		[SerializeField] private ShootingState shootingState = null;

		private IActorController actorController;
		private IMovementState movementState;
		private Dictionary<MovementStateType, IMovementState> movementStateLookup;
		private IActionState actionState;
		private ElevatorState elevatorState;
		private Dictionary<ActionStateType, IActionState> actionStateLookup;
		private SpriteRenderer sprite;


		public void Awake()
		{
			sprite = GetComponent<SpriteRenderer>();
			actorPhysics = GetComponent<IActorPhysics>();
			if (!animator)
			{
				animator = GetComponent<Animator>();
				if (!animator)
					Debug.LogWarning("No animator found for " + gameObject.name);
			}

			actionStateLookup = new Dictionary<ActionStateType, IActionState>();
			movementStateLookup = new Dictionary<MovementStateType, IMovementState>();

			if (!standingState)
				standingState = GetComponentInChildren<StandingState>();
			if (standingState)
			{
				standingState.SetActor(this);
				movementStateLookup.Add(MovementStateType.STANDING, standingState);
			}

			if (!duckingState)
				duckingState = GetComponentInChildren<KneelingState>();
			if (duckingState)
			{
				duckingState.SetActor(this);
				movementStateLookup.Add(MovementStateType.KNEELING, duckingState);
			}

			if (!walkingState)
				walkingState = GetComponentInChildren<WalkingState>();
			if (walkingState)
			{
				walkingState.SetActor(this);
				movementStateLookup.Add(MovementStateType.WALKING, walkingState);
			}

			if (stairsState)
			{
				stairsState.SetActor(this);
				movementStateLookup.Add(MovementStateType.STAIRS, stairsState);
			}

			if (!airbornState)
				airbornState = GetComponentInChildren<FallingState>();
			if (airbornState)
			{
				airbornState.SetActor(this);
				movementStateLookup.Add(MovementStateType.FALLING, airbornState);
			}

			if (!jetpackState)
				jetpackState = GetComponentInChildren<JetpackState>();
			if (jetpackState)
			{
				jetpackState.SetActor(this);
				movementStateLookup.Add(MovementStateType.JETPACK, jetpackState);
			}

			elevatorState = GetComponentInChildren<ElevatorState>();
			if (elevatorState)
			{
				elevatorState.SetActor(this);
				movementStateLookup.Add(MovementStateType.ELEVATOR, elevatorState);
				actionStateLookup.Add(ActionStateType.ELEVATOR, elevatorState);
			}



			NoActionState awaitingState = new NoActionState();
			awaitingState.SetActor(this);
			actionStateLookup.Add(ActionStateType.AWAITING_ACTION, awaitingState);

			if (!shootingState)
				shootingState = GetComponentInChildren<ShootingState>();
			if (shootingState)
			{
				shootingState.SetActor(this);
				actionStateLookup.Add(ActionStateType.SHOOT, shootingState);
			}

			currentMovementState = MovementStateType.FALLING;
			movementState = airbornState;
			currentActionState = ActionStateType.AWAITING_ACTION;
			actionState = actionStateLookup[ActionStateType.AWAITING_ACTION];
		}


		public void SetActorController(IActorController icontroller)
		{
			actorController = icontroller;
			actorController.OnActorControl(this);
		}

		public void SetAnimator(int animationHash, bool value)
		{
			if (animator != null)
			{
				animator.SetBool(animationHash, value);
			}
		}

		public void SetAnimator(int animationHash, float value)
		{
			if (animator != null)
			{
				animator.SetFloat(animationHash, value);
			}
		}

		public void NearElevator(Elevator elevator)
		{
			isNearElevator = elevator == null ? false : true;
			elevatorState.elevator = elevator;
		}

		/// <summary>
		/// A hack to get button input in the elevator.
		/// </summary>
		public void EnterElevator()
		{
			ActionStateType nextAction = ActionStateType.ELEVATOR;
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

		public int SetZDepth(int newZDepth)
		{
			int oldZ = sprite.sortingOrder;
			sprite.sortingOrder = newZDepth;
			return oldZ;
		}

		public string SetSpriteSortingLayer(string sortingLayer)
		{
			string oldLayer = sprite.sortingLayerName;
			sprite.sortingLayerName = sortingLayer;
			return oldLayer;
		}


		public void Flip()
		{
			actorPhysics.isFacingRight = !actorPhysics.isFacingRight;
			Vector3 newScale = transform.localScale;
			newScale.x *= -1;
			transform.localScale = newScale;
		}


		void Update()
		{
#if UNITY_EDITOR
			if (isDummy)
				return;
			if (actorController == null)
				return; // prevents error spam after editing a script
#endif

			actorController.UpdateCommands();

			ActionStateType nextAction = actionState.UpdateState();
			if (nextAction != ActionStateType.NONE)
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


		void FixedUpdate()
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