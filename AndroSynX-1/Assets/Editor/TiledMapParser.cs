using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AtomosZ.Editor.Tiled
{
	/// <summary>
	/// Imports Tiled Maps with image assets into Unity project.
	/// TODO: Load and create map layout.
	/// TODO (Optional): animations.
	/// </summary>
	public class TiledMapParser
	{
		private static TiledMapParser instance = new TiledMapParser();
		private XmlDocument tmxDoc;

		private string originalFilePath;
		private string newFilePath;


		public static void ParseMap(string tiledMapPath)
		{
			instance.Parse(tiledMapPath);
		}

		private void Parse(string tiledMapPath)
		{
			originalFilePath = tiledMapPath.Substring(0, tiledMapPath.LastIndexOf('/') + 1);
			newFilePath = TiledMapWindow.rootTiledPath;

			Scene scene = EditorSceneManager.GetActiveScene();
			tmxDoc = new XmlDocument();
			tmxDoc.Load(tiledMapPath);

			ImportImageAssets();
			//LoadLayerData();

			//string filename = tiledMapPath.Substring(
			//	tiledMapPath.LastIndexOf('/'), tiledMapPath.Length - tiledMapPath.LastIndexOf('/'));
			//File.Copy(tiledMapPath, TiledMapWindow.rootTiledPath + filename); // don't need this?
		}


		private void ImportImageAssets()
		{
			// load Map description
			XmlNode map = tmxDoc.GetElementsByTagName("map")[0];
			int mapWidth = Int32.Parse(map.Attributes["width"].InnerText);
			int mapHeight = Int32.Parse(map.Attributes["height"].InnerText);
			int mapTileWidth = Int32.Parse(map.Attributes["tilewidth"].InnerText);
			int mapTileHeight = Int32.Parse(map.Attributes["tileheight"].InnerText);

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

				//string filename = file.Substring(file.LastIndexOf('/') + 1, file.Length - file.LastIndexOf('/') - 1);
				string filename = Path.GetFileName(file);
				File.Copy(file, TiledMapWindow.tiledAssetsPath + filename, true);
				AssetDatabase.ImportAsset("Assets/TiledImports/TMXAssets/" + filename, ImportAssetOptions.ForceUpdate);
				//AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
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

				int tiledGID = Int32.Parse(tilesetNode.Attributes["firstgid"].InnerText);

				int tilewidth = Int32.Parse(tilesetNode.Attributes["tilewidth"].InnerText);
				int tileheight = Int32.Parse(tilesetNode.Attributes["tileheight"].InnerText);

				int columns = Int32.Parse(tilesetNode.Attributes["columns"].InnerText);
				int rows = Int32.Parse(tilesetNode.Attributes["tilecount"].InnerText) / columns;

				int spacing = 0;
				if (tilesetNode.ChildNodes[0].Attributes["spacing"] != null)
					spacing = Int32.Parse(tilesetNode.ChildNodes[0].Attributes["spacing"].InnerText);

				for (int h = 0; h < rows; ++h)
				{
					for (int w = 0; w < columns; ++w)
					{
						// fill in sprite meta data here
						SpriteMetaData smd = new SpriteMetaData();
						smd.name = "Tiled GID: " + tiledGID;
						smd.rect = new Rect(
							(w * tilewidth) + spacing,
							(h * tileheight) + spacing,
							tilewidth, tileheight);
						sprites.Add(smd);
						++tiledGID;
					}
				}

				texImp.spritesheet = sprites.ToArray();
			}
		}
	}
}