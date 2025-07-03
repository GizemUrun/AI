using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    private SerializedProperty tilesProperty;
    private SerializedProperty gridWidthXProperty;
    private SerializedProperty gridWidthZProperty;
    private SerializedProperty tileSizeProperty;
    private SerializedProperty socketCompatibilityMatrixProperty;
    private string[] socketTypeNames;

    void OnEnable()
    {
        tilesProperty = serializedObject.FindProperty("tiles");
        gridWidthXProperty = serializedObject.FindProperty("gridWidthX");
        gridWidthZProperty = serializedObject.FindProperty("gridWidthZ");
        tileSizeProperty = serializedObject.FindProperty("tileSize");
        socketCompatibilityMatrixProperty = serializedObject.FindProperty("socketCompatibilityMatrix");

        socketTypeNames = System.Enum.GetNames(typeof(SocketType));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Grid properties
        EditorGUILayout.PropertyField(gridWidthXProperty);
        EditorGUILayout.PropertyField(gridWidthZProperty);
        EditorGUILayout.PropertyField(tileSizeProperty);

        EditorGUILayout.Space();

        // Tiles list and weights
        EditorGUILayout.LabelField("Tiles", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(tilesProperty, true); // Show all tiles

        // Iterate over each tile and show weight
        for (int i = 0; i < tilesProperty.arraySize; i++)
        {
            SerializedProperty tileProperty = tilesProperty.GetArrayElementAtIndex(i);
            SerializedProperty weightProperty = tileProperty.FindPropertyRelative("weight");

            // Draw tile name and weight
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(tileProperty.FindPropertyRelative("name"), GUIContent.none);
            EditorGUILayout.PropertyField(weightProperty, new GUIContent("Weight"));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        // Socket Compatibility Matrix section
        EditorGUILayout.LabelField("Socket Compatibility Matrix", EditorStyles.boldLabel);

        DungeonGenerator generator = (DungeonGenerator)target;

        int socketTypeCount = socketTypeNames.Length;

        if (generator.socketCompatibilityMatrix == null || generator.socketCompatibilityMatrix.GetLength(0) != socketTypeCount)
        {
            generator.InitializeCompatibilityMatrix();
        }

        // Begin compatibility matrix
        EditorGUILayout.BeginHorizontal();

        // Top-left empty cell
        EditorGUILayout.LabelField("", GUILayout.Width(100));

        // Column headers
        for (int j = 0; j < socketTypeCount; j++)
        {
            EditorGUILayout.LabelField(socketTypeNames[j], GUILayout.Width(80));
        }
        EditorGUILayout.EndHorizontal();

        // Rows
        for (int i = 0; i < socketTypeCount; i++)
        {
            EditorGUILayout.BeginHorizontal();
            // Row header
            EditorGUILayout.LabelField(socketTypeNames[i], GUILayout.Width(100));

            for (int j = 0; j < socketTypeCount; j++)
            {
                bool value = generator.socketCompatibilityMatrix[i, j];
                value = EditorGUILayout.Toggle(value, GUILayout.Width(80));
                generator.socketCompatibilityMatrix[i, j] = value;
            }
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(generator);
        }
    }
}