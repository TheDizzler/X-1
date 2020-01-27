using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AtomosZ.Editor.Tiled
{
	public class TiledMapWindow : EditorWindow
	{
		private enum DialogChoice { NEW_SCENE = 0, CANCEL = 1, CURRENT_SCENE = 2 }

		[SerializeField] static Vector2 scrollPos = new Vector2(0f, 0f);

		public static string rootTiledPath;
		public static string tiledAssetsPath;


		[MenuItem("Tools/Load Tiled Map")]
		public static void ShowWindow()
		{
			//	TiledMapWindow window = EditorWindow.GetWindow<TiledMapWindow>("Tiled Maps");
			//	window.Show();
			//}

			//public void OnEnable()
			//{
			rootTiledPath = Application.dataPath + "/TiledImports/";
			tiledAssetsPath = rootTiledPath + "TMXAssets/";
			if (!Directory.Exists(rootTiledPath))
			{
				Directory.CreateDirectory(rootTiledPath);
				Debug.Log("Creating folder " + rootTiledPath);

				if (!Directory.Exists(tiledAssetsPath))
				{
					Directory.CreateDirectory(tiledAssetsPath);
					Debug.Log("Creating folder " + tiledAssetsPath);
				}
			}
			//}

			//void OnGUI()
			//{
			//GUILayout.Label("Tiled Maps", EditorStyles.boldLabel);
			//if (GUILayout.Button("Find more Tiled files"))
			//{
			string newTMXFilePath = EditorUtility.OpenFilePanel("Select Tiled file", "", "tmx");
			if (newTMXFilePath.Length != 0)
			{
				ChooseAction((DialogChoice)EditorUtility.DisplayDialogComplex("Load this Tiled map?",
						"Would you like to load this Tiled map into the current scene"
							+ " or in a new one?", "New Scene", "Cancel", "Current Scene"),
							newTMXFilePath);
			}
			//}

			//scrollPos = EditorGUILayout.BeginScrollView(scrollPos,
			//	GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

			//var searchOption = SearchOption.TopDirectoryOnly;
			//foreach (string tiledMapPath in Directory.GetFiles(rootTiledPath, "*.tmx", searchOption))
			//{
			//	string levelName = Path.GetFileNameWithoutExtension(tiledMapPath);
			//	if (GUILayout.Button("Re-import " + levelName))
			//	{
			//		//ChooseAction((DialogChoice)EditorUtility.DisplayDialogComplex("Load this Tiled map?",
			//		//	"Would you like to load this Tiled map into the current scene"
			//		//		+ " or in a new one?", "New Scene", "Cancel", "Current Scene"),
			//		//		tiledMapPath);
			//	}
			//}

			//EditorGUILayout.EndScrollView();
		}


		private static void ChooseAction(DialogChoice choice, string tiledMapPath)
		{
			if (choice == DialogChoice.CANCEL)
				return;

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			if (choice == DialogChoice.CURRENT_SCENE)
			{

			}
			else if (choice == DialogChoice.NEW_SCENE)
			{
				Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
				TiledMapParser.ParseMap(tiledMapPath);
			}
		}
	}
}