using UnityEngine;

namespace AtomosZ.AndroSyn.GameSystem
{
	public class CameraController : MonoBehaviour
	{
		public Vector3 followLead;
		public Transform followTarget;

		[SerializeField] private Camera mainCamera = null;
		[Tooltip("Approximate time to reach target. Smaller Value is faster.")]
		[SerializeField] private float smoothTime = 1.0f;

		private CameraLock cameraLock;
		private Vector3 velocity;
		private Color viewportColor;
		private float cameraZ;


		public void Awake()
		{
			cameraLock = GetComponent<CameraLock>();
			cameraZ = mainCamera.transform.localPosition.z;

		}

		public void OnDrawGizmos()
		{
			Vector3 point0 = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
			Vector3 point1 = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane));
			Vector3 point2 = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
			Vector3 point3 = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane));

			Vector3 farPoint0 = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.farClipPlane));
			Vector3 farPoint1 = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.farClipPlane));
			Vector3 farPoint2 = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.farClipPlane));
			Vector3 farPoint3 = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.farClipPlane));

			Color viewportColor = Color.yellow;
			viewportColor.a = .5f;
			Gizmos.color = viewportColor;
			Gizmos.DrawLine(point0, farPoint0);
			Gizmos.DrawLine(point1, farPoint1);
			Gizmos.DrawLine(point2, farPoint2);
			Gizmos.DrawLine(point3, farPoint3);
			Gizmos.DrawLine(farPoint0, farPoint1);
			Gizmos.DrawLine(farPoint1, farPoint2);
			Gizmos.DrawLine(farPoint2, farPoint3);
			Gizmos.DrawLine(farPoint3, farPoint0);
		}

		public void FixedUpdate()
		{
			Vector2 trackingPoint = followTarget.localPosition + followTarget.right + followLead;
			Vector3 camPos = Vector3.SmoothDamp(transform.localPosition, trackingPoint, ref velocity, smoothTime);
			camPos.z = cameraZ;
			camPos = cameraLock.CheckIfInBounds(camPos);
			mainCamera.transform.localPosition = camPos;
			//mainCamera.transform.LookAt(followTarget, followTarget.up);
		}
	}
}