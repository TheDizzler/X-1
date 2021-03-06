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
		public float MinTimeBetweenShots = .3f;
		private float LowerWeaponTime = 2.5f;

		private Actor actor;
		private float timeSinceShoot;

		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(ActionStateType previousState)
		{
			actor.animator.SetBool(Actor.IsShootingHash, true);
			timeSinceShoot = 0f;
		}

		public ActionStateType ExitState(ActionStateType nextState)
		{
			actor.animator.SetBool(Actor.IsShootingHash, false);
			return actionStateType;
		}

		void Update()
		{

		}

		public ActionStateType FixedUpdateState()
		{
			timeSinceShoot += Time.deltaTime;
			if (timeSinceShoot > MinTimeBetweenShots)
			{
				if (actor.commandList[CommandType.Shoot])
				{
					timeSinceShoot = 0;
					actor.animator.Play(Actor.ShootHash, 2, 0);
				}
				else if (timeSinceShoot > LowerWeaponTime)
					return ActionStateType.AwaitingAction;
			}

			return ActionStateType.None;
		}
	}
}