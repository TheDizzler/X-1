using AtomosZ.AndroSyn.Gimmick;
using UnityEngine;

namespace AtomosZ.AndroSyn.Weapons
{
	public class Bullet : MonoBehaviour
	{
		public float speed = 5.0f;
		[SerializeField] private Rigidbody2D rb2d = null;
		private Vector2 affectingGravity;

		public void Fire(Vector2 position, Vector2 normalizedDirection)
		{
			transform.localPosition = position;
			rb2d.velocity = speed * normalizedDirection;
		}

		void FixedUpdate()
		{
			Vector2 v = rb2d.velocity;
			v += affectingGravity;
			rb2d.velocity = v;

			affectingGravity = Vector2.zero;
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