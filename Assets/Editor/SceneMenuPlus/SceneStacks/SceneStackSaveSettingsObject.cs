using UnityEngine;

using System.Collections;
using System.Collections.Generic;

//[CreateAssetMenuAttribute(menuName = "Scene Stacks/ Scene Stack Save Settings Object", order = 111)]
public class SceneStackSaveSettingsObject : ScriptableObject 
{

	public int numOfSceneStacks = 0;
	public List <SceneStackObject> SceneStacks = new List<SceneStackObject>();


}
