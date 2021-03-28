using AtomosZ.AndroSyn.GamePhysics;
using AtomosZ.AndroSyn.Gimmick;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class IKActorPhysics : MonoBehaviour, IActorPhysics
	{
		public static readonly float Cos45 = Mathf.Cos(Mathf.Deg2Rad * 47f); // Just over 45 degrees...


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


		/// <summary>
		/// The current direction that this character should regard as "Up" for gravity/etc
		/// </summary>
		public Vector2 up { get; set; }
		public Vector2 right;

		public Collider2D lifterCollider = null;
		[SerializeField] private float moveCollisionDistance = .1f;

		/// <summary>
		/// Gravity to be applied to actor from external forces NOT including Area Gravity. 
		/// </summary>
		private Vector2 affectingGravity;
		private Vector2 lastAffectingGravity = Vector2.zero;
		private Rigidbody2D rb2d;
		private Actor actor;

		// Shared array for putting contacts into to avoid allocs
		private int contactCount = 0;
		private ContactPoint2D[] contactPoints = new ContactPoint2D[10];
		private RaycastHit2D[] results;
		private Vector2 contactNormal;
		private Vector3 slopeVector;


		public void Awake()
		{
			rb2d = GetComponent<Rigidbody2D>();
			actor = GetComponent<Actor>();
			results = new RaycastHit2D[2];
			isFacingRight = true;
			up = transform.up;
			right = transform.right;
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

		public void UpdateInternalStateFromPhysicsResult()
		{
			affectingGravity += AreaPhysics.gravity;

			up = -AreaPhysics.gravity.normalized;
			contactNormal = up;

			Vector2 contactPoint = Vector2.zero;
			bool wasGrounded = isGrounded;
			isGrounded = false;
			contactCount = lifterCollider.GetContacts(contactPoints);

			for (int i = 0; i < contactCount; i++)
			{
				var contact = contactPoints[i];
				var dot = Vector2.Dot(contact.normal, up);
				if (dot > Cos45)
				{
					contactNormal = contact.normal;
					slopeVector = Vector3.Cross(contact.normal, Vector3.down);
					isGrounded = true;
					contactPoint = contact.point;
					break;
				}
			}
		}


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
			return lifterCollider.Cast(normalizedDirection, results, moveCollisionDistance) > 0;
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