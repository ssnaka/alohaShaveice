using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CollectItemsWindow : EditorWindow
{
    private InitScript initscript;

    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        CollectItemsWindow window = (CollectItemsWindow)EditorWindow.GetWindow(typeof(CollectItemsWindow));
        window.Show();
    }

    void OnFocus()
    {
        initscript = Camera.main.GetComponent<InitScript>();
    }

    void OnGUI()
    {
        GUI.changed = false;

        GUILayout.Label("Collect Items Settings", EditorStyles.boldLabel);
        GUILayout.Label("Name                    Sprite");
        foreach (CollectedIngredients item in initscript.collectedIngredients)
        {
            EditorGUILayout.BeginHorizontal();
            item.name = EditorGUILayout.TextField("", item.name, new GUILayoutOption[] {
                GUILayout.Width (100) });
            item.sprite = (Sprite)EditorGUILayout.ObjectField(item.sprite, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            CollectedIngredients ingr = new CollectedIngredients();
            ingr.name = "Ingredient" + (initscript.collectedIngredients.Count + 1);
            initscript.collectedIngredients.Add(ingr);
            GUI.changed = true;

        }
        if (GUILayout.Button("Delete"))
        {
            if (initscript.collectedIngredients.Count > 0)
                initscript.collectedIngredients.Remove(initscript.collectedIngredients[initscript.collectedIngredients.Count - 1]);
            GUI.changed = true;

        }

        if (GUI.changed && !EditorApplication.isPlaying)
            EditorSceneManager.MarkAllScenesDirty();


    }

}
