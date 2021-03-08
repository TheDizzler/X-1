using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AtomosZ.AndroSyn.Tiles
{
	[RequireComponent(typeof(Tilemap))]
	public class TileReplacer : MonoBehaviour
	{
		public List<TileReplace> replaceTiles;


		void OnEnable()
		{
			Tilemap tilemap = GetComponent<Tilemap>();


			for (int i = tilemap.cellBounds.xMin; i < tilemap.cellBounds.xMax; ++i)
			{
				for (int j = tilemap.cellBounds.yMin; j < tilemap.cellBounds.yMax; ++j)
				{
					Vector3Int coordinate = new Vector3Int(i, j, 0);
					TileBase tile = tilemap.GetTile(coordinate);
					if (tile == null)
						continue;
					foreach (var replacer in replaceTiles)
					{
						if (replacer.tile != tile)
							continue;
						Vector3 tilePosition = tilemap.GetCellCenterWorld(coordinate);
						Instantiate(replacer.prefab, tilePosition, Quaternion.identity);

						Debug.Log(string.Format("Position of tile {4}: [{0}, {1}] = ({2}, {3})",
							coordinate.x, coordinate.y, tilePosition.x, tilePosition.y, tile.name));
						break;
					}
				}
			}

			foreach (var replacer in replaceTiles)
			{
				tilemap.SwapTile(replacer.tile, null);
				foreach (var other in replacer.connected)
					tilemap.SwapTile(other, null);
			}
		}
	}

	[System.Serializable]
	public class TileReplace
	{
		public TileBase tile;
		public GameObject prefab;
		public List<TileBase> connected;
	}
}