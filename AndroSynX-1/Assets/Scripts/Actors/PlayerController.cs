using AtomosZ.AndroSyn.InputProcessing;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class PlayerController : IActorController
	{
		public VirtualInput input;
		private Actor actor;

		public void OnActorControl(Actor actr)
		{
			actor = actr;
		}

		public void UpdateCommands()
		{

		}

		public void FixedUpdateCommands()
		{
			input.InputFixedUpdate();
			actor.inputVelocity = input.GetLeftAnalogue();
		}
	}
}