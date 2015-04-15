using UnityEngine;
using UnityEditor;
public class IndentScope : GUI.Scope
{
    readonly int _indentLevel;

    public IndentScope(int indentLevel)
    {
        _indentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = indentLevel;
    }

    protected override void CloseScope()
    {
        EditorGUI.indentLevel = _indentLevel;
    }
}