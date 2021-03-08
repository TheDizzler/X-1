using UnityEngine;

namespace AtomosZ.AndroSyn.Weapons
{
	public class Door : MonoBehaviour
	{
		public static readonly int IsOpenHash = Animator.StringToHash("isOpen");


		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.collider.CompareTag(Tags.BULLET))
			{
				GetComponent<Animator>().SetBool(IsOpenHash, true);
			}
		}

	}
}