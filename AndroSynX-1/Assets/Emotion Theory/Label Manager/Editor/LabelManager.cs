using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Reflection;

/*
 * 
 * 
 * 
 * % (ctrl)
 * # (shift)
 * & (alt)
 * _ (none)
 * 
 * 
 * 
 */
public class LabelManager : Editor {

	#region LABELS
	public enum LabelId {
		None = -1,
		Gray = 0,
		Blue,
		Teal,
		Green,
		Yellow,
		Orange,
		Red,
		Purple
	}
	public enum IconId {
		None = -1,
		CircleGray = 0,
		CircleBlue,
		CircleTeal,
		CircleGreen,
		CircleYellow,
		CircleOrange,
		CircleRed,
		CirclePurple,
		DiamondGray,
		DiamondBlue,
		DiamondTeal,
		DiamondGreen,
		DiamondYellow,
		DiamondOrange,
		DiamondRed,
		DiamondPurple
	}

	public static void AssignIcon(GameObject g, IconId id)
	{
		AssignIcon (g, (int)id);
	}
	public static void AssignLabel(GameObject g, LabelId id)
	{
		AssignLabel (g, (int)id);
	}
	private static void AssignIcon(GameObject g, int id = 0)
	{
		string s = "sv_icon_dot" + id + "_pix16_gizmo";

		AssignLabelOrIcon (g, s);
	}
	private static void AssignLabel(GameObject g, int id = 0)
	{
		string s = "sv_label_" + id;

		AssignLabelOrIcon (g, s);
	}
	private static void ClearLabelOrIcon(GameObject g)
	{
		AssignLabelOrIcon (g, "");
	}
	private static void AssignLabelOrIcon(GameObject g, string texName)
	{
		Texture2D tex = string.IsNullOrEmpty (texName) ? null : EditorGUIUtility.IconContent(texName).image as Texture2D;
		Type editorGUIUtilityType  = typeof(EditorGUIUtility);
		BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
		object[] args = new object[] {g, tex};
		editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);

		EditorUtility.SetDirty(g);
	}
	#endregion

	private static LabelId lastUsedLabel = LabelId.Red;
	private static IconId lastUsedIcon = IconId.DiamondRed;
//	private static bool lastUsedIsIcon;
	private static bool toggleLabels;
	private static int iconIndex;
	private static int labelIndex;

