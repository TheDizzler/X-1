namespace AtomosZ.AndroSyn.Actors.State
{
	public class NoActionState : IActionState
	{
		public ActionStateType actionStateType
		{
			get { return ActionStateType.AwaitingAction; }
			set { throw new System.NotImplementedException(); }
		}

		private Actor actor;


		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(ActionStateType previousState)
		{
		}

		public ActionStateType ExitState(ActionStateType nextState)
		{
			return actionStateType;
		}

		public ActionStateType FixedUpdateState()
		{
			if (actor.commandList[CommandType.Shoot])
				return ActionStateType.Shoot;

			return ActionStateType.None;
		}


	}
}