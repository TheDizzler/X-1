using UnityEngine;

namespace AtomosZ.AndroSyn.GamePhysics
{
	public class AreaPhysics : MonoBehaviour
	{
		public static readonly Vector2 STANDARD_GRAVITY = new Vector2(0, -9.81f);
		public static Vector2 gravity;

		[SerializeField] private Vector2 localGravity = new Vector2(0, -9.8f);


		public void Awake()
		{
			gravity = localGravity;
		}

		public void Update()
		{
#if UNITY_EDITOR
			gravity = localGravity;
#endif
		}
	}
}