//	[MenuItem ("Tools/Label Manager/ON")]
//	public static void TurnOnLabels ()
//	{
//		ToggleLabels (true);
//	}

	#region LABEL METHODS

	[MenuItem ("Tools/Label Manager/Set Label/Red")]
	public static void TurnOnLabelsRed () { ToggleLabels (true, LabelId.Red); }

	[MenuItem ("Tools/Label Manager/Set Label/Blue")]
	public static void TurnOnLabelsBlue () { ToggleLabels (true, LabelId.Blue); }

	[MenuItem ("Tools/Label Manager/Set Label/Gray")]
	public static void TurnOnLabelsGray () { ToggleLabels (true, LabelId.Gray); }

	[MenuItem ("Tools/Label Manager/Set Label/Green")]
	public static void TurnOnLabelsGreen () { ToggleLabels (true, LabelId.Green); }

	[MenuItem ("Tools/Label Manager/Set Label/Orange")]
	public static void TurnOnLabelsOrange () { ToggleLabels (true, LabelId.Orange); }

	[MenuItem ("Tools/Label Manager/Set Label/Purple")]
	public static void TurnOnLabelsPurple () { ToggleLabels (true, LabelId.Purple); }

	[MenuItem ("Tools/Label Manager/Set Label/Teal")]
	public static void TurnOnLabelsTeal () { ToggleLabels (true, LabelId.Teal); }

	[MenuItem ("Tools/Label Manager/Set Label/Yellow")]
	public static void TurnOnLabelsYellow () { ToggleLabels (true, LabelId.Yellow); }

	#endregion

	#region ICON METHODS

	// DIAMOND ----------------------------------------------------------------------------------------
	[MenuItem ("Tools/Label Manager/Set Icon/Diamond - Red")]
	public static void TurnOnLabelsDiamondRed () { ToggleIcons (true, IconId.DiamondRed); }

	[MenuItem ("Tools/Label Manager/Set Icon/Diamond - Blue")]
	public static void TurnOnLabelsDiamondBlue () { ToggleIcons (true, IconId.DiamondBlue); }

	[MenuItem ("Tools/Label Manager/Set Icon/Diamond - Gray")]
	public static void TurnOnLabelsDiamondGray () { ToggleIcons (true, IconId.DiamondGray); }

	[MenuItem ("Tools/Label Manager/Set Icon/Diamond - Green")]
	public static void TurnOnLabelsDiamondGreen () { ToggleIcons (true, IconId.DiamondGreen); }

	[MenuItem ("Tools/Label Manager/Set Icon/Diamond - Orange")]
	public static void TurnOnLabelsDiamondOrange () { ToggleIcons (true, IconId.DiamondOrange); }

	[MenuItem ("Tools/Label Manager/Set Icon/Diamond - Purple")]
	public static void TurnOnLabelsDiamondPurple () { ToggleIcons (true, IconId.DiamondPurple); }

	[MenuItem ("Tools/Label Manager/Set Icon/Diamond - Teal")]
	public static void TurnOnLabelsDiamondTeal () { ToggleIcons (true, IconId.DiamondTeal); }

	[MenuItem ("Tools/Label Manager/Set Icon/Diamond - Yellow")]
	public static void TurnOnLabelsDiamondYellow () { ToggleIcons (true, IconId.DiamondYellow); }

	// CIRCLE ----------------------------------------------------------------------------------------
	[MenuItem ("Tools/Label Manager/Set Icon/Circle - Red")]
	public static void TurnOnLabelsCircleRed () { ToggleIcons (true, IconId.CircleRed); }

	[MenuItem ("Tools/Label Manager/Set Icon/Circle - Blue")]
	public static void TurnOnLabelsCircleBlue () { ToggleIcons (true, IconId.CircleBlue); }

	[MenuItem ("Tools/Label Manager/Set Icon/Circle - Gray")]
	public static void TurnOnLabelsCircleGray () { ToggleIcons (true, IconId.CircleGray); }

	[MenuItem ("Tools/Label Manager/Set Icon/Circle - Green")]
	public static void TurnOnLabelsCircleGreen () { ToggleIcons (true, IconId.CircleGreen); }

	[MenuItem ("Tools/Label Manager/Set Icon/Circle - Orange")]
	public static void TurnOnLabelsCircleOrange () { ToggleIcons (true, IconId.CircleOrange); }

	[MenuItem ("Tools/Label Manager/Set Icon/Circle - Purple")]
	public static void TurnOnLabelsCirclePurple () { ToggleIcons (true, IconId.CirclePurple); }

	[MenuItem ("Tools/Label Manager/Set Icon/Circle - Teal")]
	public static void TurnOnLabelsCircleTeal () { ToggleIcons (true, IconId.CircleTeal); }

	[MenuItem ("Tools/Label Manager/Set Icon/Circle - Yellow")]
	public static void TurnOnLabelsCircleYellow () { ToggleIcons (true, IconId.CircleYellow); }

	#endregion

	#region MENU METHODS

	[MenuItem ("Tools/Label Manager/CLEAR")]
	public static void TurnOffLabels ()
	{
		ToggleLabels (false);
	}

	[MenuItem ("Tools/Label Manager/TOGGLE LABEL %#L")]
	private static void ToggleLabels()
	{
		ToggleLabels (!toggleLabels);
	}

	[MenuItem ("Tools/Label Manager/TOGGLE ICON %#I")]
	private static void ToggleIcons()
	{
		ToggleIcons (!toggleLabels);
	}

	#endregion

	private static void ToggleLabels (bool on)
	{
//		if (lastUsedIsIcon)
//			ToggleIcons (on, lastUsedIcon);
//		else
//			ToggleLabels (on, lastUsedLabel);

		ToggleLabels (on, lastUsedLabel);
	}

	private static void ToggleIcons (bool on)
	{
		//		if (lastUsedIsIcon)
		//			ToggleIcons (on, lastUsedIcon);
		//		else
		//			ToggleLabels (on, lastUsedLabel);

		ToggleIcons (on, lastUsedIcon);
	}

	private static void ToggleLabels(bool on, LabelId label)
	{
		toggleLabels = on;

		var list = Selection.gameObjects;

		foreach (var sp in list) 
		{
			if (!toggleLabels) 
			{
				ClearLabelOrIcon (sp.gameObject);
			} 
			else 
			{
				// Add label to base object.
				AssignLabel (sp.gameObject, label);
			}
		}

		if (on)
		{
			lastUsedLabel = label;
//			lastUsedIsIcon = false;
		}
	}

	private static void ToggleIcons (bool on, IconId icon)
	{
		toggleLabels = on;

		var list = Selection.gameObjects;

		foreach (var sp in list) 
		{
			if (!toggleLabels) 
			{
				ClearLabelOrIcon (sp.gameObject);
			} 
			else 
			{
				// Add label to base object.
				AssignIcon (sp.gameObject, icon);
			}
		}

		if (on)
		{
			lastUsedIcon = icon;
//			lastUsedIsIcon = true;
		}
	}

}
