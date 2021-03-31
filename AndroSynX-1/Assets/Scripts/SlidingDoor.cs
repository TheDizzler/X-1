using UnityEngine;

namespace AtomosZ.AndroSyn.Gadgets
{
	public class SlidingDoor : MonoBehaviour
	{
		[Tooltip("Local Position")]
		public Vector2 openPosition;
		[Tooltip("Local Position")]
		public Vector2 closePosition;
		public float speed = 1;


		private enum SlidingPhase
		{
			Closed,
			Opening,
			Opened,
			Closing,
		}

		[SerializeField]
		private SlidingPhase phase = SlidingPhase.Closed;
		private float t = 0;


		void Update()
		{
			switch (phase)
			{
				case SlidingPhase.Closed: // go to sleep
					enabled = false;
					t = 0;
					break;
				case SlidingPhase.Opening:
					t += speed * Time.deltaTime;
					if (t >= 1)
					{
						t = 1;
						phase = SlidingPhase.Opened;
					}

					transform.localPosition = Vector2.Lerp(
						closePosition, openPosition, t);
					break;
				case SlidingPhase.Opened:
					// nothing to do?
					break;
				case SlidingPhase.Closing:
					t -= speed * Time.deltaTime;
					if (t <= 0)
					{
						t = 0;
						phase = SlidingPhase.Closed;
					}

					transform.localPosition = Vector2.Lerp(
						closePosition, openPosition, t);
					break;
			}
		}


		public void Open()
		{
			enabled = true;
			phase = SlidingPhase.Opening;
		}

		public void Close()
		{
			phase = SlidingPhase.Closing;
		}

		public bool IsDoorOpen()
		{
			return phase == SlidingPhase.Opened;
		}

		public bool IsDoorClosed()
		{
			return phase == SlidingPhase.Closed;
		}
	}
}