using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

//using UnityEditor.SceneManagement;

//[CreateAssetMenuAttribute(fileName = "SceneStack_", menuName = "Scene Stacks/ New Scene Stack", order = 110)]
public class SceneStackObject : ScriptableObject 
{
	public string sceneStackName = "SceneStack_00";
	public bool showContenseOfSceneStack;
	public int numChildrenScenes;
	public SceneAsset sceneStackBaseScene;
	public List <bool> setChildrenSceneActive = new List <bool>();
	public List <SceneAsset> childrenScenes = new List <SceneAsset>();

	/*
	public SceneStack(bool show, string name, int children)
	{
		showContenseOfSceneStack = show;
		sceneStackName = name;
		numChildrenScenes = children;
	}
	*/


}
