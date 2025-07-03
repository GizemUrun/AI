using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SimpleNoiseDeformer : MonoBehaviour
{
    [Header("Noise Settings")]

    //scale of noise
    [SerializeField] private float noiseScaleLarge = 1f; //0 -1
    [SerializeField] private float noiseScaleMedium = 0.5f; //0.25-0.75
    [SerializeField] private float noiseScaleSmall = 0.5f; //0.45 - 0.65

    [SerializeField] private float amplitude = 1f; //amplitude of deformation
    [SerializeField] private int seed = 0; //noise pattern seed

    private MeshFilter meshFilter;
    private Mesh originalMesh;
    private Mesh deformedMesh;
    private Vector3[] originalVertices;

    //apply deformation
    public void ApplyNoiseDeformation()
    {
        //Ensure MeshFilter assigned
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                Debug.LogError("MeshFilter is missing");
                return;
            }
        }

        //Ensure original Mesh & originalVertices assigned
        if (originalMesh == null || originalVertices == null)
        {
            if (meshFilter.sharedMesh == null)
            {
                Debug.LogError("MeshFilter's sharedMesh is missing");
                return;
            }

            //keep reference to original mesh & its vertices
            originalMesh = meshFilter.sharedMesh;
            originalVertices = originalMesh.vertices;
        }

        //keep numbers in range
        if(noiseScaleLarge > 1 || noiseScaleLarge < 0)
        {
            Debug.LogError("noiseScaleLarge is out of range, pick between 0 and 1");
        }

        if (noiseScaleMedium > 0.75 || noiseScaleMedium < 0.25)
        {
            Debug.LogError("noiseScaleMedium is out of range, pick between 0.25 and 0.75");
        }

        if (noiseScaleSmall > 0.65 || noiseScaleLarge < 0.45)
        {
            Debug.LogError("noiseScaleSmall is out of range, pick between 0.45 and 0.65");
        }

        //create new deformed mesh if doesn't exist
        if (deformedMesh == null)
        {
            deformedMesh = Instantiate(originalMesh);
            meshFilter.mesh = deformedMesh;
        }

        //clone original vertices to avoid modifying them directly
        Vector3[] vertices = (Vector3[])originalVertices.Clone();


        //generate offsets based on seed
        float offsetX = Mathf.Sin(seed) * 10000f;
        float offsetZ = Mathf.Cos(seed) * 10000f;

        //apply noise deformation to each vertex
        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];


            //sample perlin noise based on the coordinates with offsets above
            float noiseValueLarge = Mathf.PerlinNoise(
                (vertex.x + offsetX) * noiseScaleLarge,
                (vertex.z + offsetX) * noiseScaleLarge
            );

            float noiseValueMedium = Mathf.PerlinNoise(
                (vertex.x + offsetX) * noiseScaleMedium,
                (vertex.z + offsetX) * noiseScaleMedium
            );

            float noiseValueSmall = Mathf.PerlinNoise(
                (vertex.x + offsetX) * noiseScaleSmall,
                (vertex.z + offsetX) * noiseScaleSmall
                );

            //deform Y coordinates based on noiseValue & amplitude
            vertex.y += (noiseValueLarge * amplitude) + (noiseValueMedium * amplitude) + (noiseValueSmall * amplitude);
            //vertex.y += noiseValueMedium * amplitude;
            //vertex.y += noiseValueSmall * amplitude;

            vertices[i] = vertex;
        }

        //update mesh with deformed vertices
        deformedMesh.vertices = vertices;
        deformedMesh.RecalculateNormals();
        deformedMesh.RecalculateBounds();

        //mark mesh as dirty to update scene
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(meshFilter);
        UnityEditor.SceneView.RepaintAll();
#endif
    }
}
