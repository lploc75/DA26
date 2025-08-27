using DarkPixelRPGUI.Scripts.UI;
using UnityEditor;
using UnityEngine;

namespace Scripts.Inventory
{
    /// <summary>
    /// Custom Property Drawer cho kiểu NullableSerializableField.
    /// Dùng để hiển thị 1 field có thể NULL trong Inspector (Unity mặc định không hỗ trợ).
    /// </summary>
    [CustomPropertyDrawer(typeof(NullableSerializableField), true)]
    public class NullableSerializablePropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Hàm vẽ giao diện Inspector cho property.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Bắt đầu vẽ property
            EditorGUI.BeginProperty(position, label, property);

            // Lưu lại indent hiện tại và set indent = 0 để canh lề cho gọn
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Xác định vùng hiển thị label và button
            var labelPosition = new Rect(5f * 4, position.y, position.width / 2 - 5f * 4, position.height);
            var buttonPosition = new Rect(position.width / 2, position.y, position.width / 2, 20);

            // Vẽ nhãn "Nullable <tên field>"
            EditorGUI.LabelField(labelPosition, new GUIContent("Nullable " + property.displayName));

            // Lấy 2 property con trong NullableSerializableField<T>
            var hasValueProperty = property.FindPropertyRelative("hasValue"); // bool: có giá trị hay null
            var valueProperty = property.FindPropertyRelative("value");       // object: giá trị thực sự

            // Nếu hiện đang có giá trị
            if (hasValueProperty.boolValue)
            {
                // Vẽ nút "Set NULL" để xóa giá trị
                if (GUI.Button(buttonPosition, "Set NULL"))
                {
                    hasValueProperty.boolValue = false; // set thành null
                }

                // Vẽ field con để chỉnh giá trị thực sự
                var propertyPosition = new Rect(buttonPosition.x, buttonPosition.y + 20,
                                                buttonPosition.width, buttonPosition.height);
                EditorGUI.PropertyField(propertyPosition, valueProperty, true);
            }
            else // Nếu đang null
            {
                // Vẽ nút "NULL. Click to set" → khi click thì set thành có giá trị
                if (GUI.Button(buttonPosition, "NULL. Click to set"))
                {
                    hasValueProperty.boolValue = true;
                }
            }

            // Restore lại indent
            EditorGUI.indentLevel = indent;

            // Kết thúc vẽ property
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Xác định chiều cao của property trong Inspector.
        /// Nếu đang có giá trị thì phải cộng thêm chiều cao của field con.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Chiều cao mặc định 20
            var propertyHeightIndent = 20f;

            // Nếu có giá trị -> cộng thêm chiều cao field con
            if (property.FindPropertyRelative("hasValue").boolValue)
            {
                propertyHeightIndent +=
                    EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"), GUIContent.none) + 1f;
            }

            return propertyHeightIndent;
        }
    }
}
