using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomPreview(typeof(AnimationClip))]
public class SpritePreview : ObjectPreview
{
    private bool hasSprites;

    public override bool HasPreviewGUI()
    {
        return hasSprites;
    }

    public override GUIContent GetPreviewTitle()
    {
        return new GUIContent("Sprites");
    }

    public override void Initialize(Object[] targets)
    {
        base.Initialize(targets);

        var sprites = new Object[0];

        foreach (var serializedProperty in targets.Select(t => new SerializedObject(t)).Select(so => so.FindProperty("m_ClipBindingConstant").FindPropertyRelative("pptrCurveMapping")))
        {
            for (var i = 0; i < serializedProperty.arraySize; i++)
            {
                var sprite = serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue as Sprite;
                if (sprite != null)
                {
                    ArrayUtility.Add(ref sprites, sprite);

                    AssetPreview.GetAssetPreview(sprite);
                }
            }
        }

        hasSprites = sprites.Length != 0;
        m_Targets = sprites;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        var previewTexture = AssetPreview.GetAssetPreview(target);
        EditorGUI.DrawTextureTransparent(r, previewTexture);
    }

}
