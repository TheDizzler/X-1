using UnityEngine;

namespace AtomosZ.AndroSyn.Gimmick
{
	[RequireComponent(typeof(CircleCollider2D))]
	public class GraviticObject : MonoBehaviour
	{
		[SerializeField] private float gravityWellRadius = 5;
		[Tooltip("Equivalent to Gm in equation Fg = Gmm/r^2")]
		[SerializeField] private float gravitationalPower = 1;
		[SerializeField] private bool isConstantForce = false;
		private CircleCollider2D gravityWellCollider;
		private float lastRadius;


		public void Start()
		{
			gravityWellCollider = GetComponent<CircleCollider2D>();
			gravityWellCollider.radius = gravityWellRadius;
		}

		public void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.localPosition, gravityWellRadius);
		}

		public void Update()
		{
			if (lastRadius != gravityWellRadius)
			{
				gravityWellCollider.radius = gravityWellRadius;
				lastRadius = gravityWellRadius;
			}
		}

		/// <summary>
		/// Fg = Gm1m2/r^2
		/// If isConstantForce
		///		returns Gm1;
		/// else
		///		returns Gm1/r^2
		/// </summary>
		/// <param name="distance"></param>
		/// <returns></returns>
		public float GetGravitationalPull(float distance)
		{

			if (isConstantForce)
			{
				//return direction / distance * gravityPower;
				return gravitationalPower;
			}

			return gravitationalPower / (distance * distance);
		}
	}
}