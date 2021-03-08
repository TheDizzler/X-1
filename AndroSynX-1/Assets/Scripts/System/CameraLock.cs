using UnityEngine;

namespace AtomosZ.AndroSyn.GameSystem
{
	public class CameraLock : MonoBehaviour
	{
		private Camera mainCamera;
		private Bounds areaBounds;


		void Start()
		{
			mainCamera = GetComponent<Camera>();
			var collider = GameObject.FindGameObjectWithTag(Tags.AREA_PHYSICS).GetComponentInChildren<BoxCollider2D>();

			areaBounds = new Bounds(collider.bounds.center, collider.size);
		}

		public Vector3 LockToBounds(Bounds cameraBounds)
		{
			if (!cameraBounds.Intersects(areaBounds))
			{
				return cameraBounds.center;
			}

			Vector3 moveTo = cameraBounds.center;
			float maxX = cameraBounds.max.x;
			float maxY = cameraBounds.max.y;
			float minX = cameraBounds.min.x;
			float minY = cameraBounds.min.y;
			if (maxX > areaBounds.max.x)
				moveTo.x = areaBounds.max.x - cameraBounds.extents.x;
			else if (minX < areaBounds.min.x)
				moveTo.x = areaBounds.min.x + cameraBounds.extents.x;
			if (maxY > areaBounds.max.y)
				moveTo.y = areaBounds.max.y - cameraBounds.extents.y;
			else if (minY < areaBounds.min.y)
				moveTo.y = areaBounds.min.y + cameraBounds.extents.y;
			return moveTo;
		}
	}
}