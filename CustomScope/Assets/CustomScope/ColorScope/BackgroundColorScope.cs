using UnityEngine;

public class BackgroundColorScope : GUI.Scope
{
    private readonly Color color;
    public BackgroundColorScope(Color color)
    {
        this.color = GUI.backgroundColor;
        GUI.backgroundColor = color;
    }
    protected override void CloseScope()
    {
        GUI.backgroundColor = color;
    }
}