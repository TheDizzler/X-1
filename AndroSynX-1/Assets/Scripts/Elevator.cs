using System.Collections.Generic;
using AtomosZ.AndroSyn.Actors;
using AtomosZ.AndroSyn.Actors.State;
using UnityEngine;

namespace AtomosZ.AndroSyn.Gadgets
{
	public class Elevator : MonoBehaviour
	{
		public enum Directions
		{
			Broken = -1,
			Up = 0, Down = 1,
			Left = 2, Right = 3,
		}


		public enum ElevatorPhase
		{
			DoorsOpen,
			DoorsClosing,
			WaitingForInstruction,
			ElevatorMoving,
			DoorsOpening,
		}

		[System.Serializable]
		public class Shaft
		{
			public List<Vector3Int> tiles = new List<Vector3Int>();
			public List<Vector3> waypoints = new List<Vector3>();
			public Elevator[] elevators = new Elevator[2];

			public Shaft(Elevator start, Elevator end)
			{
				elevators[0] = start;
				elevators[1] = end;
			}

			public void DestroyConnection()
			{
				if (elevators == null)
					return; // goddamn it unity, why can't you let me have null serialized objects?
				foreach (var elevator in elevators)
					elevator.RemoveShaft(this);
			}
		}

		private void RemoveShaft(Shaft remove)
		{
			for (int i = 0; i < connectingShafts.Length; ++i)
			{
				if (connectingShafts[i] == remove)
				{
					connectingShafts[i] = null;
					connected[i] = null;
					return;
				}
			}
		}

		public Transform carriage;
		public float carriageSpeed = 10;
		public ElevatorPhase phase;
		/// <summary>
		/// Use for creating shafts, not for use in-game.
		/// </summary>
		public Elevator[] connected = new Elevator[4];
		public Shaft[] connectingShafts = new Shaft[4];

		[SerializeField] private SlidingDoor rightDoor = null;
		[SerializeField] private SlidingDoor leftDoor = null;
		private float timeDoorsOpen;
		private bool cancelPressed;
		private bool exitingElevator;
		private Shaft currentShaft;
		private int currentWaypoint;


		void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag(Tags.PLAYER))
			{
				OpenDoors();
				collision.GetComponent<Actor>().NearElevator(this);
			}
		}

		void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.CompareTag(Tags.PLAYER))
			{
				ShutDoors();
				collision.GetComponent<Actor>().NearElevator(null);
			}
		}


		public void Entered(Actor actor)
		{
			timeDoorsOpen = 0f;
			cancelPressed = false;
			exitingElevator = false;
		}

		public ActionStateType PassengerUpdate(Dictionary<CommandType, bool> commandList)
		{
			if (exitingElevator)
				return ActionStateType.AWAITING_ACTION;
			if (!cancelPressed)
				cancelPressed = commandList[CommandType.Cancel];
			return ActionStateType.NONE;
		}

		public MovementStateType PassengerFixedUpdate(Dictionary<CommandType, bool> commandList)
		{
			switch (phase)
			{
				case ElevatorPhase.DoorsOpen:
					if (commandList[CommandType.Kneel] || cancelPressed)
					{
						exitingElevator = true;
						return MovementStateType.STANDING;
					}

					if (commandList[CommandType.MoveLeft]
						|| commandList[CommandType.MoveRight])
					{
						exitingElevator = true;
						return MovementStateType.STANDING;
					}

					timeDoorsOpen += Time.deltaTime;
					if (timeDoorsOpen > 3.5f || commandList[CommandType.Jetpack])
					{
						ShutDoors();
						timeDoorsOpen = 0f;
					}

					break;

				case ElevatorPhase.DoorsClosing:
					if (IsDoorClosed())
					{
						phase = ElevatorPhase.WaitingForInstruction;
					}
					else
					{
						if (commandList[CommandType.Kneel] || cancelPressed)
						{
							OpenDoors();
						}
					}

					break;

				case ElevatorPhase.DoorsOpening:
					if (IsDoorOpen())
					{
						phase = ElevatorPhase.DoorsOpen;
					}
					break;

				case ElevatorPhase.WaitingForInstruction:
					if (cancelPressed)
					{
						OpenDoors();
					}
					else if (commandList[CommandType.Kneel])
					{
						GetShaftAndGo(Directions.Down);
					}
					else if (commandList[CommandType.Jetpack])
					{
						GetShaftAndGo(Directions.Up);
					}
					else if (commandList[CommandType.MoveLeft])
					{
						GetShaftAndGo(Directions.Left);
					}
					else if (commandList[CommandType.MoveRight])
						GetShaftAndGo(Directions.Right);

					break;

				case ElevatorPhase.ElevatorMoving:
					carriage.position = Vector2.MoveTowards(
						carriage.position, currentShaft.waypoints[currentWaypoint], carriageSpeed * Time.deltaTime);
					if (Mathf.Approximately(carriage.position.x, currentShaft.waypoints[currentWaypoint].x)
						&& Mathf.Approximately(carriage.position.y, currentShaft.waypoints[currentWaypoint].y))
					{
						if (++currentWaypoint >= currentShaft.waypoints.Count)
						{   // arrived
							phase = ElevatorPhase.DoorsOpening;
						}
					}
					break;
			}

			cancelPressed = false;
			return MovementStateType.NONE;
		}

		Color[] rgb = new Color[4] {
			new Color(0, 1, 0),
			new Color(1, 0, 0),
			new Color(0, 1, 1),
			new Color(1, 0, 1),
		};
		private void OnDrawGizmosSelected()
		{

			for (int i = 0; i < 4; ++i)
			{
				Gizmos.color = rgb[i];
				if (connectingShafts[i] != null && connectingShafts[i].waypoints != null)
					foreach (var point in connectingShafts[i].waypoints)
						Gizmos.DrawSphere(point, .1f);
			}
		}

		
		private void GetShaftAndGo(Directions direction)
		{
			currentShaft = connectingShafts[(int)direction];

			if (currentShaft == null || currentShaft.waypoints.Count == 0)
				Debug.Log("No shaft detected in direction " + direction);
			phase = ElevatorPhase.ElevatorMoving;
			currentWaypoint = 0;
		}

		private void OpenDoors()
		{
			phase = ElevatorPhase.DoorsOpening;
			rightDoor.Open();
			leftDoor.Open();
		}

		private void ShutDoors()
		{
			phase = ElevatorPhase.DoorsClosing;
			rightDoor.Close();
			leftDoor.Close();
		}

		public bool IsDoorOpen()
		{
			return rightDoor.IsDoorOpen();
		}

		public bool IsDoorClosed()
		{
			return rightDoor.IsDoorClosed();
		}

#if UNITY_EDITOR
		public bool HasShaftInDirection(Directions direction)
		{
			return connectingShafts[(int)direction] != null;
		}

		public Shaft GetShaftInDirection(Directions direction)
		{
			return connectingShafts[(int)direction];
		}

		private void ClearShaft(Directions direction)
		{
			connectingShafts[(int)direction] = null;
		}

		public Elevator GetElevatorConnectedTo(Directions direction)
		{
			return connected[(int)direction];
		}

		public static Directions GetOpposite(Directions direction)
		{
			switch (direction)
			{
				case Directions.Down:
					return Directions.Up;
				case Directions.Left:
					return Directions.Right;
				case Directions.Right:
					return Directions.Left;
				case Directions.Up:
					return Directions.Down;
				case Directions.Broken:
				default:
					throw new System.Exception("Cannot connect broken elevator");
			}
		}
#endif
	}
}