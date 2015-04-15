using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEditor;

public abstract class OverrideEditor : Editor
{
    readonly MethodInfo methodInfo = typeof(Editor).GetMethod("OnHeaderGUI", BindingFlags.NonPublic | BindingFlags.Instance);

    private Editor m_BaseEditor;
    protected Editor baseEditor
    {
        get { return m_BaseEditor ?? (m_BaseEditor = GetBaseEditor()); }
        set { m_BaseEditor = value; }
    }

    protected abstract Editor GetBaseEditor();


    public override void OnInspectorGUI()
    {
        baseEditor.OnInspectorGUI();
    }

    public override void DrawPreview(Rect previewArea)
    {
        baseEditor.DrawPreview(previewArea);
    }

    public override string GetInfoString()
    {
        return baseEditor.GetInfoString();
    }

    public override void OnPreviewSettings()
    {
        baseEditor.OnPreviewSettings();
    }

    public override void ReloadPreviewInstances()
    {
        baseEditor.ReloadPreviewInstances();
    }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        return baseEditor.RenderStaticPreview(assetPath, subAssets, width, height);
    }
   
    protected override void OnHeaderGUI()
    {
        methodInfo.Invoke(baseEditor, new object[0]);
    }

    public override bool RequiresConstantRepaint()
    {
        return baseEditor.RequiresConstantRepaint();
    }

    public override bool UseDefaultMargins()
    {
        return baseEditor.UseDefaultMargins();
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        baseEditor.OnPreviewGUI(r, background);
    }

    public override GUIContent GetPreviewTitle()
    {
        return baseEditor.GetPreviewTitle();
    }

    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {
        baseEditor.OnInteractivePreviewGUI(r, background);
    }
}
