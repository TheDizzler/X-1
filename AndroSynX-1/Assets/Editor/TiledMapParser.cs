using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AtomosZ.Editor.Tiled
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
		private const string TILED_GID_TAG = "Tiled GID: ";
		private static TiledMapParser instance = new TiledMapParser();
		private XmlDocument tmxDoc;

		private string originalFilePath;
		private string newFilePath;

		/// <summary>
		/// <TiledGID, texture asset filepath>
		/// </summary>
		private Dictionary<string, string> spriteDict;
		private Vector2 mapStartPos = Vector2.zero;


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
				texImp.spriteImportMode = SpriteImportMode.Multiple;

				List<SpriteMetaData> sprites = new List<SpriteMetaData>();

				int tiledGID = int.Parse(tilesetNode.Attributes["firstgid"].InnerText);
				int tilewidth = int.Parse(tilesetNode.Attributes["tilewidth"].InnerText);
				int tileheight = int.Parse(tilesetNode.Attributes["tileheight"].InnerText);
				int columns = int.Parse(tilesetNode.Attributes["columns"].InnerText);
				int rows = int.Parse(tilesetNode.Attributes["tilecount"].InnerText) / columns;
				int spacing = 0;
				if (tilesetNode.ChildNodes[0].Attributes["spacing"] != null)
					spacing = int.Parse(tilesetNode.ChildNodes[0].Attributes["spacing"].InnerText);

				// Tiled (0, 0) is at the top left of the texture, like, you know, normal people.
				// Unity (0, 0) is at the bottom left, because, you know...Unity...
				for (int h = 0; h < rows; ++h)
				{
					int row = rows - 1;
					for (int w = 0; w < columns; ++w)
					{
						// fill in sprite meta data here
						string tiledGIDName = TILED_GID_TAG + tiledGID;
						sprites.Add(new SpriteMetaData {
							name = tiledGIDName,
							rect = new Rect(
								(w * tilewidth) + spacing,
								((row - h) * tileheight) + spacing,
								tilewidth, tileheight)
						});

						spriteDict.Add(tiledGIDName, "Assets/TiledImports/TMXAssets/" + filename);
						++tiledGID;
					}
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
			float mapTileWidth = float.Parse(map.Attributes["tilewidth"].InnerText) / 100;
			float mapTileHeight = float.Parse(map.Attributes["tileheight"].InnerText) / 100;

			Scene scene = SceneManager.GetActiveScene();
			foreach (XmlNode node in map.ChildNodes)
			{
				if (node.Name == "layer")
				{
					string layerName = node.Attributes["name"].InnerText;
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

					XmlNode nodes = node.FirstChild;
					if (nodes.Attributes["encoding"] != null && nodes.Attributes["encoding"].InnerText == "csv")
					{
						using (StringReader reader = new StringReader(nodes.InnerText))
						{
							string line;
							Vector2 pos = mapStartPos;
							while ((line = reader.ReadLine()) != null)
							{
								if (string.IsNullOrWhiteSpace(line))
									continue;
								foreach (string gid in line.Split(','))
								{
									if (!string.IsNullOrWhiteSpace(gid))
									{
										if (gid != "0")
										{
											string tiledGID = TILED_GID_TAG + gid;
											GameObject newGO = new GameObject();
											newGO.transform.localPosition = pos;
											newGO.transform.SetParent(rootObject.transform);
											SpriteRenderer sr = newGO.AddComponent<SpriteRenderer>();
											if (!spriteDict.TryGetValue(tiledGID, out string filepath))
											{
												Debug.LogError("Could not find filepath for " + tiledGID);
												continue;
											}

											Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(filepath);
											foreach (Object sprite in sprites)
											{
												if (((Sprite)sprite).name == tiledGID)
												{
													sr.sprite = ((Sprite)sprite);
													break;
												}
											}
										}
									}
									pos.x += mapTileWidth;
								}
								pos.x = mapStartPos.x;
								pos.y -= mapTileHeight;
							}
						}
					}
				}
			}
		}
	}
}