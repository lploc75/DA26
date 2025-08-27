// Assets/Editor/RenameAttributesWithChild.cs
using UnityEngine;
using UnityEditor;
using System.Linq;
using TMPro;

public class RenameAttributesWithChild : EditorWindow
{
    int startIndex = 1;
    bool orderByHierarchy = true; // bỏ chọn để sắp theo vị trí Y (trên xuống)

    [MenuItem("Tools/Rename/Attributes + TextChild")]
    static void Open() => GetWindow<RenameAttributesWithChild>("Rename Attr + Child");

    void OnGUI()
    {
        EditorGUILayout.LabelField("Rename selected parents", EditorStyles.boldLabel);
        startIndex = EditorGUILayout.IntField("Start Index", startIndex);
        orderByHierarchy = EditorGUILayout.ToggleLeft("Order by Hierarchy (else: by posY desc)", orderByHierarchy);

        if (GUILayout.Button("Rename"))
            DoRename();
    }

    void DoRename()
    {
        var parents = Selection.transforms;
        if (parents == null || parents.Length == 0)
        {
            EditorUtility.DisplayDialog("Rename", "Hãy chọn các parent (Attribute) trong Hierarchy.", "OK");
            return;
        }

        // Sắp xếp theo nhu cầu
        var ordered = orderByHierarchy
            ? parents.OrderBy(t => t.GetSiblingIndex()).ToArray()
            : parents.OrderByDescending(t => t.position.y).ToArray();

        Undo.RegisterCompleteObjectUndo(ordered, "Rename Attributes + Child");

        int idx = startIndex;
        foreach (var p in ordered)
        {
            // Đổi tên parent: "Attribute {i}"
            p.gameObject.name = $"Attribute {idx}";
            EditorUtility.SetDirty(p.gameObject);

            // Tìm child TMP ưu tiên; nếu không có TMP thì lấy child đầu tiên (nếu có)
            GameObject childGo = p.GetComponentsInChildren<TextMeshProUGUI>(true)
                                  .Select(tmp => tmp.gameObject)
                                  .FirstOrDefault(go => go.transform.parent == p);

            if (childGo == null && p.childCount > 0)
                childGo = p.GetChild(0).gameObject;

            if (childGo != null)
            {
                childGo.name = $"TextAttribute{idx}";
                EditorUtility.SetDirty(childGo);
            }

            idx++;
        }

        EditorUtility.DisplayDialog("Rename", $"Đã đổi tên {ordered.Length} parent + child.", "OK");
    }
}
