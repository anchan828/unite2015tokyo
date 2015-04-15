using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[CustomPreview(typeof(AnimationClip))]
public class SpriteAnimationPreview : ObjectPreview
{
    private AnimationClipSettings animationClipSettings;
    Sprite[] sprites = new Sprite[0];

    private bool singleTarget;
    public override bool HasPreviewGUI()
    {
        return sprites.Length != 0 && singleTarget;
    }

    public override GUIContent GetPreviewTitle()
    {
        return new GUIContent("SpriteAnimation");
    }

    private SpriteTimeline timeline;

    public override void Initialize(Object[] targets)
    {
        singleTarget = targets.Length == 1;

        base.Initialize(targets);
        animationClipSettings = AnimationUtility.GetAnimationClipSettings((AnimationClip)target);

        timeline = new SpriteTimeline(animationClipSettings.startTime, animationClipSettings.stopTime);

        var clipObject = new SerializedObject(target);
        var serializedProperty = clipObject.FindProperty("m_ClipBindingConstant").FindPropertyRelative("pptrCurveMapping");

        for (var i = 0; i < serializedProperty.arraySize; i++)
        {
            var sprite = (Sprite)serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue;
            ArrayUtility.Add(ref sprites, sprite);

            // プレビュー画像生成のために呼び出しとく
            AssetPreview.GetAssetPreview(sprite);
        }
    }

    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {
        timeline.OnGUI(r);

        var currentSpriteNum = Mathf.Clamp(Mathf.FloorToInt(timeline.currentTime * 10), 0, sprites.Length - 1);

        var sprite = sprites[currentSpriteNum];
        var texture = AssetPreview.GetAssetPreview(sprite);
        var zoom = Mathf.Min(r.width / sprite.rect.width, r.height / sprite.rect.height);
        var textureRect = new Rect(r.x, r.y + 22, sprite.rect.width * zoom, sprite.rect.height * zoom);

        EditorGUI.DrawTextureTransparent(textureRect, texture);
    }
}

public class SpriteTimeline
{
    public float currentTime
    {
        get { return m_currentTime; }
        private set { m_currentTime = Mathf.Repeat(value, stopTime); }
    }

    public bool isPlaying { get; private set; }

    const int toolbarHeight = 21;
    private float deltaTime { get; set; }
    private float m_currentTime { get; set; }
    private double lastFrameEditorTime { get; set; }
    private float startTime { get; set; }
    private float stopTime { get; set; }

    private int frameRate { get; set; }

    private float m_normalizedTime;
    private float normalizedTime
    {
        get
        {
            m_normalizedTime = Mathf.Repeat(((currentTime - startTime) / (stopTime - startTime)), 1.0f);
            currentTime = startTime * (1 - m_normalizedTime) + stopTime * m_normalizedTime;
            return m_normalizedTime;
        }
    }

    public SpriteTimeline(float startTime, float stopTime, int frameRate = 12)
    {
        this.startTime = startTime;
        this.stopTime = stopTime;
        this.frameRate = frameRate;
    }

    public void OnGUI(Rect rect)
    {
        if (isPlaying)
        {
            var timeSinceStartup = EditorApplication.timeSinceStartup;
            deltaTime = (float)(timeSinceStartup - lastFrameEditorTime);
            lastFrameEditorTime = timeSinceStartup;
            currentTime += deltaTime;
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            var toolbarRect = new Rect(rect) { height = toolbarHeight };
            var buttonRect = new Rect(toolbarRect) { width = 33 };
            var lineRect = new Rect(toolbarRect);
            lineRect.x += buttonRect.width;
            isPlaying = GUI.Toggle(buttonRect, isPlaying, isPlaying ? Styles.pauseButton : Styles.playButton, Styles.timeScrubberButton);
            var xPos = Mathf.Lerp(lineRect.x, lineRect.xMax, normalizedTime);
            Handles.color = Color.red;

            for (var x = 0; x < 2; x++)
                Handles.DrawLine(new Vector2(xPos + x, lineRect.yMin), new Vector2(xPos + x, lineRect.yMax));
        }

        if (isPlaying)
        {
            foreach (var editor in ActiveEditorTracker.sharedTracker.activeEditors)
            {
                editor.Repaint();
            }
        }
    }

    private class Styles
    {
        public static GUIContent playButton = EditorGUIUtility.IconContent("PlayButton");
        public static GUIContent pauseButton = EditorGUIUtility.IconContent("PauseButton");
        public static GUIStyle timeScrubberButton = "TimeScrubberButton";
    }

}