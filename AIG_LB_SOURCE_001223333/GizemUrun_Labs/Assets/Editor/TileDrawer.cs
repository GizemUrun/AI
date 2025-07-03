using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Tile))]
public class TileDrawer : PropertyDrawer
{
    private const float lineHeight = 18f;
    private const float spacing = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Calculate the required height of the property drawer
        int lineCount = property.isExpanded ? 4 : 1; // Adjust based on the number of fields
        return (lineHeight + spacing) * lineCount;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the 'name' property to use as the label
        SerializedProperty nameProp = property.FindPropertyRelative("name");
        string tileName = nameProp.stringValue;
        if (string.IsNullOrEmpty(tileName))
        {
            tileName = "Unnamed Tile";
        }

        // Draw the foldout with the tile's name
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, lineHeight),
            property.isExpanded, tileName, true);

        if (!property.isExpanded)
            return;

        EditorGUI.indentLevel++;

        // Calculate positions for each field
        Rect fieldRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);

        // Draw 'name' field
        EditorGUI.PropertyField(fieldRect, nameProp);
        fieldRect.y += lineHeight + spacing;

        // Draw 'prefab' field
        SerializedProperty prefabProp = property.FindPropertyRelative("prefab");
        EditorGUI.PropertyField(fieldRect, prefabProp);
        fieldRect.y += lineHeight + spacing;

        // Draw 'sockets' field
        SerializedProperty socketsProp = property.FindPropertyRelative("sockets");
        EditorGUI.PropertyField(fieldRect, socketsProp, true);

        EditorGUI.indentLevel--;
    }
}
