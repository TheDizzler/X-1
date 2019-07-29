using AtomosZ.AndroSyn.Actors.State;
using TMPro;
using UnityEngine;

namespace AtomosZ.AndroSyn.UI
{
	public class EnergyGauge : MonoBehaviour
	{
		private TextMeshProUGUI energyGauge;
		private JetpackState jetpack;
		private int last;


		public void Start()
		{
			energyGauge = GetComponentsInChildren<TextMeshProUGUI>()[1];
			jetpack = GameObject.FindGameObjectWithTag(Tags.PLAYER).GetComponentInChildren<JetpackState>();
		}

		public void Update()
		{
			int current = Mathf.CeilToInt(jetpack.currentEnergy);
			if (last != current)
			{
				energyGauge.SetText(current.ToString());
				last = current;
			}
		}
	}
}