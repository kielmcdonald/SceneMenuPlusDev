
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.Collections;
using System.IO;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;


public class SceneStackTools : EditorWindow
{
	// //---------------------------------------\\
	public class SceneStack
	{
		public bool showContenseOfSceneStack {get; set;}
		public string sceneStackName {get; set;}
		public int numChildrenScenes {get; set;}
		public Object sceneStackBaseScene {get; set;}
		public List <bool> setChildrenSceneActive = new List <bool>();
		public List <Object> childrenScenes = new List <Object>();

		public SceneStack(bool show, string name, int children)
		{
			showContenseOfSceneStack = show;
			sceneStackName = name;
			numChildrenScenes = children;
		}
	}
	// \\________________________________________//


	// Scene stack vars 
 	static int numOfSceneStacks = 0;
	static List <SceneStack> SceneStacks = new List<SceneStack>();

	// GUI VARS
	Vector2 scrollPos;
	bool sceneStackWindowMenuFoldout = false;

	// ICONS
	Texture saveIcon;
	Texture openIcon;
	const string moveUpIcon = "\u25B2";
	const string moveDownIcon = "\u25BC";



	void OnEnable()
	{
		saveIcon = (Texture)EditorGUIUtility.Load("SceneMenuPlus/Icons/saveBtn.png"); 
		openIcon = (Texture)EditorGUIUtility.Load("SceneMenuPlus/Icons/openBtn.png");
	}
		
