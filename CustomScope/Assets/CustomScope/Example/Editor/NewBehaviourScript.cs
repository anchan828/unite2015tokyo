using UnityEngine;
using System.Collections;
using UnityEditor;

public class NewBehaviourScript : EditorWindow
{
    [MenuItem("Window/Example")]
    static void Open()
    {
        GetWindow<NewBehaviourScript>();
    }

    void OnGUI()
    {
        ExampleIndentScope();
        EditorGUILayout.Space();
        ExampleBackgroundColorScope();
        EditorGUILayout.Space();
    }

    void ExampleBackgroundColorScope()
    {
        GUILayout.Button("Button");

        using (new BackgroundColorScope(Color.green))
        {
            GUILayout.Button("Button");

            using (new BackgroundColorScope(Color.yellow))
            {
                GUILayout.Button("Button");
            }
        }


        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Label");
            GUILayout.Button("Button");
        }

        GUILayout.Button("Button");
    }

    void ExampleIndentScope()
    {
        using (var scope = new IndentScope(0))
        {
            EditorGUILayout.LabelField("IndentLevel 0");
        }

        using (var scope = new IndentScope(1))
        {
            EditorGUILayout.LabelField("IndentLevel 1");
        }

        using (var scope = new IndentScope(2))
        {
            EditorGUILayout.LabelField("IndentLevel 2");
        }
    }
}