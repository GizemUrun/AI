using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleNoiseDeformer))]
public class SimpleNoiseDeformerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //reference target script
        SimpleNoiseDeformer deformer = (SimpleNoiseDeformer)target;

        //add space before button
        EditorGUILayout.Space();

        //add button
        if(GUILayout.Button("Apply Noise Deformer"))
        {
            //call method
            deformer.ApplyNoiseDeformation();
            //mark object as dirty to ensure scene updates
            EditorUtility.SetDirty(deformer);
        }

        //apply deformation automatically when parameters change
        if (GUI.changed)
        {
            deformer.ApplyNoiseDeformation();
        }

    }

}
