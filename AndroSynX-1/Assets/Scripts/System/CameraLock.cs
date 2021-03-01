using UnityEngine;

namespace AtomosZ.AndroSyn.GameSystem
{
	/// <summary>
	/// FIX THIS. I stupidly wanted to make camera clamping to non-rectangular shapes.
	/// </summary>
	public class CameraLock : MonoBehaviour
	{
		private Camera mainCamera;
		private EdgeCollider2D areaBounds;
		private BoxCollider2D cameraCollider;

		//private float raycastDepth;
		//private RaycastHit ray;
		//private Vector3 topLeft, topRight, bottomRight, bottomLeft;
		//private Vector3 bottomLeftRayDirect;
		private int layerMask;
		private Vector3 hitpoint;
		private bool foundHitpoint;
		private bool outOfBounds;
		private Ray ray;

		void Start()
		{
			mainCamera = GetComponent<Camera>();
			areaBounds = GameObject.FindGameObjectWithTag(Tags.AREA_PHYSICS).GetComponentInChildren<EdgeCollider2D>();

			Vector3 farPoint0 = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 10));
			Vector3 farPoint1 = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 10));
			Vector3 farPoint2 = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 10));
			Vector3 farPoint3 = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 10));
			Vector3 size = new Vector3();
			size.x = farPoint1.x - farPoint0.x;
			size.y = farPoint3.y - farPoint0.y;

			cameraCollider = gameObject.AddComponent<BoxCollider2D>();
			cameraCollider.size = size;
			cameraCollider.offset = new Vector3(0, 0, 10);
			//raycastDepth = areaBounds.transform.localPosition.z - mainCamera.transform.localPosition.z;
			//layerMask = 1 << LayerMask.GetMask("CameraLockBoundary");
		}

		public Vector3 CheckIfInBounds(Vector3 desiredPosition)
		{
			Vector3 actualPosition = desiredPosition;
			Vector3 direction = transform.localPosition - desiredPosition;

			//bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
			//bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane));
			//topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
			//topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane));
			//bottomLeftRayDirect = bottomLeft -  mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.farClipPlane));

			//Vector3 diff = transform.localPosition - desiredPosition;
			//diff.z = 0;
			//Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 10)) + diff;
			//if (!areaBounds.OverlapPoint(bottomLeft))
			//{
			//	if (!outOfBounds)
			//		Debug.LogError("BottomLeft is out of bounds!");
			//	outOfBounds = true;

			//	ray = new Ray();
			//	ray.direction = direction;
			//	ray.origin = bottomLeft;
			//	if (Physics.Raycast(ray, out RaycastHit hit))
			//	{
			//		Debug.Log(hit.point);
			//		hitpoint = hit.point;
			//		foundHitpoint = true;
			//	}
			//	else
			//		foundHitpoint = false;
			//}
			//else
			//{
			//	foundHitpoint = false;
			//	outOfBounds = false;
			//}

			return actualPosition;
			//if (!Physics.Raycast(bottomLeft, bottomLeftRayDirect, raycastDepth, layerMask, QueryTriggerInteraction.Collide))
			//{
			//	Debug.Log("BottomLeft is out of bounds!");
			//}
		}

		bool once = true;
		public void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;
			if (outOfBounds)
			{
				Gizmos.DrawRay(ray);
				if (once)
					Debug.LogError("Stop");
				once = false;
			}

			if (foundHitpoint)
			{

				Gizmos.DrawSphere(hitpoint, .5f);
			}
		}


	}
}