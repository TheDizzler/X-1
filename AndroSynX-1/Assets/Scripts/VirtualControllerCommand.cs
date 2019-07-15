using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomosZ.AndroSyn.InputProcessing
{
	/// <summary>
	/// A button that a single button on a controller can be bound to
	/// Ie: Drop through is not "DropThrough" here, but "LeftAxisDown" + "Jump"
	/// </summary>
	public enum VirtualControllerCommand : int
	{
		Up,
		Down,
		Left,
		Right,

		Jump,
		Attack,
		Invert,
		DebugHP,
		DebugPossess,

		Dash, // A short dash
		Sprint, // A sustained sprint

		DebugMenu, // Button to open the debug menu

		Max,
	}
}
