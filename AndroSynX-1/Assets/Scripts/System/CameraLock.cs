using UnityEngine;

namespace AtomosZ.AndroSyn.GameSystem
{
	public class CameraLock : MonoBehaviour
	{
		private Camera mainCamera;
		private BoxCollider2D areaBounds;


		void Start()
		{
			mainCamera = GetComponent<Camera>();
			areaBounds = GameObject.FindGameObjectWithTag(Tags.AREA_PHYSICS).GetComponentInChildren<BoxCollider2D>();

			Vector3 farPoint0 = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 10));
			Vector3 farPoint1 = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 10));
			Vector3 farPoint2 = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 10));
			Vector3 farPoint3 = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 10));
			Vector3 size = new Vector3();
			size.x = farPoint1.x - farPoint0.x;
			size.y = farPoint3.y - farPoint0.y;

		}

		public Vector3 LockToBounds(Vector3 desiredPosition)
		{
			if (areaBounds.bounds.Contains(desiredPosition))
				return desiredPosition;

			var bounds = areaBounds.bounds;
			
			if (desiredPosition.x > bounds.max.x)
				desiredPosition.x = bounds.max.x;
			else if (desiredPosition.x < bounds.min.x)
				desiredPosition.x = bounds.min.x;
			if (desiredPosition.y > bounds.max.y)
				desiredPosition.y = bounds.max.y;
			else if (desiredPosition.y < bounds.min.y)
				desiredPosition.y = bounds.min.y;

			return desiredPosition;
		}
	}
}