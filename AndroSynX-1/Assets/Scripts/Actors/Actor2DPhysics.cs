using AtomosZ.AndroSyn.GamePhysics;
using AtomosZ.AndroSyn.Gimmick;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	/// <summary>
	/// Class to manage interactions with physics for a player character
	/// </summary>
	public class Actor2DPhysics : MonoBehaviour, IActorPhysics
	{
		private static readonly float Cos45 = Mathf.Cos(Mathf.Deg2Rad * 47f); // Just over 45 degrees...

		public bool isGrounded { get; set; }
		/// <summary>
		/// Desired velocity derived from input/etc, where:
		/// - DesiredVelocity.X is along this.Right (NOT Facing) 
		/// - DesiredVelocity.Y is along this.Up 
		/// </summary>
		public Vector2 desiredVelocity { get; set; }
		/// <summary>
		/// Is the Character currently facing the GameObject's Right vector?
		/// </summary>
		public bool isFacingRight { get; set; }

		[SerializeField] private float moveCollisionDistance = .1f;
		[SerializeField] private Collider2D groundCollider = null;
		/// <summary>
		/// Gravity to be applied to actor from external forces NOT including Area Gravity. 
		/// </summary>
		private Vector2 affectingGravity;
		/// <summary>
		/// The current direction that this character should regard as "Up" for gravity/etc
		/// </summary>
		public Vector2 up { get; set; }
		public Vector2 right;


		private Vector2 lastAffectingGravity = Vector2.zero;
		private Rigidbody2D rb2d;
		// Shared array for putting contacts into to avoid allocs
		private int contactCount = 0;
		private ContactPoint2D[] contactPoints = new ContactPoint2D[10];
		private RaycastHit2D[] results;
		private Vector3 footOffset;
		private Actor actor;
		private Vector2 contactNormal;
		private Vector3 slopeVector;



		public void Awake()
		{
			rb2d = GetComponent<Rigidbody2D>();
			if (groundCollider == null)
				groundCollider = GetComponent<Collider2D>();
			results = new RaycastHit2D[2];
			isFacingRight = true;
			up = transform.up;
			right = transform.right;

			footOffset = new Vector3(0, -groundCollider.bounds.size.y / 2);
			actor = GetComponent<Actor>();
		}

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

		/// <summary>
		/// For informational purposes (such as the gravity rose). Not for scientific use.
		/// </summary>
		/// <returns></returns>
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

		public Vector2 GetVelocity()
		{
			return rb2d.velocity;
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

			stepsSinceLastGrounded += 1;
			bool wasGrounded = isGrounded;
			isGrounded = false;
			contactCount = groundCollider.GetContacts(contactPoints);
			for (int i = 0; i < contactCount; i++)
			{
				var contact = contactPoints[i];
				var dot = Vector2.Dot(contact.normal, up);
				if (dot > Cos45)
				{
					contactNormal = contact.normal;
					slopeVector = Vector3.Cross(contactNormal, Vector3.down);
					isGrounded = true;
					stepsSinceLastGrounded = 0;
					break;
				}
			}

			if (stepsSinceLastGrounded == 1 && wasGrounded && !isGrounded)
			{
				// check if ungrounding was intended, if not, stick to ground
				SnapToGround();
			}
		}

		private void SnapToGround()
		{
			if (!(groundCollider.Cast(-up, results, moveCollisionDistance) > 0))
				return;

			
			if (results[0].normal.y < Cos45)
				return;

			isGrounded = true;
			contactNormal = results[0].normal;
			slopeVector = Vector3.Cross(contactNormal, -up);
			Vector3 pos = transform.position;
			pos.y -= results[0].distance;
			transform.position = pos;
		}

		private int stepsSinceLastGrounded = 0;

		public void ApplyToPhysics()
		{
			Vector2 grav = affectingGravity;
			Vector2 v = Vector2.zero;
			var forward = isGrounded ? Vector2.Perpendicular(-contactNormal) : Vector2.Perpendicular(-up);
			v += forward * desiredVelocity.x;

			if (isGrounded)
			{
				grav.y = 0;
			}
			else if (CheckForCollision(new Vector2(v.x, 0).normalized))
			{
				v.x = 0;
			}

			v += up * desiredVelocity.y + grav;
			rb2d.velocity = v;

			lastAffectingGravity = affectingGravity;
			affectingGravity = Vector2.zero;
			desiredVelocity = Vector2.zero;
		}


		public bool CheckForCollision(Vector2 normalizedDirection)
		{
			return groundCollider.Cast(normalizedDirection, results, moveCollisionDistance) > 0;
		}

		void OnTriggerStay2D(Collider2D collision)
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