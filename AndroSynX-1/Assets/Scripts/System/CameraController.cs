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
		private float cameraZ;
		private Bounds bounds;


		public void Awake()
		{
			cameraLock = GetComponent<CameraLock>();
			cameraZ = mainCamera.transform.localPosition.z;

			Vector3 playPoint0 = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, -mainCamera.transform.position.z));
			Vector3 playPoint1 = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, -mainCamera.transform.position.z));
			Vector3 playPoint2 = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, -mainCamera.transform.position.z));

			Vector3 pos = mainCamera.transform.localPosition;
			pos.z = 0;
			bounds = new Bounds(pos,
				new Vector3(Vector3.Distance(playPoint1, playPoint2),
							Vector3.Distance(playPoint0, playPoint1), 0));
		}

		public void OnDrawGizmos()
		{
			Vector3 point0 = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
			Vector3 point1 = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane));
			Vector3 point2 = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
			Vector3 point3 = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane));

			Vector3 playPoint0 = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, -mainCamera.transform.position.z));
			Vector3 playPoint1 = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, -mainCamera.transform.position.z));
			Vector3 playPoint2 = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, -mainCamera.transform.position.z));
			Vector3 playPoint3 = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, -mainCamera.transform.position.z));

			Vector3 farPoint0 = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.farClipPlane));
			Vector3 farPoint1 = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.farClipPlane));
			Vector3 farPoint2 = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.farClipPlane));
			Vector3 farPoint3 = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.farClipPlane));

			Color viewportColor = new Color(1, .92f, .016f, .5f);
			Gizmos.color = viewportColor;
			Gizmos.DrawLine(point0, farPoint0);
			Gizmos.DrawLine(point1, farPoint1);
			Gizmos.DrawLine(point2, farPoint2);
			Gizmos.DrawLine(point3, farPoint3);
			Gizmos.DrawLine(farPoint0, farPoint1);
			Gizmos.DrawLine(farPoint1, farPoint2);
			Gizmos.DrawLine(farPoint2, farPoint3);
			Gizmos.DrawLine(farPoint3, farPoint0);

			Color playfieldColor = new Color(0, .92f, 1, .5f);
			Gizmos.color = playfieldColor;
			Gizmos.DrawLine(playPoint0, playPoint1);
			Gizmos.DrawLine(playPoint1, playPoint2);
			Gizmos.DrawLine(playPoint2, playPoint3);
			Gizmos.DrawLine(playPoint3, playPoint0);
		}

		public void FixedUpdate()
		{
			Vector2 trackingPoint = followTarget.localPosition + followTarget.right + followLead;
			Vector3 camPos = Vector3.SmoothDamp(transform.localPosition, trackingPoint, ref velocity, smoothTime);
			camPos.z = 0;
			bounds.center = camPos;
			camPos = cameraLock.LockToBounds(bounds);
			camPos.z = cameraZ;
			mainCamera.transform.localPosition = camPos;
		}



		public void ChangeCameraUp()
		{
			mainCamera.transform.LookAt(followTarget, followTarget.up);
		}
	}
}