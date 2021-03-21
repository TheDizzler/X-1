using System;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class IKLegSettings : MonoBehaviour
	{
		public Transform body;
		public IKLegSolver[] legSolvers;
		public float raycastDistance = 3.1f;
		public float speed = 4;
		public float stepLength = .275f;
		public float kneeStepHeight = .3f;
		/// <summary>
		/// Max height that leg can climb onto.
		/// </summary>
		public float minDistanceBelowCenterStepHeight = .2f;
		public LayerMask probeMask = -1;



		void Start()
		{
			Actor actor = GetComponent<Actor>();
			legSolvers[0].SetOwner(actor, legSolvers[1], this);
			legSolvers[1].SetOwner(actor, legSolvers[0], this);
		}

		public void TryPutFeetOnGround()
		{
			foreach (var leg in legSolvers)
				leg.TryPutFootOnGround();
		}

		public float MaxStepHeight()
		{
			return body.position.y - minDistanceBelowCenterStepHeight;
		}
	}
}
