using UnityEngine;
using System.Linq;
using UnityEditor;

/// <summary>
///  Sprite Animation をエディターAPIを使って自作する
/// Unity5.1では「Assets/Create/Animation」で同じことができる
/// </summary>
public class SpriteAnimation
{
    [MenuItem("Assets/Create/Sprite Animation")]
    static void CreateSpriteAnimationMenu()
    {
        var sprites = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets)
            .Select(tex => AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tex)))
            .SelectMany(x => x)
            .Where(t => t is Sprite)
            .Cast<Sprite>()
            .OrderBy(s => s.name)
            .ToArray();

        var animationClip = CreateSpriteAnimation(sprites);
        ProjectWindowUtil.CreateAsset(animationClip, Selection.activeObject.name + ".anim");
    }


    static AnimationClip CreateSpriteAnimation(params Sprite[] sprites)
    {
        var animationClip = new AnimationClip
        {
            frameRate = 12
        };

        var animationClipSettings = new AnimationClipSettings
        {
            loopTime = true
        };

        AnimationUtility.SetAnimationClipSettings(animationClip, animationClipSettings);

        var objectReferenceKeyframes = new ObjectReferenceKeyframe[sprites.Length];

        for (var i = 0; i < objectReferenceKeyframes.Length; i++)
        {
            objectReferenceKeyframes[i] = new ObjectReferenceKeyframe
            {
                value = sprites[i],
                time = i / animationClip.frameRate
            };
        }

        var editorCurveBinding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");

        AnimationUtility.SetObjectReferenceCurve(animationClip, editorCurveBinding, objectReferenceKeyframes);

        return animationClip;
    }
}
