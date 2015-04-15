using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using AnimatorController = UnityEditor.Animations.AnimatorController;


/// <summary>
/// Animator Override Controller を一括で作成する
/// </summary>
public class AnimatorOverrideControllerCreator : ScriptableWizard
{

    [SerializeField]
    private RuntimeAnimatorController animatorController;

    Dictionary<AnimatorOverrideController, ReorderableList> dic = new Dictionary<AnimatorOverrideController, ReorderableList>();

    [MenuItem("Window/AnimatorOverrideControllerCreator")]
    static void Open()
    {
        GetWindow<AnimatorOverrideControllerCreator>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns> true だと OnWizardUpdate が呼び出される</returns>
    protected override bool DrawWizardGUI()
    {
        EditorGUI.BeginChangeCheck();

        animatorController = EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(AnimatorController), false) as RuntimeAnimatorController;

        if (animatorController == null)
            return true;

        if (EditorGUI.EndChangeCheck())
            dic.Clear();
        
        if (GUILayout.Button("Add Override Controlelr"))
        {
            AddAnimatorOverrideControllerList();
        }

        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(position.width * 0.1f);
            
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(position.width * 0.8f)))
            {
                foreach (var reorderableList in dic.Values)
                {
                    reorderableList.DoLayoutList();
                }
            }
        }
        return true;
    }

    void AddAnimatorOverrideControllerList()
    {
        var animatorOverrideController = new AnimatorOverrideController
        {
            name = "New Animator Override Controller " + (dic.Count + 1),
            runtimeAnimatorController = animatorController
        };

        var list = new ReorderableList(animatorOverrideController.clips,
            typeof(AnimationClipPair), 
            false,
            false,
            false,
            false)
        {
            elementHeight = 16
        };

        list.drawElementCallback += (rect, index, active, focused) =>
        {
            var pair = animatorOverrideController.clips[index];
            
            EditorGUI.BeginChangeCheck();
            pair.overrideClip =
                EditorGUI.ObjectField(rect, pair.originalClip.name, pair.overrideClip, typeof(AnimationClip), false) as
                    AnimationClip;
            
            if (EditorGUI.EndChangeCheck())
            {
                var _clips = animatorOverrideController.clips;
                _clips[index] = pair;
                animatorOverrideController.clips = _clips;
            }
        };

        list.drawHeaderCallback += rect =>
        {
            animatorOverrideController.name = EditorGUI.TextField(new Rect(rect.x, rect.y + 2, 200, rect.height), animatorOverrideController.name, EditorStyles.miniTextField);

            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, 20), EditorGUIUtility.IconContent("Toolbar Minus"), "label"))
            {
                // InvalidOperationException 回避
                // delayCall はインスペクターのアップデート後（描画周りも含む）に実行されるためInvalidOperationExceptionは発生しない
                EditorApplication.delayCall += () =>
                {
                    dic.Remove(animatorOverrideController);
                    Repaint();
                };
            }
        };

        dic.Add(animatorOverrideController, list);
    }

    void OnWizardUpdate()
    {
        isValid = dic.Count != 0;
    }

    void OnWizardCreate()
    {
        const string path = "Assets/OverrideControllers";
        Directory.CreateDirectory(path);

        foreach (var animatorOverrideController in dic.Keys)
        {
            AssetDatabase.CreateAsset(animatorOverrideController,
                AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, animatorOverrideController.name + ".overrideController")));
        }
    }
}