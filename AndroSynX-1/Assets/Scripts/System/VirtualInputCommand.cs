using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtomosZ.AndroSyn.InputProcessing
{
	/// <summary>
	/// A button that a single button on a controller can be bound to
	/// Ie: Drop through is not "DropThrough" here, but "LeftAxisDown" + "Jump"
	/// </summary>
	public enum VirtualInputCommand : int
	{
		Up,
		Down,
		Left,
		Right,

		Attack,

		Invert,
		DebugHP,
		DebugPossess,
		DebugMenu, // Button to open the debug menu
	}
}
