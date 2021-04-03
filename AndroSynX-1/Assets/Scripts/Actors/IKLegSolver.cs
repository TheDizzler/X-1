using UnityEngine;
using UnityEngine.Experimental.U2D.IK;

namespace AtomosZ.AndroSyn.Actors
{
	public class IKLegSolver : MonoBehaviour
	{
		[SerializeField] private Transform body = null;
		[SerializeField] private Transform foot = null;
		[SerializeField] private LimbSolver2D limbSolver = null;
		[SerializeField] private IKLegSolver otherFoot = null;
		private IKLegSettings ikSettings;
		private float lerpT;
		private bool autoDeactivateOnComplete = false;
		private Vector2 oldPosition, currentPosition, newPosition;
		private Vector2 oldNormal, currentNormal, newNormal;
		private Actor actor;

		public Vector3 forwardCast() => -new Vector3(actor.actorPhysics.up.x, actor.actorPhysics.up.y)
			+ transform.right * ikSettings.stepLength;




		public void SetOwner(Actor owner, IKLegSolver otherFoot, IKLegSettings ikSettings)
		{
			actor = owner;
			this.otherFoot = otherFoot;
			this.ikSettings = ikSettings;
		}


		public void SetTarget(RaycastHit2D stepTarget)
		{
			newPosition = stepTarget.point;
			newNormal = stepTarget.normal;
			lerpT = 0;

			oldPosition = transform.position;
			oldNormal = transform.up;

			limbSolver.enabled = true;
			this.enabled = true;
		}

		public void TryPutFootOnGround()
		{
			RaycastHit2D hit = Physics2D.Raycast(foot.position,
				-actor.actorPhysics.up, ikSettings.raycastDistance, ikSettings.probeMask);
			if (hit.collider != null && hit.distance > .3f)
			{
				oldPosition = currentPosition = transform.position;
				oldNormal = currentNormal = transform.up;
				newPosition = hit.point;
				limbSolver.enabled = true;
				this.enabled = true;
				lerpT = 1;
				autoDeactivateOnComplete = true;
			}

			newPosition = transform.position;
		}


		public void LockAtCurrentPosition()
		{
			currentPosition = newPosition = oldPosition = transform.position;
			currentNormal = newNormal = oldNormal = transform.up;
			lerpT = 1;
			limbSolver.enabled = false;
			this.enabled = false;
		}

		public void Deactivate()
		{
			limbSolver.enabled = false;
			this.enabled = false;
			lerpT = 1;
		}

		void Update()
		{
			if (lerpT >= 1)
			{
				currentPosition = newPosition;
				currentNormal = newNormal;
				if (autoDeactivateOnComplete)
				{
					autoDeactivateOnComplete = false;
					Deactivate();
				}
			}
			else
			{
				Vector2 footPos = Vector2.Lerp(oldPosition, newPosition, lerpT);
				footPos.y += Mathf.Sin(lerpT * Mathf.PI) * ikSettings.kneeStepHeight;

				currentPosition = footPos;
				currentNormal = Vector3.Lerp(oldNormal, newNormal, lerpT);
				lerpT += Time.deltaTime * ikSettings.speed;
			}

			transform.position = currentPosition;
			transform.up = currentNormal;
		}

		//void Update()
		//{
		//	transform.position = currentPosition;
		//	transform.up = currentNormal;


		//	RaycastHit2D hit = Physics2D.Raycast(body.position,
		//		-actor.actorPhysics.up, ikSettings.raycastDistance, ikSettings.probeMask);
		//	if (hit.collider != null)
		//	{
		//		if (Vector2.Distance(newPosition, hit.point) > ikSettings.stepDistance
		//			&& !otherFoot.IsMoving() && lerpT >= 1)
		//		{
		//			int direction =
		//				transform.InverseTransformPoint(hit.point).x > transform.InverseTransformPoint(newPosition).x ? 1 : -1;
		//			Vector2 stepVector = (actor.actorPhysics.Facing() * ikSettings.stepLength * direction);
		//			//newPosition = hit.point + stepVector + footOffset;
		//			newPosition = otherFoot.currentPosition + stepVector;
		//			newPosition.y = hit.point.y;
		//			newNormal = hit.normal;
		//			lerpT = 0;
		//		}
		//	}

		//	if (lerpT < 1)
		//	{
		//		Vector2 footPos = Vector2.Lerp(oldPosition, newPosition, lerpT);
		//		footPos.y += Mathf.Sin(lerpT * Mathf.PI) * ikSettings.stepHeight;

		//		currentPosition = footPos;
		//		currentNormal = Vector3.Lerp(oldNormal, newNormal, lerpT);
		//		lerpT += Time.deltaTime * ikSettings.speed;
		//	}
		//	else
		//	{
		//		oldPosition = newPosition;
		//		oldNormal = newNormal;
		//	}
		//}

		public bool IsMoving()
		{
			return lerpT < 1;
		}

		void OnDrawGizmos()
		{
			if (!Application.isPlaying || actor == null || actor.actorPhysics == null || body == null)
				return;
			Gizmos.color = Color.green;
			Vector3 height = Vector3.Distance(currentPosition, body.position) * -actor.actorPhysics.up;
			Vector3 facing = actor.actorPhysics.Facing();
			Vector3 raycastTarget = new Vector3(body.position.x, body.position.y) +
				facing * ikSettings.stepLength + height;
			Gizmos.DrawLine(body.position, raycastTarget);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(newPosition, .1f);
		}
	}
}