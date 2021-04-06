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
			serializedObject.Update();
			//base.OnInspectorGUI();
			// for debugging
			EditorGUILayout.PropertyField(serializedObject.FindProperty("phase"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("carriage"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("carriageSpeed"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("rightDoor"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("leftDoor"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("connectingShafts"));

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

			EditorGUILayout.BeginFoldoutHeaderGroup(true, "Connecting Elevators");
			EditorGUI.indentLevel += 1;
			for (int i = 0; i < elevator.connected.Length; ++i)
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel(((Elevator.Directions)i).ToString());

					var newElevator = (Elevator)
						EditorGUILayout.ObjectField(elevator.connected[i], typeof(Elevator), true);
					if (newElevator != null && newElevator == elevator)
						Debug.LogError("Cannot connect to self");
					else if (newElevator != elevator.connected[i])
					{
						var other = elevator.connected[i];
						var connectingShaft = elevator.GetShaftInDirection((Elevator.Directions)i);
						RemoveShaftTiles(connectingShaft);
						connectingShaft.DestroyConnection();
						elevator.connected[i] = newElevator;
						if (other != null)
						{
							EditorUtility.SetDirty(other);
							if (other.connected[(int)Elevator.GetOpposite((Elevator.Directions)i)] != null)
							{
								Debug.Log("SRSLY WTF");
							}
						}
					}

					if (elevator.connected[i] != null && GUILayout.Button("Connect"))
					{
						var other = elevator.connected[i];
						ConnectElevatorDoors(other, (Elevator.Directions)i);

						elevator.connected[i] = other;
						other.connected[(int)Elevator.GetOpposite((Elevator.Directions)i)] = elevator;
						EditorUtility.SetDirty(other);
						EditorUtility.SetDirty(elevator);
						if (elevator.connected[i] == null)
							Debug.Log("WTF");
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUI.indentLevel -= 1;
			EditorGUILayout.EndFoldoutHeaderGroup();

			serializedObject.ApplyModifiedProperties();

		}

		/// <summary>
		/// Connecting elevator contract:
		/// 1) We will ALWAYS connect from bottom to top. Left/right doesn't matter.
		/// 2) An Elevator.Direction MUST connect with the opposite Elevator.Direction on the other,
		///		i.e this Up must connect with other Down.
		/// </summary>
		/// <param name="other"></param>
		/// <param name="direction"></param>
		private void ConnectElevatorDoors(Elevator other, Elevator.Directions direction)
		{
			switch (direction)
			{
				case Elevator.Directions.Up:
					if (elevator.transform.position.y >= other.transform.position.y)
					{
						Debug.LogError("Elevator connected to \"Up\" must be" +
							" above this elevator.");
						return;
					}
					break;
				case Elevator.Directions.Down:
					if (elevator.transform.position.y <= other.transform.position.y)
					{
						Debug.LogError("Elevator connected to \"Down\" must be" +
							" below this elevator.");
						return;
					}
					break;
				case Elevator.Directions.Right:
					if (elevator.transform.position.x >= other.transform.position.x)
					{
						Debug.LogError("Elevator connected to \"Right\" must be" +
							" to the right this elevator.");
						return;
					}
					break;
				case Elevator.Directions.Left:
					if (elevator.transform.position.x <= other.transform.position.x)
					{
						Debug.LogError("Elevator connected to \"Left\" must be" +
							" to the left this elevator.");
						return;
					}
					break;
			}

			var oppositeDirection = Elevator.GetOpposite(direction);

			if (other.connected[(int)oppositeDirection] != elevator
				&& other.HasShaftInDirection(oppositeDirection))
			{
				//Debug.LogError("Cannot connect: Other elevator already has a connection " +
				//	"to a different elevator on " + oppositeDirection);
				var otherConnection = other.GetShaftInDirection(oppositeDirection);
				RemoveShaftTiles(otherConnection);
				otherConnection.DestroyConnection();
			}

			other.connected[(int)oppositeDirection] = elevator;

			// remove old shaft if exists
			Elevator.Shaft oldShaft = elevator.connectingShafts[(int)direction];
			RemoveShaftTiles(oldShaft);
			oldShaft.DestroyConnection();

			// get tilemap position of elevator entrances
			bool belowOther = elevator.transform.position.y <= other.transform.position.y;
			Elevator.Shaft newShaft;
			Vector3Int startPos;
			Vector3Int endPos;
			if (belowOther)
			{
				startPos = shaftTilemap.WorldToCell(elevator.transform.position);
				endPos = shaftTilemap.WorldToCell(other.transform.position);
				newShaft = new Elevator.Shaft(elevator, other);
			}
			else
			{
				startPos = shaftTilemap.WorldToCell(other.transform.position);
				endPos = shaftTilemap.WorldToCell(elevator.transform.position);
				newShaft = new Elevator.Shaft(other, elevator);
			}

			Vector3Int diff = endPos - startPos;

			bool hasBreakPos = false;
			bool buildingVertically; // is shaft currently building vertically
			Vector3Int pairOffset; // will be right or down, depending on horz or vert shaft
			Vector3Int buildDirection; // current direction shaft is being built
			Vector3Int horizontalDirection; // is shaft going left or right from origin?
			Tile[] currentShaft; // the current tileset being used to build the shaft
			Vector3Int[] midpoints = new Vector3Int[2]; // where the shaft changes directions

			if (direction == Elevator.Directions.Up || direction == Elevator.Directions.Down)
			{
				buildingVertically = true;
				pairOffset = right;
				buildDirection = up;
				horizontalDirection = right;
				currentShaft = verticalShaft;
			}
			else
			{
				buildingVertically = false;
				pairOffset = down;
				if (!belowOther)
				{
					buildDirection = (direction == Elevator.Directions.Left) ? right : left;
					horizontalDirection = (direction == Elevator.Directions.Left) ? right : left;
				}
				else
				{
					buildDirection = (direction == Elevator.Directions.Left) ? left : right;
					horizontalDirection = (direction == Elevator.Directions.Left) ? left : right;
				}
				currentShaft = horizontalShaft;
			}

			int nextBreakPos = 0;

			// are shafts vertically aligned?
			switch (direction)
			{
				case Elevator.Directions.Up:
				case Elevator.Directions.Down:
					if (diff.x != 0)
					{
						hasBreakPos = true;
						int vertMidpoint = startPos.y + (int)(.5f * diff.y);
						midpoints[0] = new Vector3Int(startPos.x, vertMidpoint, 0) + left + down;
						midpoints[1] = new Vector3Int(endPos.x, vertMidpoint, 0);

						if (startPos.x < endPos.x)
						{
							midpoints[1] += left;
						}

						horizontalDirection = startPos.x < endPos.x ? right : left;
					}
					break;
				case Elevator.Directions.Left:
				case Elevator.Directions.Right:
					if (diff.y != 0)
					{
						hasBreakPos = true;
						int horzMidPoint = startPos.x + (int)(.5f * diff.x);
						midpoints[0] = new Vector3Int(horzMidPoint, startPos.y, 0);
						midpoints[1] = new Vector3Int(horzMidPoint, endPos.y, 0);
						if (belowOther)
							horizontalDirection = (direction == Elevator.Directions.Left) ? left : right;
						else
							horizontalDirection = (direction == Elevator.Directions.Left) ? right : left;
					}
					break;
			}


			Vector3Int currentPos = startPos + left; // start building from topleft of door
			switch (direction)
			{
				case Elevator.Directions.Up:
				case Elevator.Directions.Down:
					endPos += down + left; // stop building at bottomleft of door
					break;
				case Elevator.Directions.Right:
					if (belowOther)
					{
						midpoints[0] += left;
						midpoints[1] += down + left;
					}
					else
					{
						endPos += left + left;
						midpoints[1] += left + down;
					}
					break;
				case Elevator.Directions.Left:
					if (belowOther)
					{
						endPos += left + left;
						midpoints[1] += left + down;
					}
					else
					{
						midpoints[0] += left;
						midpoints[1] += down + left;
					}
					break;
			}

			newShaft.waypoints.Add(startPos);

			switch (direction)
			{
				case Elevator.Directions.Up:
				case Elevator.Directions.Down:
					newShaft.waypoints.Add(midpoints[0] + right + up);
					newShaft.waypoints.Add(midpoints[1]);
					newShaft.waypoints.Add(endPos + right + up);
					break;
				case Elevator.Directions.Right:
					newShaft.waypoints.Add(midpoints[0] + right);
					newShaft.waypoints.Add(midpoints[1] + right + up);
					newShaft.waypoints.Add(endPos);
					break;
				case Elevator.Directions.Left:
					newShaft.waypoints.Add(midpoints[0] + right);
					newShaft.waypoints.Add(midpoints[1] + right + up);
					newShaft.waypoints.Add(endPos);
					break;
			}

			int nextTile = 0;
			while (currentPos != endPos)
			{
				shaftTilemap.SetTile(currentPos, currentShaft[nextTile++]);
				shaftTilemap.SetTile(currentPos + pairOffset, currentShaft[nextTile++]);

				newShaft.tiles.Add(currentPos);
				newShaft.tiles.Add(currentPos + pairOffset);

				bgTilemap.SetTile(currentPos, internalShaft[0]);
				bgTilemap.SetTile(currentPos + pairOffset, internalShaft[1]);
				if (nextTile >= currentShaft.Length)
					nextTile = 0;
				currentPos += buildDirection;
				if (hasBreakPos && currentPos == midpoints[nextBreakPos])
				{
					var lastDirection = buildDirection;
					buildingVertically = !buildingVertically;
					currentShaft = buildingVertically ? verticalShaft : horizontalShaft;
					pairOffset = buildingVertically ? right : down;
					buildDirection = buildingVertically ? up : horizontalDirection;
					nextTile = 0;
					if (++nextBreakPos == 2)
						hasBreakPos = false;


					if (buildDirection == right)
					{
						shaftTilemap.SetTile(currentPos, cornerShaftRD[0]);
						shaftTilemap.SetTile(currentPos + right, cornerShaftRD[1]);
						newShaft.tiles.Add(currentPos);
						newShaft.tiles.Add(currentPos + right);
						currentPos += up;
						shaftTilemap.SetTile(currentPos, cornerShaftRD[2]);
						shaftTilemap.SetTile(currentPos + right, cornerShaftRD[3]);
						newShaft.tiles.Add(currentPos);
						newShaft.tiles.Add(currentPos + right);
						currentPos += right;
						currentPos += right;
						continue;
					}

					if (buildDirection == left)
					{
						shaftTilemap.SetTile(currentPos, cornerShaftLD[0]);
						shaftTilemap.SetTile(currentPos + right, cornerShaftLD[1]);
						newShaft.tiles.Add(currentPos);
						newShaft.tiles.Add(currentPos + right);
						currentPos += up;
						shaftTilemap.SetTile(currentPos, cornerShaftLD[2]);
						shaftTilemap.SetTile(currentPos + right, cornerShaftLD[3]);
						newShaft.tiles.Add(currentPos);
						newShaft.tiles.Add(currentPos + right);
						currentPos += left;
						continue;
					}

					if (buildDirection != up)
						Debug.LogError("We dun fq'd up, boi!");

					if (lastDirection == right)
					{
						shaftTilemap.SetTile(currentPos, cornerShaftLU[0]);
						shaftTilemap.SetTile(currentPos + down, cornerShaftLU[1]);
						newShaft.tiles.Add(currentPos);
						newShaft.tiles.Add(currentPos + down);
						currentPos += right;
						shaftTilemap.SetTile(currentPos, cornerShaftLU[2]);
						shaftTilemap.SetTile(currentPos + down, cornerShaftLU[3]);
						newShaft.tiles.Add(currentPos);
						newShaft.tiles.Add(currentPos + down);
						currentPos += up;
						currentPos += left;
						continue;
					}
					else
					{ // last direction was left
						shaftTilemap.SetTile(currentPos, cornerShaftRU[0]);
						shaftTilemap.SetTile(currentPos + down, cornerShaftRU[1]);
						newShaft.tiles.Add(currentPos);
						newShaft.tiles.Add(currentPos + down);
						currentPos += left;
						shaftTilemap.SetTile(currentPos, cornerShaftRU[2]);
						shaftTilemap.SetTile(currentPos + down, cornerShaftRU[3]);
						newShaft.tiles.Add(currentPos);
						newShaft.tiles.Add(currentPos + down);
						currentPos += up;
					}
				}

				if (Mathf.Abs(currentPos.x) > 50 || Mathf.Abs(currentPos.y) > 50)
				{
					throw new System.Exception("dun fk'd up");
				}
			}

			if (direction == Elevator.Directions.Down
				|| direction == Elevator.Directions.Up)
			{
				currentPos += down;
				if (nextTile == 0)
				{ // make sure we don't have any cut off windows
					currentPos += down;
					shaftTilemap.SetTile(currentPos, verticalShaft[0]);
					shaftTilemap.SetTile(currentPos + right, verticalShaft[1]);
					newShaft.tiles.Add(currentPos);
					newShaft.tiles.Add(currentPos + right);
					currentPos += up;
				}

				shaftTilemap.SetTile(currentPos, verticalShaftGround[0]);
				shaftTilemap.SetTile(currentPos + right, verticalShaftGround[1]);
				newShaft.tiles.Add(currentPos);
				newShaft.tiles.Add(currentPos + right);
			}

			elevator.connectingShafts[(int)direction] = newShaft;
			other.connectingShafts[(int)oppositeDirection] = newShaft;
		}

		private void RemoveShaftTiles(Elevator.Shaft oldShaft)
		{
			if (oldShaft == null || oldShaft.tiles == null)
				return;
			foreach (var tile in oldShaft.tiles)
			{
				shaftTilemap.SetTile(tile, null);
				bgTilemap.SetTile(tile, null);
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