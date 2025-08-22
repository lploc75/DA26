using UnityEngine;
using UnityEditor;

public class RenameSkills : EditorWindow
{
    [MenuItem("Tools/Rename Skills")]
    public static void ShowWindow()
    {
        GetWindow<RenameSkills>("Rename Skills");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Rename Selected Skills"))
        {
            RenameSelected();
        }
    }

    private void RenameSelected()
    {
        var selected = Selection.gameObjects;
        for (int i = 0; i < selected.Length; i++)
        {
            selected[i].name = "Skill " + (i + 1);
        }
    }
}
