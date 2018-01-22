using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.Collections;
using System.IO;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

using UnityEditor.AnimatedValues;


public class SceneStacksNew : EditorWindow
{
	// Scriptable Object Settings
	static public SceneStackSaveSettingsObject sceneStackSaveSettings;

	// GUI EDITOR WINDOW VARS
	Vector2 scrollPos;
	bool sceneStackWindowMenuFoldout = false;
	static AnimBool expandFields_SceneStackOptions;
	static bool saveFileNotAssigned = false;

	// ICONS
	Texture saveIcon;
	Texture openIcon;
	const string moveUpIcon = "\u25B2";
	const string moveDownIcon = "\u25BC";




	void OnEnable()
	{
		saveIcon = (Texture)EditorGUIUtility.Load("SceneMenuPlus/Icons/saveBtn.png"); 
		openIcon = (Texture)EditorGUIUtility.Load("SceneMenuPlus/Icons/openBtn.png");
		expandFields_SceneStackOptions = new AnimBool(false);
		expandFields_SceneStackOptions.valueChanged.AddListener(Repaint);
	}

	void OnGUI()
	{
		#region GUI Styles 
		GUIStyle titleStyle = new GUIStyle (GUI.skin.label);
		titleStyle.fontSize = 14;
		titleStyle.fontStyle = FontStyle.Bold;

		GUIStyle backgroundStyle = new GUIStyle();
		backgroundStyle.normal.background = DarwTexture(1, 1, new Color(.15f, .15f, .15f, 1.0f));

		GUIStyle stackbackgroundStyle = new GUIStyle();
		stackbackgroundStyle.normal.background = DarwTexture(1, 1, new Color(.25f, .25f, .25f, 1.0f));

		GUIStyle childbackgroundStyle = new GUIStyle();
		childbackgroundStyle.normal.background = DarwTexture(1, 1, new Color(.2f, .2f, .2f, 1.0f));
		#endregion


		// TITLE BAR \\\\\\\\\\\\\\\\\\\\\\\\\
		//Debug.Log(position.xMax);
		GUILayout.BeginArea(new Rect(3, 3, position.width-7, 160));
			EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.BeginHorizontal();
					GUILayout.Label ("Scene Stacks", titleStyle);
					sceneStackWindowMenuFoldout = EditorGUI.Foldout( new Rect(position.width-28, 6, position.width - 6,18), sceneStackWindowMenuFoldout,new GUIContent("", "Expand Scene Stack Options") );
				EditorGUILayout.EndHorizontal();
			
				if (sceneStackWindowMenuFoldout)
				{
					EditorGUILayout.BeginHorizontal();
					if (DrawButtonFlexSize("Create Blank", "Create a new blank scene stack", 80f))
					{	
						CreateSceneStack();
					}
				
					if (DrawButtonFlexSize("Create", "Create a new scene stack from open scenes", 50f))
					{	
						createSceneStackFromOpenScenes();
					}

					

					if ( GUILayout.Button( EditorGUIUtility.IconContent("_Popup","Scene Stack Options"), GUILayout.MaxWidth(28f),  GUILayout.Height(20f)) )
					{
						expandFields_SceneStackOptions.target = !expandFields_SceneStackOptions.target;
					}

					EditorGUILayout.EndHorizontal();

					//EXPAND FIELD - Options
					if (expandFields_SceneStackOptions.target) EditorGUILayout.BeginVertical("Box");

					if (EditorGUILayout.BeginFadeGroup(expandFields_SceneStackOptions.faded))
					{
						EditorGUILayout.LabelField("Scene Stack Save Settings File:");
						sceneStackSaveSettings = EditorGUILayout.ObjectField("", sceneStackSaveSettings , typeof(ScriptableObject), false, GUILayout.MinWidth(128)) as SceneStackSaveSettingsObject; 
				     	if (saveFileNotAssigned) 
						{	
							EditorGUILayout.HelpBox("Save File not Assigned", MessageType.Error, true);

							if (DrawButtonFlexSize("! FIX UNASSIGNED SAVE FILE !", "Finds or Creates Scene Stack save file", 80f))
							{	
								sceneStackSaveSettings = AssetDatabase.LoadAssetAtPath("Assets/Editor/SceneMenuPlus/SceneStacks/SaveSettings/SceneStackSettings.asset", typeof(SceneStackSaveSettingsObject)) as SceneStackSaveSettingsObject;
								if (!sceneStackSaveSettings)
								{
									SceneStackSaveSettingsObject SceneStackSettings = ScriptableObject.CreateInstance<SceneStackSaveSettingsObject>();
									AssetDatabase.CreateAsset(SceneStackSettings, "Assets/Editor/SceneMenuPlus/SceneStacks/SaveSettings/SceneStackSettings.asset");
									sceneStackSaveSettings = AssetDatabase.LoadAssetAtPath("Assets/Editor/SceneMenuPlus/SceneStacks/SaveSettings/SceneStackSettings.asset", typeof(SceneStackSaveSettingsObject)) as SceneStackSaveSettingsObject;
								}
								saveFileNotAssigned = false;
								AssetDatabase.SaveAssets();
							}
						}	
					}
					EditorGUILayout.EndFadeGroup();
					if (expandFields_SceneStackOptions.target) EditorGUILayout.EndVertical();
				}


			EditorGUILayout.EndVertical();
		GUILayout.EndArea();

	}

	public void OnInspectorUpdate()
	{
		this.Repaint();
	}



	static public void CreateSceneStack()
	{
		SceneStackObject newSceneStack = ScriptableObject.CreateInstance<SceneStackObject>();

		if (sceneStackSaveSettings) 
		{
			AssetDatabase.CreateAsset(newSceneStack, "Assets/Editor/SceneMenuPlus/SceneStacks/SceneStackObjects/newSceneStack" + sceneStackSaveSettings.numOfSceneStacks.ToString() +".asset");
			sceneStackSaveSettings.numOfSceneStacks ++;
			sceneStackSaveSettings.SceneStacks.Add(newSceneStack);
		}
		else 
		{
			saveFileNotAssigned = true;
			expandFields_SceneStackOptions.target = true;
			Debug.LogAssertion("Missing Scene Stack save file");
			EditorApplication.Beep();
		}
			
		AssetDatabase.SaveAssets();

		//* unity error when the project window is already visible *
		//EditorUtility.FocusProjectWindow();
		Selection.activeObject = newSceneStack;
	}


	static public void createSceneStackFromOpenScenes()
	{
		CreateSceneStack();

	}


	// //---------------------------------------\\
	#region Menu Shortcuts 
	[MenuItem("Assets/Scene Menu/Show Window New")]
	static public void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(SceneStacksNew),false, "Scene Menu Options New", true);
	}

	[MenuItem("Assets/Scene Menu/Scene Stacks/Create New Empty Scene Stack new")]
	static public void createNewEmptySceneStack()
	{
		CreateSceneStack();
	}
	#endregion
	// \\________________________________________//



	// //---------------------------------------\\
	#region UI ELEMENTS 
	bool DrawButtonFlexSize (string title, string tooltip, float width) 
	{
		return GUILayout.Button(new GUIContent(title, tooltip), GUILayout.MinWidth(width),  GUILayout.Height(20f) );
	}

	private Texture2D DarwTexture(int width, int height, Color col)
	{
		Color[] pix = new Color[width*height];

		for(int i = 0; i < pix.Length; i++)
			pix[i] = col;

		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();

		return result;
	}
	#endregion
	// \\________________________________________//
}
