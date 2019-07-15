using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomosZ.AndroSyn.InputProcessing
{
	public class VirtualController
	{
		const float DeadZone = 0.1f;

		public int ControllerIndex { get; set; }

		private VirtualControllerCommand[] allVirtualControllerCommands;
		private readonly KeyCode[] commandToKeycode;
		private string axisX;
		private string axisY;

		Vector2 currentInput = Vector2.zero;

		public VirtualController()
		{
			allVirtualControllerCommands = (VirtualControllerCommand[])System.Enum.GetValues(typeof(VirtualControllerCommand));
			commandToKeycode = new KeyCode[allVirtualControllerCommands.Length];
		}
		
		public void SetBinding(VirtualControllerCommand command, KeyCode code)
		{
			commandToKeycode[(int)command] = code;
		}

		public void SetAxisNames(string x, string y)
		{
			axisX = x;
			axisY = y;
		}

		public KeyCode GetBinding(VirtualControllerCommand command)
		{
			return commandToKeycode[(int)command];
		}

		public void InputUpdate()
		{
		}

		/// <summary>
		/// Must be called every FixedUpdfate
		/// </summary>
		public void InputFixedUpdate()
		{
			currentInput.x = Input.GetAxisRaw(axisX);
			currentInput.y = Input.GetAxisRaw(axisY);

			if (currentInput.magnitude < DeadZone) currentInput = Vector2.zero;

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

		public bool IsInput(VirtualControllerCommand command)
		{
			return Input.GetKey(GetBinding(command));
		}

		/// <summary>
		/// Note: this is only valid during Update. Buttons will be missed in fixed update
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public bool IsInputDown(VirtualControllerCommand command)
		{
			return Input.GetKeyDown(GetBinding(command));
		}

		/// <summary>
		/// Note: this is only valid during Update. Buttons will be missed in fixed update
		/// </summary>
		public bool IsInputUp(VirtualControllerCommand command)
		{
			return Input.GetKeyUp(GetBinding(command));
		}

		public bool IsAnyKey()
		{
			foreach (var val in commandToKeycode)
			{
				if (Input.GetKey(val)) return true;
			}

			return false;
		}

		private float CalcHorizontalAxis()
		{
			float horizontalAxis = currentInput.x;
			if (horizontalAxis == 0f)
			{
				if (IsInput(VirtualControllerCommand.Left))
				{
					horizontalAxis = -1f;
				}
				else if (IsInput(VirtualControllerCommand.Right))
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
				if (IsInput(VirtualControllerCommand.Down))
				{
					verticalAxis = -1f;
				}
				else if (IsInput(VirtualControllerCommand.Up))
				{
					verticalAxis = 1f;
				}
			}

			return verticalAxis;
		}
	}
}
