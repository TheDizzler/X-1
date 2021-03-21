using AtomosZ.AndroSyn.Actors;
using AtomosZ.AndroSyn.GamePhysics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtomosZ.AndroSyn.UI
{
	public class GravityRose : MonoBehaviour
	{
		private const float ROSE_MEDIAN = 4.5f;

		private GameObject areaGravityRose;
		private GameObject playerGravityRose;
		private TextMeshProUGUI areaMagnitudeTMP;
		private TextMeshProUGUI playerMagnitudeTMP;
		private Vector3 lastAreaGravity;
		private Vector3 lastPlayerGravity;
		private IActorPhysics playerPhysics;


		public void Start()
		{
			var imgs = GetComponentsInChildren<Image>();
			foreach (Image image in imgs)
			{
				if (image.name.Contains("Area"))
					areaGravityRose = image.gameObject;
				else if (image.name.Contains("Player"))
					playerGravityRose = image.gameObject;
				else
					Debug.LogError(image.name + " is not a valid Gravity Indicator Image");
			}

			var tmps = GetComponentsInChildren<TextMeshProUGUI>();
			foreach (TextMeshProUGUI tmp in tmps)
			{
				if (tmp.name.Contains("Area"))
					areaMagnitudeTMP = tmp;
				else if (tmp.name.Contains("Player"))
					playerMagnitudeTMP = tmp;
				else
					Debug.LogError(tmp.name + " is not a valid Gravity Indicator TMP");
			}

			playerPhysics = GameObject.FindGameObjectWithTag(Tags.PLAYER).GetComponent<IActorPhysics>();
		}


		public void Update()
		{
			Vector3 currentAreaGravity = AreaPhysics.gravity;
			if (currentAreaGravity != lastAreaGravity)
			{
				areaGravityRose.transform.rotation =
					Quaternion.FromToRotation(Vector3.up, currentAreaGravity.normalized);
				float scale = currentAreaGravity.magnitude / ROSE_MEDIAN;
				areaGravityRose.transform.localScale = new Vector3(scale, scale, 1);
				areaMagnitudeTMP.text = "g = " + currentAreaGravity.magnitude;
				lastAreaGravity = currentAreaGravity;
			}
#if UNITY_EDITOR
			if (Application.isPlaying && playerPhysics == null)
				return; // prevents error spam when stopping play
#endif
			Vector3 currentPlayerGravity = playerPhysics.GetTotalAffectingGravity();
			if (currentPlayerGravity != lastPlayerGravity)
			{
				playerGravityRose.transform.rotation =
					Quaternion.FromToRotation(Vector3.up, currentPlayerGravity.normalized);
				float scale = currentPlayerGravity.magnitude / ROSE_MEDIAN;
				playerGravityRose.transform.localScale = new Vector3(scale, scale, 1);
				playerMagnitudeTMP.text = "g = " + currentPlayerGravity.magnitude;
				lastPlayerGravity = currentPlayerGravity;
			}
		}
	}
}