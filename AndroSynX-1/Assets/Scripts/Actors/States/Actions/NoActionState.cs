namespace AtomosZ.AndroSyn.Actors.State
{
	public class NoActionState : IActionState
	{
		public ActionStateType actionStateType
		{
			get { return ActionStateType.AWAITING_ACTION; }
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

		public ActionStateType UpdateState()
		{
			if (actor.commandList[CommandType.Shoot])
				return ActionStateType.SHOOT;

			return ActionStateType.NONE;
		}


	}
}