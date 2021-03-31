using UnityEngine;

namespace AtomosZ.AndroSyn.InputProcessing
{
	/// <summary>
	/// @TODO: This still uses old Unity input. Switch to new input system.
	/// </summary>
	public class VirtualInput
	{
		const float DeadZone = 0.1f;

		public int ControllerIndex { get; set; }

		private VirtualInputCommand[] allVirtualInputCommands;
		private readonly KeyCode[] commandToKeycode;
		private string axisX;
		private string axisY;

		Vector2 currentInput = Vector2.zero;

		public VirtualInput()
		{
			allVirtualInputCommands = (VirtualInputCommand[])
				System.Enum.GetValues(typeof(VirtualInputCommand));
			commandToKeycode = new KeyCode[allVirtualInputCommands.Length];
		}

		public void SetBinding(VirtualInputCommand command, KeyCode code)
		{
			commandToKeycode[(int)command] = code;
		}

		public void SetAxisNames(string x, string y)
		{
			axisX = x;
			axisY = y;
		}

		public KeyCode GetBinding(VirtualInputCommand command)
		{
			return commandToKeycode[(int)command];
		}


		/// <summary>
		/// Must be called every FixedUpdate
		/// </summary>
		public void InputFixedUpdate()
		{
			currentInput.x = Input.GetAxisRaw(axisX);
			currentInput.y = Input.GetAxisRaw(axisY);

			if (currentInput.magnitude < DeadZone)
				currentInput = Vector2.zero;

			currentInput.x = CalcHorizontalAxis();
			currentInput.y = CalcVerticalAxis();
		}

		public Vector2 GetLeftAnalogue()
		{
			return currentInput;
		}

		public Vector2 GetRightAnalogue()
		{
			return Vector2.zero;
		}

		public bool IsInput(VirtualInputCommand command)
		{
			return Input.GetKey(GetBinding(command));
		}

		/// <summary>
		/// Note: this is only valid during Update. Buttons will be missed in fixed update
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public bool IsInputDown(VirtualInputCommand command)
		{
			return Input.GetKeyDown(GetBinding(command));
		}

		/// <summary>
		/// Note: this is only valid during Update. Buttons will be missed in fixed update
		/// </summary>
		public bool IsInputUp(VirtualInputCommand command)
		{
			return Input.GetKeyUp(GetBinding(command));
		}

		public bool IsAnyKey()
		{
			foreach (var val in commandToKeycode)
			{
				if (Input.GetKey(val))
					return true;
			}

			return false;
		}

		private float CalcHorizontalAxis()
		{
			float horizontalAxis = currentInput.x;
			if (horizontalAxis == 0f)
			{
				if (IsInput(VirtualInputCommand.Left))
				{
					horizontalAxis = -1f;
				}
				else if (IsInput(VirtualInputCommand.Right))
				{
					horizontalAxis = 1f;
				}
			}

			return horizontalAxis;
		}

		private float CalcVerticalAxis()
		{
			float verticalAxis = currentInput.y;
			if (verticalAxis == 0f)
			{
				if (IsInput(VirtualInputCommand.Down))
				{
					verticalAxis = -1f;
				}
				else if (IsInput(VirtualInputCommand.Up))
				{
					verticalAxis = 1f;
				}
			}

			return verticalAxis;
		}
	}
}