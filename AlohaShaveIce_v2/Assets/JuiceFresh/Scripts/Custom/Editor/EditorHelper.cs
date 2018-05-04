using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameToolkit.Localization;

public class EditorHelper : Editor
{
	[MenuItem("Tools/Find Text And Add Localization")]
	static void SelectGameObjectsWithText ()
	{
		List<GameObject> rootObjects = new List<GameObject>();
		Scene scene = SceneManager.GetActiveScene();
		scene.GetRootGameObjects( rootObjects );

		List<Object> textList = new List<Object>();
		foreach (GameObject rootObject in rootObjects)
		{
			Transform selectedTransform = rootObject.transform;
			Component[] components = selectedTransform.GetComponentsInChildren<Component>(true);
			foreach (Component component in components)
			{
				if (component is Text)
				{
					LocalizedFontBehaviour fontBehaviour = component.gameObject.GetComponent<LocalizedFontBehaviour>();
					if (fontBehaviour == null)
					{
						fontBehaviour = component.gameObject.AddComponent<LocalizedFontBehaviour>();
					}
					LocalizedTextBehaviour textBehaviour = component.gameObject.GetComponent<LocalizedTextBehaviour>();
					if (textBehaviour == null)
					{
						textBehaviour = component.gameObject.AddComponent<LocalizedTextBehaviour>();
					}

					fontBehaviour.m_Component = component;
					fontBehaviour.m_Property = "font";
					LocalizedFont localizeAsset = AssetDatabase.LoadAssetAtPath<LocalizedFont>("Assets/JuiceFresh/Prefabs/Custom/Localization/Aloha_LocalizedFont.asset");
					fontBehaviour.LocalizedAsset = localizeAsset;
					textList.Add(component.gameObject);

					Text text = component as Text;
					text.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/JuiceFresh/Fonts/BorisBlackBloxx.ttf");
				}
			}
		}

		if (textList.Count > 0)
		{
			//Set the selection in the editor
			Debug.LogError("All Texts in the Scene = " + textList.Count);
			Selection.objects = textList.ToArray();
		}
	}
}