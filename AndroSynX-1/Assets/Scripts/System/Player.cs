using System.Collections.Generic;
using AtomosZ.AndroSyn.Actors;
using AtomosZ.AndroSyn.InputProcessing;
using UnityEngine;

namespace AtomosZ.AndroSyn.Player
{
	public class Player : MonoBehaviour
	{
		private enum InputType
		{
			Keyboard,
			Controller1,
			Controller2,
		}

		public VirtualInput inputDevice;

		[SerializeField] private InputType inputType = InputType.Keyboard;

		/// <summary>
		/// List of actor prefabs to spawn at game start that the player can control.
		/// </summary>
		[SerializeField] private List<Actor> playerPrefabs;
		private List<Actor> playerActors = new List<Actor>();
		private PlayerController playerActorController;


		public void Awake()
		{
			GameObject spawnPoint = GameObject.FindGameObjectWithTag(Tags.PLAYER_SPAWN_POINT);
			inputDevice = CreateInput(inputType);
			foreach (Actor gameObject in playerPrefabs)
			{
				Actor actor = Instantiate(gameObject, spawnPoint.transform.localPosition, Quaternion.identity);
				playerActors.Add(actor);
			}

			playerActorController = new PlayerController();
			playerActorController.input = inputDevice;
			playerActors[0].SetActorController(playerActorController);

		}





		private VirtualInput CreateInput(InputType type)
		{
			switch (type)
			{
				case InputType.Controller1:
					return CreateJoystick1VC();
				case InputType.Keyboard:
				default:
					return CreateKeyboardVC();
			}
		}

		private VirtualInput CreateKeyboardVC()
		{
			var controller = new VirtualInput();
			controller.SetAxisNames("Horizontal", "Vertical");

			controller.SetBinding(VirtualInputCommand.Up, KeyCode.UpArrow);
			controller.SetBinding(VirtualInputCommand.Down, KeyCode.DownArrow);
			controller.SetBinding(VirtualInputCommand.Left, KeyCode.LeftArrow);
			controller.SetBinding(VirtualInputCommand.Right, KeyCode.RightArrow);

			controller.SetBinding(VirtualInputCommand.Attack, KeyCode.Z);

			return controller;
		}

		private VirtualInput CreateJoystick1VC()
		{
			var controller = new VirtualInput();
			controller.SetAxisNames("Horizontal", "Vertical");

			// No DPAD

			//controller.SetBinding(VirtualControllerCommand.Jump, KeyCode.Joystick1Button0);
			//controller.SetBinding(VirtualControllerCommand.Dash, KeyCode.Joystick1Button4);
			//controller.SetBinding(VirtualControllerCommand.Sprint, KeyCode.Joystick1Button5);

			controller.SetBinding(VirtualInputCommand.Attack, KeyCode.Joystick1Button2);

			return controller;
		}
	}
}