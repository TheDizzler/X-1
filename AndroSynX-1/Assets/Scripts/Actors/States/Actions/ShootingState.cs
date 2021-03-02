using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class ShootingState : MonoBehaviour, IActionState
	{
		public ActionStateType actionStateType
		{
			get { return ActionStateType.Shoot; }
			set { throw new System.NotImplementedException(); }
		}

		private Actor actor;


		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(ActionStateType previousState)
		{
			actor.animator.SetBool(Actor.IsShooting, true);
		}

		public ActionStateType ExitState(ActionStateType nextState)
		{
			return actionStateType;
		}

		void Update()
		{

		}

		public ActionStateType FixedUpdateState()
		{

			return ActionStateType.None;
		}
	}
}