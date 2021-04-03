using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AtomosZ.AndroSyn.Editors.Tiled
{
	/// <summary>
	/// Imports Tiled Maps with image assets into Unity project.
	/// TODO:	Layer sorting?
	///			Collider data
	///			Parse object layers
	///			(Optional) animations
	/// </summary>
	public class TiledMapParser
	{
		private const string TILED_GID_TAG = "TiledGID - ";
		private const float PIXELS_PER_UNIT = 100;
		private static TiledMapParser instance = new TiledMapParser();
		private XmlDocument tmxDoc;

		private string originalFilePath;
		private string newFilePath;

		/// <summary>
		/// <TiledGID, texture asset filepath>
		/// </summary>
		private Dictionary<string, string> spriteDict;
		private Dictionary<string, string> gidToSpriteName = new Dictionary<string, string>();
		private Vector2 mapStartPos = Vector2.zero;
		private float mapTileWidth;
		private float mapTileHeight;
		private bool importAssetsOnly = false;
		private bool importColliderLayers = true;


		public static void ParseMap(string tiledMapPath)
		{
			instance.Parse(tiledMapPath);
		}

		private void Parse(string tiledMapPath)
		{
			spriteDict = new Dictionary<string, string>();
			originalFilePath = tiledMapPath.Substring(0, tiledMapPath.LastIndexOf('/') + 1);
			newFilePath = TiledMapWindow.rootTiledPath;

			Scene scene = SceneManager.GetActiveScene();
			tmxDoc = new XmlDocument();
			tmxDoc.Load(tiledMapPath);

			ImportImageAssets();
			if(importAssetsOnly)
				return;
			ImportLayerData();

			//File.Copy(tiledMapPath, TiledMapWindow.rootTiledPath + filename); // don't need this?
		}

		private void ImportImageAssets()
		{
			// load tilesets
			foreach (XmlNode tilesetNode in tmxDoc.GetElementsByTagName("tileset"))
			{
				string tilesetFileName = tilesetNode.ChildNodes[0].Attributes["source"].InnerText;
				String file = originalFilePath + tilesetFileName;
				if (!File.Exists(file))
				{
					Debug.LogError("Cannot find tileset image " + file);
					continue;
				}

				string filename = Path.GetFileName(file);
				File.Copy(file, TiledMapWindow.tiledAssetsPath + filename, true);
				AssetDatabase.ImportAsset("Assets/TiledImports/TMXAssets/" + filename, ImportAssetOptions.ForceUpdate);


				string metafile = TiledMapWindow.tiledAssetsPath + filename + ".meta";
				if (!File.Exists(metafile))
				{
					Debug.LogError("No metafile!");
					continue;
				}


				AssetImporter assImp = AssetImporter.GetAtPath("Assets/TiledImports/TMXAssets/" + filename);
				if (assImp == null)
				{
					Debug.LogError("Assets/TiledImports/TMXAssets/" + filename + " could not be found!");
					continue;
				}

				TextureImporter texImp = (TextureImporter)assImp;
				texImp.spritePivot = Vector2.zero;
				texImp.spriteImportMode = SpriteImportMode.Multiple;

				List<SpriteMetaData> sprites = new List<SpriteMetaData>();

				int tiledGID = int.Parse(tilesetNode.Attributes["firstgid"].InnerText);
				int tilewidth = int.Parse(tilesetNode.Attributes["tilewidth"].InnerText);
				int tileheight = int.Parse(tilesetNode.Attributes["tileheight"].InnerText);
				int columns = int.Parse(tilesetNode.Attributes["columns"].InnerText);
				int rows = int.Parse(tilesetNode.Attributes["tilecount"].InnerText) / columns;
				int spacing = 0;
				if (tilesetNode.Attributes["spacing"] != null)
					spacing = int.Parse(tilesetNode.ChildNodes[0].Attributes["spacing"].InnerText);

				Dictionary<string, TileType> nameToType = new Dictionary<string, TileType>();
				Dictionary<int, TileType> intToType = new Dictionary<int, TileType>();
				foreach (XmlNode typeNode in tilesetNode.SelectNodes("tile"))
				{
					if (typeNode.Attributes["type"] == null)
						continue; // this could be an animation node
					string name = typeNode.Attributes["type"].InnerText;
					int id = int.Parse(typeNode.Attributes["id"].InnerText);
					if (nameToType.TryGetValue(name, out TileType newTile))
					{
						newTile.ids.Add(id);
					}
					else
					{
						newTile = new TileType {
							name = name
						};
						newTile.ids.Add(id);
						nameToType.Add(name, newTile);
					}

					intToType.Add(id, newTile);
				}

				// Tiled (0, 0) is at the top left of the texture, like, you know, normal people.
				// Unity (0, 0) is at the bottom left, because, you know...Unity...
				int internalID = 0;
				for (int h = 0; h < rows; ++h)
				{
					int row = rows - 1;
					for (int w = 0; w < columns; ++w)
					{// fill in sprite meta data here
						string tiledGIDName = TILED_GID_TAG + tiledGID;
						SpriteMetaData metadata = new SpriteMetaData {
							name = tiledGIDName,
							rect = new Rect(
								(w * tilewidth) + spacing,
								((row - h) * tileheight) + spacing,
								tilewidth, tileheight),
						};

						if (intToType.TryGetValue(internalID, out TileType tileType))
						{   // this is part of a bigger sprite
							tileType.metaDatas.Add(metadata);
						}
						else
						{
							metadata.pivot = new Vector2(1, 1);
							metadata.alignment = 1;
							sprites.Add(metadata);
							spriteDict.Add(tiledGIDName, "Assets/TiledImports/TMXAssets/" + filename);
						}
						++tiledGID;
						++internalID;
					}
				}

				foreach (KeyValuePair<string, TileType> tile in nameToType)
				{
					TileType tt = tile.Value;
					SpriteMetaData bigsprite = new SpriteMetaData {
						name = tile.Key,
						rect = tt.metaDatas[0].rect,
					};
					foreach (SpriteMetaData smd in tt.metaDatas)
					{
						if (bigsprite.rect.xMax < smd.rect.xMax)
						{
							bigsprite.rect.xMax = smd.rect.xMax;
						}
						if (bigsprite.rect.y > smd.rect.y)
						{
							bigsprite.rect.y = smd.rect.y;
							bigsprite.rect.height += smd.rect.height;
						}
					}

					sprites.Add(bigsprite);
					spriteDict.Add(tile.Key, "Assets/TiledImports/TMXAssets/" + filename);
					gidToSpriteName.Add(tt.metaDatas[0].name, tile.Key);
				}

				texImp.spritesheet = sprites.ToArray();
				texImp.SaveAndReimport();
			}
		}


		private void ImportLayerData()
		{
			// load Map description
			XmlNode map = tmxDoc.GetElementsByTagName("map")[0];
			int mapWidth = Int32.Parse(map.Attributes["width"].InnerText);
			int mapHeight = Int32.Parse(map.Attributes["height"].InnerText);
			// operating under the assumption that 100 pixels == 1 Unity unit
			mapTileWidth = float.Parse(map.Attributes["tilewidth"].InnerText) / PIXELS_PER_UNIT;
			mapTileHeight = float.Parse(map.Attributes["tileheight"].InnerText) / PIXELS_PER_UNIT;

			Scene scene = SceneManager.GetActiveScene();
			int orderInLayer = -2;
			foreach (XmlNode node in map.ChildNodes)
			{
				switch (node.Name)
				{
					case "layer":
						ParseAndCreateLayer(node, scene, orderInLayer++);
						break;
					case "objectgroup":
						if (importColliderLayers && node.Attributes["name"].InnerText == "ColliderLayer")
							ParseAndCreateColliderLayer(node, scene);
						break;
				}
			}
		}

		private void ParseAndCreateColliderLayer(XmlNode colliderNode, Scene scene)
		{
			GameObject rootObject = null;
			foreach (GameObject obj in scene.GetRootGameObjects())
			{
				if (obj.name == "ColliderLayer")
				{
					rootObject = obj;
					break;
				}
			}

			if (rootObject == null)
				rootObject = new GameObject("ColliderLayer");

			foreach (XmlNode objectNode in colliderNode.ChildNodes)
			{
				GameObject newGO = new GameObject();
				newGO.transform.SetParent(rootObject.transform);
				if (objectNode.ChildNodes.Count == 0)
				{
					CreateBoxCollider(newGO, objectNode);
					continue;
				}

				GameObject.DestroyImmediate(newGO);
				switch (objectNode.ChildNodes[0].Name)
				{
					case "point":
						Debug.Log("point found");
						break;
					case "ellipse":
						Debug.Log("ellipse found");
						break;
					case "polygon":
						Debug.Log("polygon found");
						break;
				}
			}
		}

		private void CreateBoxCollider(GameObject newGO, XmlNode objectNode)
		{
			Vector2 pos = new Vector2(
					float.Parse(objectNode.Attributes["x"].InnerText) / PIXELS_PER_UNIT,
					float.Parse(objectNode.Attributes["y"].InnerText) / PIXELS_PER_UNIT);
			var box = newGO.AddComponent<BoxCollider2D>();
			box.size = new Vector2(
				float.Parse(objectNode.Attributes["width"].InnerText) / PIXELS_PER_UNIT,
				float.Parse(objectNode.Attributes["height"].InnerText) / PIXELS_PER_UNIT);
			pos.x += box.size.x / 2;
			pos.y -= box.size.y / 2;
			newGO.transform.localPosition = pos;
		}

		private void ParseAndCreateLayer(XmlNode layerNode, Scene scene, int orderInLayer)
		{
			string layerName = layerNode.Attributes["name"].InnerText;
			GameObject rootObject = null;
			foreach (GameObject obj in scene.GetRootGameObjects())
			{
				if (obj.name == layerName)
				{
					rootObject = obj;
					break;
				}
			}

			if (rootObject == null)
				rootObject = new GameObject(layerName);


			XmlNode nodes = layerNode.FirstChild;
			if (nodes.Attributes["encoding"] != null && nodes.Attributes["encoding"].InnerText == "csv")
			{
				using (StringReader reader = new StringReader(nodes.InnerText))
				{
					string line;
					Vector2 mappos = mapStartPos;
					//mappos.x += mapTileWidth / 2;
					//mappos.y -= mapTileHeight / 2;
					while ((line = reader.ReadLine()) != null)
					{
						if (string.IsNullOrWhiteSpace(line))
							continue;
						foreach (string gid in line.Split(','))
						{
							if (!string.IsNullOrWhiteSpace(gid) && gid != "0")
							{
								string tiledGID = TILED_GID_TAG + gid;
								if (!spriteDict.TryGetValue(tiledGID, out string filepath))
								{ // check to see if this GID has been replaced with a bigger sprite
									if (!gidToSpriteName.TryGetValue(tiledGID, out string tileName))
									{   // probably been combined into a bigger sprite
										continue;
									}

									if (!spriteDict.TryGetValue(tileName, out filepath))
									{
										Debug.LogWarning("Could not find filepath for " + tileName
											+ " in spriteDict");
										continue;
									}

									tiledGID = tileName;
								}

								Vector2 pos = mappos;
								GameObject newGO = new GameObject();
								newGO.name = tiledGID;
								newGO.transform.SetParent(rootObject.transform);

								Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(filepath);
								foreach (Object sprite in sprites)
								{
									if (((Sprite)sprite).name == tiledGID)
									{
										SpriteRenderer sr = newGO.AddComponent<SpriteRenderer>();
										sr.sprite = (Sprite)sprite;
										sr.sortingOrder = orderInLayer;
										pos.x += ((Sprite)sprite).rect.width / 200.0f;
										pos.y -= ((Sprite)sprite).rect.height / 200.0f;
										break;
									}
								}
								newGO.transform.localPosition = pos;
							}
							mappos.x += mapTileWidth;
						}
						mappos.x = mapStartPos.x;
						mappos.y -= mapTileHeight;
					}
				}
			}
		}

		private class TileType
		{
			public string name;
			public List<int> ids = new List<int>();
			public List<SpriteMetaData> metaDatas = new List<SpriteMetaData>();
		}
	}
}