using System.Collections.Generic;
using AtomosZ.AndroSyn.Actors;
using AtomosZ.AndroSyn.Actors.State;
using UnityEngine;

namespace AtomosZ.AndroSyn.Gadgets
{
	public class Elevator : MonoBehaviour
	{
		[System.Flags]
		public enum Directions
		{
			Broken = 0,
			Up = 1 << 0, Down = 1 << 1,
			Left = 1 << 2, Right = 1 << 3, // maybe?
		}


		public enum ElevatorPhase
		{
			DoorsOpen,
			DoorsClosing,
			WaitingForInstruction,
			ElevatorMoving,
			DoorsOpening,
		}

		public Directions directions;
		public ElevatorPhase phase;

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
						if (directions.HasFlag(Directions.Down))
						{
							Debug.Log("can go down");
						}
						else
							Debug.Log("can't go down");
					}
					else if (commandList[CommandType.Jetpack])
					{
						if (directions.HasFlag(Directions.Up))
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
	}
}