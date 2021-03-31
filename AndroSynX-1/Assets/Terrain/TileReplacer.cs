﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AtomosZ.AndroSyn.Tiles
{
	/// <summary>
	/// Place on gameobject that contains tiles to be replaced with objects.
	/// </summary>
	[RequireComponent(typeof(Tilemap))]
	public class TileReplacer : MonoBehaviour
	{
		public List<TileReplace> replaceTiles;


		void OnEnable()
		{
			Tilemap tilemap = GetComponent<Tilemap>();

			// go through every tile in tilemap and check if it is to be replaced....ugh!
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
						Instantiate(replacer.prefab, tilePosition + replacer.spawnOffset,
							Quaternion.identity);
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
		public Vector3 spawnOffset;
		public List<TileBase> connected;
	}
}