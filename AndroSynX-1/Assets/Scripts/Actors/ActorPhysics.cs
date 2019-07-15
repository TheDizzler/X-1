﻿using AtomosZ.AndroSyn.Gimmick;
using AtomosZ.AndroSyn.Physics;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actor
{
	/// <summary>
	/// Class to manage interactions with physics for a player character
	/// </summary>
	public class ActorPhysics : MonoBehaviour
	{
		const int CharacterLayer = 13;
		const int CharacterIgnoringCharactersLayer = 14;

		static readonly float Cos45 = Mathf.Cos(Mathf.Deg2Rad * 47f); // Just over 45 degrees...

		/// <summary>
		/// Gravity to be applied to actor from external forces NOT including Area Gravity. 
		/// </summary>
		private Vector2 affectingGravity;
		/// <summary>
		/// The current direction that this character should regard as "Up" for gravity/etc
		/// </summary>
		public Vector2 up;
		public Vector2 right;

		/// <summary>
		/// Is the Character currently facing the GameObject's Right vector?
		/// </summary>
		public bool isFacingRight;
		public bool isGrounded;
		/// <summary>
		/// Desired velocity derived from input/etc, where:
		/// - DesiredVelocity.X is along this.Right (NOT Facing) 
		/// - DesiredVelocity.Y is along this.Up 
		/// </summary>
		public Vector2 desiredVelocity;

		private Vector2 lastAffectingGravity = Vector2.zero;
		private Rigidbody2D rb2d;
		private Collider2D mainCollider;
		// Shared array for putting contacts into to avoid allocs
		private int contactCount = 0;
		private ContactPoint2D[] contactPoints = new ContactPoint2D[10];
		private Vector3 footOffset;
		private Vector2 contactNormal;
		private float contactNormalDotUp;



		public void Awake()
		{
			rb2d = GetComponent<Rigidbody2D>();
			mainCollider = GetComponent<BoxCollider2D>();

			isFacingRight = true;
			up = transform.up;
			right = transform.right;
			footOffset = new Vector3(0, -mainCollider.bounds.size.y / 2);
		}
		/// <summary>
		/// dsfsad
		/// </summary>
		public void OnDrawGizmos()
		{
			Gizmos.color = Color.magenta;
			for (int i = 0; i < contactCount; i++)
			{
				var contact = contactPoints[i];
				Gizmos.DrawLine(contact.point, contact.point + (contact.normal * 0.5f));
			}

			if (isGrounded)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireSphere(transform.position + footOffset, 0.5f);
			}
		}

		public Vector3 GetTotalAffectingGravity()
		{
			return lastAffectingGravity;
		}

		/// <summary>
		/// Change which direction is "Up"
		/// </summary>
		/// <param name="newUp"></param>
		public void ChangeUpDirection(Vector2 newUp)
		{
			float cosAngle = Vector2.Dot(newUp, up);
			float angleDegrees = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
			transform.RotateAround(transform.position, Vector3.forward, angleDegrees);
			up = newUp;
			right = -Vector2.Perpendicular(up);
		}

		/// <summary>
		/// The normalized direction that the character is currently "facing"
		/// </summary>
		public Vector2 Facing()
		{
			return isFacingRight ? right : -right;
		}

		/// <summary>
		/// Look at physics and update internal state, in preparation for whatever manipulates this object
		/// to be able to decide what to do next.
		/// </summary>
		public void UpdateInternalStateFromPhysicsResult()
		{
			affectingGravity += AreaPhysics.gravity;

			up = -AreaPhysics.gravity.normalized;
			contactNormal = up;

			bool wasGrounded = isGrounded;
			isGrounded = false;

			contactCount = mainCollider.GetContacts(contactPoints);
			for (int i = 0; i < contactCount; i++)
			{
				var contact = contactPoints[i];
				var dot = Vector2.Dot(contact.normal, up);
				if (dot > Cos45)
				{
					contactNormal = contact.normal;
					isGrounded = true;
					break;
				}
			}
		}

		public void ApplyToPhysics()
		{
			Vector2 v = Vector2.zero;
			var forward = isGrounded ? Vector2.Perpendicular(-contactNormal) : Vector2.Perpendicular(-up);
			v += forward * desiredVelocity.x;
			v += up * desiredVelocity.y + affectingGravity;
			rb2d.velocity = v;

			lastAffectingGravity = affectingGravity;
			affectingGravity = Vector2.zero;
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			if (collision.CompareTag(Tags.GRAVITIC_OBJECT))
			{
				float power = collision.GetComponent<GraviticObject>().GetGravitationalPull(
					Vector2.Distance(transform.localPosition, collision.transform.localPosition));

				Vector2 direction = collision.transform.localPosition - transform.localPosition;

				affectingGravity += direction * power * rb2d.mass;
			}
		}
	}
}