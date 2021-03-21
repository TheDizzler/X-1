using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public class SpawnPoint : MonoBehaviour
	{
		[Tooltip("The type of actor spawned at this point. Not currently used.")]
		public Actor spawnedType;


		void Start()
		{
			Destroy(gameObject, 1f);
		}
	}
}