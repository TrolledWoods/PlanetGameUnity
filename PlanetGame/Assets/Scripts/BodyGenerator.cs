﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(PolygonCollider2D))]
[ExecuteInEditMode]
public class BodyGenerator : MonoBehaviour {

    public float Size = 5f;
    public int Rings = 5;
    public int Slices = 6;
    
    [HideInInspector]
    public Vector3[] vertices;
    Mesh mesh;
    PolygonCollider2D polyCollider;

	// Use this for initialization
	void Awake () {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MeshFilter filter = GetComponent<MeshFilter>();
        polyCollider = GetComponent<PolygonCollider2D>();

        mesh = CreateRing(Size / Rings, Rings, Slices);

        vertices = mesh.vertices;

        filter.mesh = mesh;
    }

    void Update()
    {
        
    }
    
    Mesh CreateRing(float layerSize, int startIncrease, int layers)
    {
        // Generate the shape of the collider
        { // Make a scope for this section
            int verticesInLayer = (layers) * startIncrease - startIncrease;
            float magnitude = layerSize * (layers-1);
            float angleStep = (Mathf.PI * 2) / verticesInLayer;

            Vector2[] points = new Vector2[verticesInLayer];
            // Create all the points for the collider
            for(int i = 0; i < verticesInLayer; i++)
            {
                points[i] = new Vector2(magnitude * Mathf.Cos(-angleStep * i), magnitude * Mathf.Sin(-angleStep * i));
            }

            polyCollider.points = points;
        }

        // Create the vertices array and the uv array
        int nOfVertices = verticesInRing(startIncrease, layers)+1;
        Vector3[] vertices = new Vector3[nOfVertices];
        Vector2[] uv = new Vector2[nOfVertices];

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
                uv[verticeIndex] = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
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
        mesh.uv = uv;
        return mesh;
    }

    int verticesInRing(int startIncrease, int layers)
    {
        return startIncrease * ((layers * (layers + 1)) / 2) - startIncrease * layers;
    }
    
}
