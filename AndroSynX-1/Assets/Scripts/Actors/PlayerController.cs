﻿using AtomosZ.AndroSyn.InputProcessing;

namespace AtomosZ.AndroSyn.Actors
{
	public class PlayerController : IActorController
	{
		public VirtualInput input;
		private Actor actor;

		public void OnActorControl(Actor actr)
		{
			actor = actr;
			actor.commandList.Add(CommandType.MoveLeft, false);
			actor.commandList.Add(CommandType.MoveRight, false);
			actor.commandList.Add(CommandType.Shoot, false);
			actor.commandList.Add(CommandType.Duck, false);
			actor.commandList.Add(CommandType.Jetpack, false);
		}

		public void UpdateCommands()
		{
			actor.commandList[CommandType.Shoot] = input.IsInputDown(VirtualInputCommand.Attack);
		}

		public void FixedUpdateCommands()
		{
			input.InputFixedUpdate();
			actor.inputVelocity = input.GetLeftAnalogue();
			actor.commandList[CommandType.MoveLeft] = actor.inputVelocity.x < 0;
			actor.commandList[CommandType.MoveRight] = actor.inputVelocity.x > 0;
			actor.commandList[CommandType.Duck] = actor.inputVelocity.y < 0;
			actor.commandList[CommandType.Jetpack] = actor.inputVelocity.y > 0;
		}
	}
}