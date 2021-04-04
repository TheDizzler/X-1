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
			Broken = 0,
			Up = 1, Down = 2,
			Left = 3, Right = 4,
		}


		public enum ElevatorPhase
		{
			DoorsOpen,
			DoorsClosing,
			WaitingForInstruction,
			ElevatorMoving,
			DoorsOpening,
		}

		public ElevatorPhase phase;

		public Elevator[] connected = new Elevator[4];
		public List<Vector3Int> upShaft = new List<Vector3Int>();
		public List<Vector3Int> downShaft = new List<Vector3Int>();
		public List<Vector3Int> rightShaft = new List<Vector3Int>();
		public List<Vector3Int> leftShaft = new List<Vector3Int>();

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


		public bool IsConnectedTo(Elevator elevator)
		{
			for (int i = 0; i < connected.Length; ++i)
				if (connected[i] == elevator)
					return true;
			return false;
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

		public bool HasShaftInDirection(Directions direction)
		{
			switch (direction)
			{
				case Directions.Up:
					return upShaft != null && upShaft.Count > 0;
				case Directions.Down:
					return downShaft != null && downShaft.Count > 0;
				case Directions.Left:
					return leftShaft != null && leftShaft.Count > 0;
				case Directions.Right:
					return rightShaft != null && rightShaft.Count > 0;
			}

			return false;
		}

		public List<Vector3Int> GetShaftInDirection(Directions direction)
		{
			switch (direction)
			{
				case Directions.Up:
					return upShaft;
				case Directions.Down:
					return downShaft;
				case Directions.Left:
					return leftShaft;
				case Directions.Right:
					return rightShaft;
				default:
					return null;
			}
		}

		public void ClearShaft(Directions direction)
		{
			switch (direction)
			{
				case Directions.Up:
					upShaft = null;
					break;
				case Directions.Down:
					downShaft = null;
					break;
				case Directions.Left:
					leftShaft = null;
					break;
				case Directions.Right:
					rightShaft = null;
					break;
			}
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
	}
}