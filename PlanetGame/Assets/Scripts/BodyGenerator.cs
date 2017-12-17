using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class BodyGenerator : MonoBehaviour {

    public float Size = 5f;
    public int Rings = 5;
    public int Slices = 6;

    public Vector3[] vertices;
    Mesh mesh;

	// Use this for initialization
	void Awake () {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MeshFilter filter = GetComponent<MeshFilter>();

        mesh = CreateRing(Size / Rings, Rings, Slices);

        vertices = mesh.vertices;

        filter.mesh = mesh;
        renderer.material.color = Color.cyan;
    }

    void Update()
    {
        
    }
    
    Mesh CreateRing(float layerSize, int startIncrease, int layers)
    {
        // Create the vertices array
        int nOfVertices = verticesInRing(startIncrease, layers)+1;
        Vector3[] vertices = new Vector3[nOfVertices];

        int verticeIndex = 0;

        // Generate the vertices
        for(int l = 0; l < layers; l++)
        {
            // Calculate the number of vertices in the layer, the radius of the polygon that the vertices are part of, 
            // and the angle between vertices
            int verticesInLayer = l == 0 ? 1 : (l+1) * startIncrease - startIncrease;
            float magnitude = layerSize * l;
            float angleStep = (Mathf.PI * 2) / verticesInLayer;
            
            // Create all the vertices
            for(int i = 0; i < verticesInLayer; i++)
            {
                vertices[verticeIndex] = new Vector3(magnitude * Mathf.Cos(-angleStep * i), magnitude * Mathf.Sin(-angleStep * i));
                verticeIndex++;
            }
        }

        // Create the array of triangles
        int nOfTriangles = startIncrease * (((layers-1)*(layers)/2) + ((layers - 2) * (layers-1) / 2));
        int[] triangles = new int[nOfTriangles*3];

        int triangleIndex = 0;
        verticeIndex = 0;

        // Generate the triangles
        for (int l = 0; l < layers; l++)
        {
            // Find some useful properties about the layer
            int verticesInLayer = l == 0 ? 1 : (l + 1) * startIncrease - startIncrease;
            int verticesInRingUnderCurrentLayer = verticesInRing(startIncrease, l+1);
            int verticesInRingUnderLayer = verticesInRing(startIncrease, l);
            int verticesInSmallerRingUnderLayer = verticesInRing(startIncrease, l - 1);
            
            if (verticesInLayer > 2)
            {
                for (int i = 0; i < verticesInLayer; i++)
                {
                    if (l < layers - 1)
                    {
                        // Make a triangle pointing outwards
                        int nextI = verticesInRingUnderCurrentLayer + i + 2 + Mathf.FloorToInt(i*1.0f / (l));

                        int a = verticeIndex;
                        int b = i == verticesInLayer - 1 ? verticesInRingUnderLayer + 1 : verticeIndex + 1;
                        int c = nextI;
                        triangles[triangleIndex * 3 + 0] = c;
                        triangles[triangleIndex * 3 + 1] = b;
                        triangles[triangleIndex * 3 + 2] = a;
                        triangleIndex++;
                    }
                    if(l > 0)
                    {
                        // Make a triangle pointing inwards
                        int nextI = verticesInSmallerRingUnderLayer + 1 + i - Mathf.FloorToInt(i*1.0f / l*1.0f);
                        nextI = i == verticesInLayer - 1 ? verticesInSmallerRingUnderLayer + 1 : nextI;

                        // If we are on the first layer, just default to vertice zero
                        nextI = l == 1 ? 0 : nextI;

                        int a = verticeIndex;
                        int b = i >= verticesInLayer - 1 ? verticesInRingUnderLayer + 1 : verticeIndex + 1;
                        int c = nextI;
                        triangles[triangleIndex * 3 + 0] = a;
                        triangles[triangleIndex * 3 + 1] = b;
                        triangles[triangleIndex * 3 + 2] = c;
                        triangleIndex++;
                    }
                    verticeIndex++;
                }
            }
            else
            {
                verticeIndex += verticesInLayer;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }

    int verticesInRing(int startIncrease, int layers)
    {
        return startIncrease * ((layers * (layers + 1)) / 2) - startIncrease * layers;
    }
    
}
