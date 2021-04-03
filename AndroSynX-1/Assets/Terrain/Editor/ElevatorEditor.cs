using System.Collections.Generic;
using AtomosZ.AndroSyn.Gadgets;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AtomosZ.AndroSyn.Editors
{
	[CustomEditor(typeof(Elevator))]
	public class ElevatorEditor : Editor
	{
		private readonly string tilePath = "Assets/Terrain/Palettes/Tiles/";
		private readonly Vector3Int right = new Vector3Int(1, 0, 0);
		private readonly Vector3Int left = new Vector3Int(-1, 0, 0);
		private readonly Vector3Int up = new Vector3Int(0, 1, 0);
		private readonly Vector3Int down = new Vector3Int(0, -1, 0);

		public Tile[] verticalShaft;
		public Tile[] verticalShaftGround;
		public Tile[] horizontalShaft;
		public Tile[] cornerShaftLU;
		public Tile[] cornerShaftLD;
		public Tile[] cornerShaftRU;
		public Tile[] cornerShaftRD;
		public Tile[] internalShaft;

		private Elevator elevator;
		private Tilemap shaftTilemap;
		private Tilemap bgTilemap;
		private GUIStyle warningText;


		void OnEnable()
		{
			elevator = (Elevator)target;
			shaftTilemap = elevator.transform.parent?.GetComponent<Tilemap>();
			bgTilemap = GameObject.FindGameObjectWithTag(Tags.BG_TILEMAP).GetComponent<Tilemap>();
			CreateStyles();

			// these get loaded every time an elevator is selected
			// maybe making these static will save them?
			verticalShaft = new Tile[]
			{
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_vert_0a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_vert_0b.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_vert_1a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_vert_1b.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_vert_2a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_vert_2b.asset", typeof(Tile)),
			};

			verticalShaftGround = new Tile[]
			{
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_ground_0a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_ground_0b.asset", typeof(Tile)),
			};

			horizontalShaft = new Tile[]
			{
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_horz_0a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_horz_0b.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_horz_1a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_horz_1b.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_horz_2a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_horz_2b.asset", typeof(Tile)),
			};

			cornerShaftLU = new Tile[]
			{
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_corner_LU_inside.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_horz_0b.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_vert_0b.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_corner_LU_outside.asset", typeof(Tile)),
			};

			cornerShaftRD = new Tile[]
			{
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_vert_0a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_corner_RD_inside.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_corner_RD_outside.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_horz_0a.asset", typeof(Tile)),
			};

			cornerShaftLD = new Tile[]
			{
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_corner_LD_inside.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_vert_0b.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_horz_0a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_corner_LD_outside.asset", typeof(Tile)),
			};

			cornerShaftRU = new Tile[]
			{
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_corner_RU_inside.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_horz_0b.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_vert_0a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevator shaft_corner_RU_outside.asset", typeof(Tile)),
			};

			internalShaft = new Tile[]
			{
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevators_internalshaft_0a.asset", typeof(Tile)),
				(Tile)AssetDatabase.LoadAssetAtPath(tilePath + "Starbase Elevators_internalshaft_0b.asset", typeof(Tile)),
			};
		}


		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			// for debugging
			EditorGUILayout.PropertyField(serializedObject.FindProperty("phase"));

			if (elevator.connected.Length != 4)
			{
				Elevator[] fix = new Elevator[4];
				for (int i = 0; i < Mathf.Min(fix.Length, elevator.connected.Length); ++i)
					fix[i] = elevator.connected[i];
				elevator.connected = fix;
			}

			if (shaftTilemap == null)
			{
				if (warningText == null)
					CreateStyles();
				EditorGUILayout.LabelField("Nest in a tilemap to connect elevators", warningText);
				return;
			}

			EditorGUILayout.BeginFoldoutHeaderGroup(true, "Connected Shafts");
			EditorGUI.indentLevel += 1;
			for (int i = 0; i < elevator.connected.Length; ++i)
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel(((Elevator.Directions)i).ToString());

					elevator.connected[i] = (Elevator)
						EditorGUILayout.ObjectField(elevator.connected[i], typeof(Elevator), true);
					if (elevator.connected[i] != null && GUILayout.Button("Connect"))
					{
						ConnectElevatorDoors(elevator.connected[i], (Elevator.Directions)i);
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUI.indentLevel -= 1;
			EditorGUILayout.EndFoldoutHeaderGroup();
		}

		/// <summary>
		/// We will ALWAYS connect from bottom to top.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="direction"></param>
		private void ConnectElevatorDoors(Elevator other, Elevator.Directions direction)
		{
			//if (!other.IsConnectedTo(elevator))
			//{
			//	Debug.LogError("Other elevator is not connected to this one");
			//	return;
			//}
			List<Vector3Int> oldShaft = null;
			switch (direction)
			{
				case Elevator.Directions.Up:
					oldShaft = elevator.upShaft;
					break;
				case Elevator.Directions.Down:
					oldShaft = elevator.downShaft;
					break;
				case Elevator.Directions.Left:
					oldShaft = elevator.leftShaft;
					break;
				case Elevator.Directions.Right:
					oldShaft = elevator.rightShaft;
					break;
			}

			if (oldShaft != null)
			{
				// remove old shaft
				foreach (var tile in oldShaft)
				{
					shaftTilemap.SetTile(tile, null);
					bgTilemap.SetTile(tile, null);
				}

				oldShaft = null;
			}

			// get tilemap position of elevator entrances
			Vector3Int startPos = shaftTilemap.WorldToCell(elevator.transform.position);
			Vector3Int endPos = shaftTilemap.WorldToCell(other.transform.position);
			List<Vector3Int> shaft = new List<Vector3Int>();

			Vector3Int diff = endPos - startPos;
			if (Mathf.Abs(diff.y) >= Mathf.Abs(diff.x))
			{ // vertical
				Debug.Log("isVertical");
				bool hasBreakPos = false;
				bool movingVertically = true;
				Vector3Int pairOffest = right;
				Vector3Int constructDirection = up;
				Vector3Int horizontalDirection = right;
				Tile[] currentShaft = verticalShaft;
				int nextBreakPos = 0;
				Vector3Int[] midpoints = new Vector3Int[2];
				if (startPos.x != endPos.x)
				{
					hasBreakPos = true;
					int vertMidpoint = startPos.y + (int)(.5f * diff.y);
					midpoints[0] = new Vector3Int(startPos.x, vertMidpoint, 0) + left;
					midpoints[1] = new Vector3Int(endPos.x, vertMidpoint, 0) + up;

					if (startPos.x < endPos.x)
					{
						midpoints[1] += left;
					}

					horizontalDirection = startPos.x < endPos.x ? right : left;
				}

				Vector3Int currentPos = startPos + left;
				endPos += down + left;

				int nextTile = 0;
				while (currentPos != endPos)
				{
					shaftTilemap.SetTile(currentPos, currentShaft[nextTile++]);
					shaftTilemap.SetTile(currentPos + pairOffest, currentShaft[nextTile++]);

					shaft.Add(currentPos);
					shaft.Add(currentPos + pairOffest);

					bgTilemap.SetTile(currentPos, internalShaft[0]);
					bgTilemap.SetTile(currentPos + pairOffest, internalShaft[1]);
					if (nextTile >= currentShaft.Length)
						nextTile = 0;
					currentPos += constructDirection;
					if (hasBreakPos && currentPos == midpoints[nextBreakPos])
					{
						var lastDirection = constructDirection;
						movingVertically = !movingVertically;
						currentShaft = movingVertically ? verticalShaft : horizontalShaft;
						pairOffest = movingVertically ? right : down;
						constructDirection = movingVertically ? up : horizontalDirection;
						nextTile = 0;
						if (++nextBreakPos == 2)
							hasBreakPos = false;

						if (constructDirection == right)
						{
							shaftTilemap.SetTile(currentPos, cornerShaftRD[0]);
							shaftTilemap.SetTile(currentPos + right, cornerShaftRD[1]);
							shaft.Add(currentPos);
							shaft.Add(currentPos + right);
							currentPos += up;
							shaftTilemap.SetTile(currentPos, cornerShaftRD[2]);
							shaftTilemap.SetTile(currentPos + right, cornerShaftRD[3]);
							shaft.Add(currentPos);
							shaft.Add(currentPos + right);
							currentPos += right;
							currentPos += right;
							continue;
						}

						if (constructDirection == left)
						{
							shaftTilemap.SetTile(currentPos, cornerShaftLD[0]);
							shaftTilemap.SetTile(currentPos + right, cornerShaftLD[1]);
							shaft.Add(currentPos);
							shaft.Add(currentPos + right);
							currentPos += up;
							shaftTilemap.SetTile(currentPos, cornerShaftLD[2]);
							shaftTilemap.SetTile(currentPos + right, cornerShaftLD[3]);
							shaft.Add(currentPos);
							shaft.Add(currentPos + right);
							currentPos += left;
							continue;
						}

						if (constructDirection != up)
							Debug.LogError("We dun fq'd up, boi!");
						//{
						if (lastDirection == right)
						{
							shaftTilemap.SetTile(currentPos, cornerShaftLU[0]);
							shaftTilemap.SetTile(currentPos + down, cornerShaftLU[1]);
							shaft.Add(currentPos);
							shaft.Add(currentPos + down);
							currentPos += right;
							shaftTilemap.SetTile(currentPos, cornerShaftLU[2]);
							shaftTilemap.SetTile(currentPos + down, cornerShaftLU[3]);
							shaft.Add(currentPos);
							shaft.Add(currentPos + down);
							currentPos += up;
							currentPos += left;
							continue;
						}

						shaftTilemap.SetTile(currentPos, cornerShaftRU[0]);
						shaftTilemap.SetTile(currentPos + down, cornerShaftRU[1]);
						shaft.Add(currentPos);
						shaft.Add(currentPos + down);
						currentPos += left;
						shaftTilemap.SetTile(currentPos, cornerShaftRU[2]);
						shaftTilemap.SetTile(currentPos + down, cornerShaftRU[3]);
						shaft.Add(currentPos);
						shaft.Add(currentPos + down);
						currentPos += up;
						//continue;
						//}
					}

					if (Mathf.Abs(currentPos.x) > 50 || Mathf.Abs(currentPos.y) > 50)
					{
						Debug.Log("dun fk'd up");
						break;
					}
				}

				currentPos += down;
				if (nextTile == 0)
				{ // make sure we don't have any cut off windows
					currentPos += down;
					shaftTilemap.SetTile(currentPos, verticalShaft[0]);
					shaftTilemap.SetTile(currentPos + right, verticalShaft[1]);
					shaft.Add(currentPos);
					shaft.Add(currentPos + right);
					currentPos += up;
				}

				shaftTilemap.SetTile(currentPos, verticalShaftGround[0]);
				shaftTilemap.SetTile(currentPos + right, verticalShaftGround[1]);
				shaft.Add(currentPos);
				shaft.Add(currentPos + right);

				switch (direction)
				{
					case Elevator.Directions.Up:
						elevator.upShaft = shaft;
						break;
					case Elevator.Directions.Down:
						elevator.downShaft = shaft;
						break;
					case Elevator.Directions.Left:
						elevator.leftShaft = shaft;
						break;
					case Elevator.Directions.Right:
						elevator.rightShaft = shaft;
						break;
				}
			}
			else
			{
				Debug.Log("isHorizontal");
				int horzMidpoint = startPos.x + (int)(.5f * diff.x);
				//midpoints[0] = new Vector3Int(horzMidpoint, startPos.y, 0);
				//midpoints[1] = new Vector3Int(horzMidpoint, endPos.y, 0);
			}



		}


		private void CreateStyles()
		{
			try
			{
				warningText = new GUIStyle(EditorStyles.label);
				warningText.normal.textColor = Color.red;
			}
			catch (System.Exception) { }
		}
	}
}