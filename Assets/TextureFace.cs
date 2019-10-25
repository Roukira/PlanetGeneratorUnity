using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureFace : MonoBehaviour{

    ShapeGenerator shapeGenerator;
    public Mesh mesh;
    public GameObject meshObj;
    public int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public int faceID;
    public Planet planet;

    public Sommet[] Sommets;

    public float[] temperature;
    public Color[] colorNormal;
    public Color[] colorTemp;

    public void Initialize(ShapeGenerator shapeGenerator, GameObject meshObj, Mesh mesh, int resolution, Vector3 localUp, Planet planet, int faceID)
    {
        this.planet = planet;
        this.faceID = faceID;
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.meshObj = meshObj;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);

        temperature = new float[resolution * resolution];
        Debug.Log("creating Texture Face...");
        colorTemp = new Color[resolution * resolution];
        colorNormal = new Color[resolution * resolution];
        Sommets = new Sommet[resolution * resolution];
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);
                Sommets[i] = meshObj.AddComponent<Sommet>();
                Sommets[i].hideFlags = HideFlags.HideInInspector;
                Sommets[i].Initialize(planet, faceID, vertices[i],x,y);

                if (x!=resolution-1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();

        MeshCollider meshCollider = meshObj.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

    }

    public void calculateAll()
    {
        Vector3[] currentVertice = mesh.vertices;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                currentVertice[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);
            }
        }

        mesh.vertices = currentVertice;
    }

    
}
