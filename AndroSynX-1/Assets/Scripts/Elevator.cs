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
						if (connected[(int)Directions.Down] != null)
						{
							Debug.Log("can go down");
						}
						else
							Debug.Log("can't go down");
					}
					else if (commandList[CommandType.Jetpack])
					{
						if (connected[(int)Directions.Up] != null)
						{
							Debug.Log("can go up");
						}
						else
							Debug.Log("can't go up");
					}

					break;

				case ElevatorPhase.ElevatorMoving:
					throw new System.Exception("NYI");
			}

			cancelPressed = false;
			return MovementStateType.NONE;
		}

		private void OnDrawGizmosSelected()
		{
			if (connectingShafts[0] != null && connectingShafts[0].waypoints != null)
				foreach (var point in connectingShafts[0].waypoints)
					Gizmos.DrawSphere(point, .1f);
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

		public List<Vector3Int> RemoveConnection(Directions direction)
		{
			if (connected[(int)direction] == null)
				return null;
			var opp = GetOpposite(direction);
			List<Vector3Int> otherShaft = connected[(int)direction].GetShaftInDirection(opp);
			ClearShaft(direction);
			connected[(int)direction].ClearShaft(opp);
			connected[(int)direction].connected[(int)opp] = null;
			connected[(int)direction] = null;
			return otherShaft;
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