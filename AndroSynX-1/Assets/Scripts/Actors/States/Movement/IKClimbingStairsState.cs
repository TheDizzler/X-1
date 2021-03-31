using AtomosZ.AndroSyn.Actors.State;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class IKClimbingStairsState : MonoBehaviour, IMovementState
	{
		public enum StepPhase
		{
			NotOnSteps = 0,
			LegMoving,
			LegLanded,
			BodyMoving,
		}


		public MovementStateType movementStateType
		{
			get => MovementStateType.STAIRS;
			set => throw new System.NotImplementedException();
		}

		[SerializeField] private IKLegSettings ikLegSettings = null;
		private Actor actor;
		private IKActorPhysics actorPhysics;
		private Transform body;
		private IKLegSolver[] legSolvers;
		private Collider2D lifterCollider;
		private Vector3 stepBodyTarget;
		private Vector3 originalStepPosition;
		private int lastFootToMove;

		private StepPhase stepPhase;
		private float stepLerp = 0;


		public void SetActor(Actor owner)
		{
			actor = owner;
			actorPhysics = (IKActorPhysics)actor.actorPhysics;
			if (actorPhysics == null)
				throw new System.Exception("Invalid ActorPhysics type");
			if (ikLegSettings == null)
				ikLegSettings = actor.GetComponent<IKLegSettings>();
			body = ikLegSettings.body;
			legSolvers = ikLegSettings.legSolvers;
			lifterCollider = actorPhysics.lifterCollider;
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawCube(originalStepPosition, new Vector3(.125f, .125f, .125f));
			Gizmos.color = Color.yellow;
			Gizmos.DrawCube(stepBodyTarget, new Vector3(.125f, .125f, .125f));
		}


		public void EnterState(MovementStateType previousState)
		{
			RaycastHit2D hit = Physics2D.Raycast(body.position
					+ new Vector3(ikLegSettings.stepLength * (actorPhysics.isFacingRight ? 1 : -1), 0, 0),
				-actorPhysics.up, ikLegSettings.raycastDistance, ikLegSettings.probeMask);
			if (hit.collider == null)
			{
				Debug.LogError("INVALID STATE");
			}

			var dot = Vector2.Dot(hit.normal, actorPhysics.up);
			if (dot <= IKActorPhysics.Cos45
				|| hit.point.y > ikLegSettings.MaxStepHeight())
			{
				Debug.LogError("INVALID STATE");
			}
			else
			{
				stepPhase = StepPhase.LegMoving;
				legSolvers[lastFootToMove].LockAtCurrentPosition();
				lastFootToMove = lastFootToMove == 1 ? 0 : 1;
				legSolvers[lastFootToMove].SetTarget(hit);
				Vector3 diff = (Vector3)hit.point - legSolvers[lastFootToMove].transform.position;
				stepBodyTarget.x = hit.point.x;
				stepBodyTarget.y = body.position.y + diff.y;
				originalStepPosition = transform.position;
				stepLerp = 0f;
			}
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			foreach (var leg in ikLegSettings.legSolvers)
				leg.Deactivate();
			lifterCollider.enabled = true;
			return movementStateType;
		}

		public MovementStateType FixedUpdateState()
		{
			//if (!actor.actorPhysics.isGrounded)
			//	return MovementStateType.FALLING;

			if (actor.commandList[CommandType.Kneel])
			{
				Debug.LogWarning("Need special case for kneeling on stairs?");
				actor.commandList[CommandType.Kneel] = false;
				return MovementStateType.KNEELING;
			}

			if (actor.commandList[CommandType.Jetpack])
			{
				return MovementStateType.JETPACK;
			}

			switch (stepPhase)
			{
				case StepPhase.LegMoving:
					if (!legSolvers[lastFootToMove].IsMoving())
					{
						stepPhase = StepPhase.LegLanded;
						lifterCollider.enabled = false;
						stepLerp = 0;
					}
					break;

				case StepPhase.LegLanded:
					if (actor.commandList[CommandType.MoveLeft]
						|| actor.commandList[CommandType.MoveRight])
					{
						//stepPhase = StepPhase.BodyMoving;
						lifterCollider.enabled = true;
						return MovementStateType.STANDING;
					}
					break;

				case StepPhase.BodyMoving:
					stepLerp += Time.deltaTime * ikLegSettings.speed;

					if (stepLerp >= 1)
					{
						actor.transform.position = stepBodyTarget;
						lifterCollider.enabled = true;
						return MovementStateType.STANDING;
					}
					else
					{
						actor.transform.position = Vector3.Lerp(
							originalStepPosition, stepBodyTarget, stepLerp);
					}
					break;
			}



			return MovementStateType.NONE;
		}
	}
}