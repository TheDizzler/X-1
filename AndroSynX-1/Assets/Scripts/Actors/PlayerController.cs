using AtomosZ.AndroSyn.InputProcessing;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actor
{
	public class PlayerController : MonoBehaviour
	{
		private enum ControllerType
		{
			Keyboard,
			Controller1,
			Controller2,
		}

		public VirtualController controller;

		[SerializeField] ControllerType controllerType = ControllerType.Keyboard;
		[SerializeField] private float jetpackPower = 3.33f;
		[SerializeField] private float jetpackThreshold = .1f;
		[SerializeField] private float airHorizontalSpeed = 3.33f;
		[SerializeField] private float groundHorizontalSpeed = 2.4f;
		private float currentHorizontalSpeed;
		private bool jetpackOn;
		private ActorPhysics actorPhysics;


		public void Awake()
		{
			controller = CreateController(controllerType);
			actorPhysics = GetComponent<ActorPhysics>();
			currentHorizontalSpeed = airHorizontalSpeed;
		}

		public void Update()
		{
			// parse input
			controller.InputUpdate();
		}

		public void FixedUpdate()
		{
			actorPhysics.UpdateInternalStateFromPhysicsResult();
			controller.InputFixedUpdate();

			currentHorizontalSpeed = actorPhysics.isGrounded ? groundHorizontalSpeed : airHorizontalSpeed;
			Vector2 inputVelocity = controller.GetLeftAnalogue();
			jetpackOn = inputVelocity.y >= jetpackThreshold;

			inputVelocity.x *= currentHorizontalSpeed;
			if (jetpackOn)
				inputVelocity.y *= jetpackPower;
			else
				inputVelocity.y = 0;
			actorPhysics.desiredVelocity = inputVelocity;

			actorPhysics.ApplyToPhysics();
		}


		private VirtualController CreateController(ControllerType type)
		{
			switch (type)
			{
				case ControllerType.Controller1:
					return CreateJoystick1VC();
				case ControllerType.Keyboard:
				default:
					return CreateKeyboardVC();
			}
		}

		private VirtualController CreateKeyboardVC()
		{
			var controller = new VirtualController();
			controller.SetAxisNames("Horizontal", "Vertical");

			controller.SetBinding(VirtualControllerCommand.Up, KeyCode.UpArrow);
			controller.SetBinding(VirtualControllerCommand.Down, KeyCode.DownArrow);
			controller.SetBinding(VirtualControllerCommand.Left, KeyCode.LeftArrow);
			controller.SetBinding(VirtualControllerCommand.Right, KeyCode.RightArrow);

			controller.SetBinding(VirtualControllerCommand.Attack, KeyCode.Z);

			return controller;
		}

		private VirtualController CreateJoystick1VC()
		{
			var controller = new VirtualController();
			controller.SetAxisNames("Horizontal", "Vertical");

			// No DPAD

			controller.SetBinding(VirtualControllerCommand.Jump, KeyCode.Joystick1Button0);
			controller.SetBinding(VirtualControllerCommand.Dash, KeyCode.Joystick1Button4);
			controller.SetBinding(VirtualControllerCommand.Sprint, KeyCode.Joystick1Button5);

			controller.SetBinding(VirtualControllerCommand.Attack, KeyCode.Joystick1Button2);

			return controller;
		}
	}
}