	void OnGUI()
	{
		#region GUI Styles 
		GUIStyle titleStyle = new GUIStyle (GUI.skin.label);
		titleStyle.fontSize = 14;
		titleStyle.fontStyle = FontStyle.Bold;

		GUIStyle backgroundStyle = new GUIStyle();
		backgroundStyle.normal.background = MakeTex(1, 1, new Color(.15f, .15f, .15f, 1.0f));

		GUIStyle stackbackgroundStyle = new GUIStyle();
		stackbackgroundStyle.normal.background = MakeTex(1, 1, new Color(.25f, .25f, .25f, 1.0f));

		GUIStyle childbackgroundStyle = new GUIStyle();
		childbackgroundStyle.normal.background = MakeTex(1, 1, new Color(.2f, .2f, .2f, 1.0f));
		#endregion


		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		// TITLE BAR
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.BeginHorizontal();
	
		GUILayout.Label ("Scene Stacks", titleStyle);
	
		sceneStackWindowMenuFoldout = EditorGUI.Foldout( new Rect(position.width-28, 10, position.width - 6,18), sceneStackWindowMenuFoldout,new GUIContent("", "Expand Scene Stack Options") );
		EditorGUILayout.EndHorizontal();

		if (sceneStackWindowMenuFoldout)
		{
			EditorGUILayout.BeginHorizontal();
			if (DrawButtonFlexSize("Create Blank", "Create a new blank scene stack", 80f))
			{	
				//Add a new Scene Stack to the Scene Stack List
				numOfSceneStacks++;
				SceneStacks.Add(new SceneStack(true, "SceneStack_" + numOfSceneStacks.ToString(), 0) );
			}

			if (DrawButtonFlexSize("Create", "Create a new scene stack from open scenes", 50f))
			{	
				createSceneStackFromOpenScenes();
			}

			if (DrawButtonFlexSize("Save All", "Save the configuration of all all scene stacks", 60f)) 
			{
				//findSameLocation();
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical(backgroundStyle);
		EditorGUILayout.Space();
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


		// Start the scroll window
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);


		// Build the Scene Stack UI
		for (int i = 0; i <= numOfSceneStacks-1; i++)
		{
			buildSceneStackUI(i, stackbackgroundStyle, childbackgroundStyle);
			EditorGUILayout.Space();
		}
			

		// End the scroll view 
		EditorGUILayout.EndScrollView();
		// End Dark BG
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();



	
	}


	public void OnInspectorUpdate()
	{
		this.Repaint();
	}


	#region SCENE STACK UI
 	void buildSceneStackUI(int stackNum, GUIStyle bgstyle, GUIStyle childBgStyle)
	{


		var operationStyle = new GUIStyle("ScriptText");
		operationStyle.stretchHeight = false;
		operationStyle.padding = new RectOffset(6, 6, 4, 4);
		Rect operationRect = EditorGUILayout.BeginVertical(operationStyle);


		EditorGUILayout.EndVertical();

		var coloredHighlightRect = new Rect(operationRect);
		coloredHighlightRect.yMin += 2.0f;
		coloredHighlightRect.yMax -= 1.0f;
		coloredHighlightRect.xMin += 2.0f;
		coloredHighlightRect.width = 3.0f;
		//var oldColor = GUI.color;
		//GUI.color = this.HighlightColor;
		GUI.DrawTexture(coloredHighlightRect, Texture2D.whiteTexture);
		//GUI.color = oldColor;


		bool removedScene = false;
		EditorGUILayout.BeginVertical(bgstyle);  
		EditorGUILayout.BeginHorizontal();
	
		//!!
		//SceneStacks[stackNum].showContenseOfSceneStack = EditorGUI.Foldout( new Rect(0+stackNum*30, 10, 0, 18), SceneStacks[stackNum].showContenseOfSceneStack, new GUIContent("", "Expand Scene Stack") );
		SceneStacks[stackNum].showContenseOfSceneStack = EditorGUILayout.Foldout(SceneStacks[stackNum].showContenseOfSceneStack, "");
		//EditorGUILayout.Space();
		SceneStacks[stackNum].sceneStackName = EditorGUILayout.TextField("", SceneStacks[stackNum].sceneStackName, GUILayout.MinWidth(10) );

		EditorGUILayout.Space();
		// Save scene stack 
		bool enableSaveButton = true;
		if (!SceneStacks[stackNum].sceneStackBaseScene) enableSaveButton = false;
		if (DrawContexTextureButton(saveIcon, "", "Save Scene Stack", enableSaveButton, EditorStyles.miniButtonLeft, 25f)) 
		{
			//findSameLocation();
		}

		// Open Scene Stack
		bool enableOpenButton = true;
		if (!SceneStacks[stackNum].sceneStackBaseScene) enableOpenButton = false;
		if (DrawContexTextureButton(openIcon, "", "Open Scene Stack", enableOpenButton, EditorStyles.miniButtonRight, 25f)) 
		{
			openSceneStack(SceneStacks[stackNum]);
		}

		EditorGUILayout.Space();

		// Move Scene Stack Up
		bool enableUpbutton  = true; 
		if (stackNum == 0) enableUpbutton = false;
	
		if (DrawContexButton(moveUpIcon, "Move Scene Stack up", enableUpbutton, EditorStyles.miniButtonLeft, 25f))
		{
			if (stackNum >= 1) 
			{	
				SceneStacks.Insert(stackNum-1, SceneStacks[stackNum]);
				SceneStacks.RemoveAt(stackNum+1);
			}
		}

		enableUpbutton = true;
		if (stackNum == SceneStacks.Count-1) enableUpbutton = false;

		// Move Scene Stack Down 
		if (DrawContexButton(moveDownIcon, "Move Scene Stack down", enableUpbutton, EditorStyles.miniButtonMid, 25f))
		{
			if (stackNum < SceneStacks.Count-1) 
			{	
				SceneStacks.Insert(stackNum+2, SceneStacks[stackNum]);
				SceneStacks.RemoveAt(stackNum);
			}
		}

		enableUpbutton = true;
		// Delete Scene Stack 
		if (DrawContexButton("X", "Delete Scene Stack", enableUpbutton, EditorStyles.miniButtonRight, 25f))
		{
			SceneStacks.RemoveAt(stackNum);
			numOfSceneStacks--;
			removedScene = true;
		}
		EditorGUILayout.EndHorizontal();

		// Expand Scene Stack options 
		if (!removedScene && SceneStacks[stackNum].showContenseOfSceneStack)
		{
			// Add child scene
			EditorGUILayout.Space();
			if (DrawButtonFlexSize("Add Child Scene", "Add an additional child scene to the scene stack", 125f)) 
			{
				SceneStacks[stackNum].numChildrenScenes ++;
			}
			EditorGUILayout.Space();

			//Base Scene 
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("Base Scene:", GUILayout.MaxWidth(72));
			SceneStacks[stackNum].sceneStackBaseScene = EditorGUILayout.ObjectField("", SceneStacks[stackNum].sceneStackBaseScene, typeof(Object), false, GUILayout.MinWidth(128));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			// Children Scenes 
			if (SceneStacks[stackNum].numChildrenScenes != 0)  EditorGUILayout.BeginVertical(); 
			//EditorGUI.indentLevel++;
			for (int i = 0; i <= SceneStacks[stackNum].numChildrenScenes-1; i++)
			{
				EditorGUILayout.BeginHorizontal();
				buildChildSceneUI(i, stackNum, childBgStyle);
				EditorGUILayout.EndHorizontal();
			}
			//EditorGUI.indentLevel--;
			EditorGUILayout.Space();
			if (SceneStacks[stackNum].numChildrenScenes != 0) GUILayout.EndVertical();
		}

		EditorGUILayout.EndVertical();
	}
	#endregion

	// Child Scene UI
	void buildChildSceneUI(int sceneNum, int stackNum, GUIStyle childBgStyle)
	{
		EditorGUILayout.BeginHorizontal(GUI.skin.box); //GUI.skin.box childBgStyle
		// Add a new entry to the Set Children Active List
		SceneStacks[stackNum].setChildrenSceneActive.Add(true);
		DrawToggleSimple(SceneStacks[stackNum].setChildrenSceneActive[sceneNum], "Enable scene to be loaded", 24f);

		// Add a new scene to the children scene list 
		SceneStacks[stackNum].childrenScenes.Add(null);
		SceneStacks[stackNum].childrenScenes[sceneNum] = EditorGUILayout.ObjectField("", SceneStacks[stackNum].childrenScenes[sceneNum], typeof(Object), false, GUILayout.MinWidth(128)); 

		// Remove Child Scene 
		if (DrawButtonSimple("X", "Remove Child Scene", 18f, 14f)) 
		{
			SceneStacks[stackNum].childrenScenes.RemoveAt(sceneNum);
			SceneStacks[stackNum].numChildrenScenes --;
		}
		EditorGUILayout.EndHorizontal();
	}



	// Open scene stacks 
	void openSceneStack(SceneStack StackToOpen)
	{
		if (StackToOpen.sceneStackBaseScene != null)
		{
			// Search the root folder of the project for all unity scene files - returns the FULL path of the base scene
			string folderName = Application.dataPath + "/";
			var dirInfo = new DirectoryInfo(folderName);
			var allFileInfos = dirInfo.GetFiles(StackToOpen.sceneStackBaseScene.name + ".unity", SearchOption.AllDirectories);

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene(allFileInfos[0].ToString());
		}
		else 
		{
			EditorApplication.Beep();
			Debug.Log("No base scene assigned");
		}
 
		if (StackToOpen.numChildrenScenes > 0)
		{
			for (int i = 0; i <= StackToOpen.numChildrenScenes-1; i++)
			{
				if (StackToOpen.childrenScenes[i])
				{
					if (StackToOpen.setChildrenSceneActive[i])
					{
						// Search the root folder of the project for all unity scene files - returns the FULL path of the base scene
						string folderName = Application.dataPath + "/";
						var dirInfo = new DirectoryInfo(folderName);
						var allFileInfos = dirInfo.GetFiles(StackToOpen.childrenScenes[i].name + ".unity", SearchOption.AllDirectories);

						EditorSceneManager.OpenScene(allFileInfos[0].ToString(),OpenSceneMode.Additive);
					}
				}
				else 
				{
					EditorApplication.Beep();
					Debug.Log("No child sceen assigned to child: "+ i);
				}
			}
		}
	}






	#region Menu Shortcuts 
	[MenuItem("Assets/Scene Menu/Show Window")]
	static public void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(SceneStackTools),false, "Scene Menu Options", true);
	}
		
	[MenuItem("Assets/Scene Menu/Scene Stacks/Create New Empty Scene Stack")]
	static public void createNewEmptySceneStack()
	{
		numOfSceneStacks++;
		SceneStacks.Add(new SceneStack(true, "SceneStack_" + numOfSceneStacks.ToString(), 0) );
	}

	[MenuItem("Assets/Scene Menu/Scene Stacks/Create New Scene Stack From Open Scenes")]
	static public void createSceneStackFromOpenScenes()
	{
		// create new scene stack 
		numOfSceneStacks++;
		SceneStacks.Add(new SceneStack(true, "SceneStack_" + numOfSceneStacks.ToString(), 0) );

		//Set up base scene - based off the first scene loaded  
		SceneStacks[numOfSceneStacks-1].sceneStackBaseScene = AssetDatabase.LoadAssetAtPath(EditorSceneManager.GetSceneAt(0).path, typeof(Object) );
		//Set up number of child scenes based off total number of scenes loaded 
		SceneStacks[numOfSceneStacks-1].numChildrenScenes = EditorSceneManager.sceneCount-1;

		// Set up child scenes 
		for (int i = 0; i <= EditorSceneManager.sceneCount-2; i++)
		{
			// Add a new scene to the children scene list 
			SceneStacks[numOfSceneStacks-1].childrenScenes.Add(null);
			// Assign child scene 
			SceneStacks[numOfSceneStacks-1].childrenScenes[i] = AssetDatabase.LoadAssetAtPath(EditorSceneManager.GetSceneAt(i+1).path, typeof(Object) );

			// Add a new entry 
			SceneStacks[numOfSceneStacks-1].setChildrenSceneActive.Add(true);
			// Assign scene state - loaded or not 
			SceneStacks[numOfSceneStacks-1].setChildrenSceneActive[i] = EditorSceneManager.GetSceneAt(i+1).isLoaded;
		}
	}



	// !! saving from Menu
	// this is going to be hard.  I will have to know if I have an open scene stack already, then if its modified or changes.  
	// I will have to know where to save it ( create a new scene stack, save over an existing one)
	// if saving over ( keep the old scene stack? )






	//!! testing 
	[MenuItem("Assets/Scene Menu/MenuTesting")]
	static public void MenuTesting()
	{
		EditorSceneManager.OpenScene("/Users/kielmcdonald/Documents/WorkingPixel/UnityTools/SceneMenuPlus/SceneMenuPlusDev/Assets/Scenes/TestScene_01.unity", 
			OpenSceneMode.Additive);
		EditorSceneManager.OpenScene("/Users/kielmcdonald/Documents/WorkingPixel/UnityTools/SceneMenuPlus/SceneMenuPlusDev/Assets/Scenes/TestScene_02.unity", 
			OpenSceneMode.Additive);
		//EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		//EditorSceneManager.OpenScene( "/Users/kielmcdonald/Dropbox/Working Pixel/Projects/Unity Tools/SceneMenuTesting/Assets/newScene_01.unity" ); 
	}
	#endregion











	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	#region UI ELEMENTS 
	bool DrawContexButton (string title, string tooltip, bool enabled, GUIStyle btnStyle, float width) 
	{
		if (enabled)
		{
			// Draw a regular button
			return GUILayout.Button(new GUIContent(title, tooltip), btnStyle, GUILayout.MinWidth(width),GUILayout.MinHeight(14f) );
		}
		else
		{
			// Button should be disabled -- draw it darkened and ignore its return value
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, 0.25f);
			GUILayout.Button(new GUIContent(title, tooltip), btnStyle, GUILayout.MinWidth(width), GUILayout.MinHeight(14f) );
			GUI.color = color;
			return false;
		}
	}

	bool DrawContexTextureButton (Texture icon, string title, string tooltip, bool enabled, GUIStyle btnStyle, float width) 
	{
		// Draw a highlighted button
		if (enabled)
		{
			return GUILayout.Button(new GUIContent("", icon, tooltip), btnStyle, GUILayout.MinWidth(width),GUILayout.MinHeight(14f)  );
		}
		// Draw darkened button 
		else
		{
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, 0.25f);
			GUILayout.Button(new GUIContent("", icon, tooltip), btnStyle, GUILayout.MinWidth(width),GUILayout.MinHeight(14f) );
			GUI.color = color;
			return false;
		}
	}

	bool DrawButtonSimple (string title, string tooltip, float width, float height) 
	{
		return GUILayout.Button(new GUIContent(title, tooltip), GUILayout.MaxWidth(width),  GUILayout.MaxHeight(height) );
	}

	bool DrawButtonFlexSize (string title, string tooltip, float width) 
	{
		return GUILayout.Button(new GUIContent(title, tooltip), GUILayout.MinWidth(width),  GUILayout.MinHeight(14f) );
	}

	//!! unused ignores indents 
	bool DrawToggleSimple (bool value, string tooltip, float width)
	{
		return GUILayout.Toggle(value, new GUIContent("",tooltip), GUILayout.MaxWidth(width));
	}



	//!! this is a test for bg highlights  
	private Texture2D MakeTex(int width, int height, Color col)
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









	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
}
