using AtomosZ.AndroSyn.Weapons;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class ShootingState : MonoBehaviour, IActionState
	{
		public ActionStateType actionStateType
		{
			get { return ActionStateType.SHOOT; }
			set { throw new System.NotImplementedException(); }
		}

		public GameObject bulletPrefab;
		public Transform bulletSpawnPoint;

		public float MinTimeBetweenShots = .3f;
		private float LowerWeaponTime = 2.5f;

		private Actor actor;
		private float timeSinceShoot;
		private bool spawnBullet;
		private bool firstShot;

		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		public void EnterState(ActionStateType previousState)
		{
			actor.SetAnimator(Actor.IsShootingHash, true);
			spawnBullet = true;
			firstShot = true;
			timeSinceShoot = 0f;
		}

		public ActionStateType ExitState(ActionStateType nextState)
		{
			actor.SetAnimator(Actor.IsShootingHash, false);
			return actionStateType;
		}

		void Update()
		{
			if (firstShot)
			{ // the first shot will come out of the gun in resting position if there is no delay
				firstShot = false;
				return;
			}

			if (spawnBullet)
			{
				spawnBullet = false;
				Bullet newBullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
				newBullet.Fire(bulletSpawnPoint.position, actor.actorPhysics.Facing());
			}
		}

		public ActionStateType UpdateState()
		{
			timeSinceShoot += Time.deltaTime;
			if (timeSinceShoot > MinTimeBetweenShots)
			{
				if (actor.commandList[CommandType.Shoot])
				{
					timeSinceShoot = 0;
					actor.animator.Play(Actor.ShootHash, 2, 0);
					spawnBullet = true;
				}
				else if (timeSinceShoot > LowerWeaponTime)
					return ActionStateType.AWAITING_ACTION;
			}

			return ActionStateType.NONE;
		}
	}
}