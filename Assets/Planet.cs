using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {


    [Range(2,256)]
    public int resolution = 20;

    public Material planetMaterial;
    public Material waterMaterial;
    public Material atmosphereMaterial;

    public Color planetColor;

    public ShapeGenerator shapeGenerator;

    public MeshFilter[] meshFilters;
    public TextureFace[] textureFaces;

    public GameObject WaterSphere;
    public GameObject Atmosphere;
    int atmosphereCounter = 0;

    public SimpleNoiseFilter atmosphereNoiseFilter;

    public int verticeMaxFaceIndex = 0;
    public int verticeMaxIndex = 0;
    public int verticeMinFaceIndex = 0;
    public int verticeMinIndex = 0;

    public float waterLevel = 0;
    public float atmosphereLevel = 0;
    bool waterSphereCreated = false;
    bool atmosphereCreated = false;

    public TempManager tempManager;


    private void Update()
    {
        atmosphereCounter++;
        if (atmosphereCounter == 10)
        {
            CalculateAtmosphere();
            atmosphereCounter = 0;
        }
    }

    public void Initialize()
    {
        planetColor = new Color(30f / 255f, 147f / 255f, 45f / 255f);

        MainGame game = GameObject.Find("MainGameLoop").GetComponent<MainGame>();
        planetMaterial = game.materialManager.planetMaterial;
        waterMaterial = game.materialManager.waterMaterial;
        atmosphereMaterial = game.materialManager.atmosphereMaterial;

        shapeGenerator = transform.gameObject.AddComponent<ShapeGenerator>();
        shapeGenerator.Initiliaze();
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        textureFaces = new TextureFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            GameObject meshObj = new GameObject("face "+i);
            
            meshObj.transform.parent = transform;
                
            meshObj.AddComponent<MeshRenderer>();
            meshFilters[i] = meshObj.AddComponent<MeshFilter>();

            meshFilters[i].sharedMesh = new Mesh();
            

            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = planetMaterial;

            textureFaces[i] = meshObj.AddComponent<TextureFace>();
            textureFaces[i].Initialize(shapeGenerator, meshObj,meshFilters[i].sharedMesh, resolution, directions[i], this, i);

        }
        GeneratePlanet();
    }

    public void RecalculateVectors()
    {
        for (int i = 0; i < 6; i++)
        {
            textureFaces[i].calculateAll();
            CalculateWaterLevel();
            float multiplier = 1 / shapeGenerator.planetRadius;
            WaterSphere.transform.localScale = new Vector3(multiplier * waterLevel, multiplier * waterLevel, multiplier * waterLevel);
            Atmosphere.transform.localScale = new Vector3(multiplier * atmosphereLevel, multiplier * atmosphereLevel, multiplier * atmosphereLevel);
        }
        ColoratePlanet();
    }

    public void ElevateSlot(int triIndex, RaycastHit hit, float ratio, int size)
    {
        int index = int.Parse(hit.transform.name.Replace("face ",""));

        Debug.Log(index);

        int pos = textureFaces[index].mesh.triangles[triIndex * 3];

        Sommet s = textureFaces[index].Sommets[pos];
        Terraform.Elevate(s, ratio);
        foreach (Sommet s2 in s.pointsAdjacents)
        {
            Terraform.Elevate(s2, (1+ratio)/2);
        }


    }

    public void GeneratePlanet()
    {
        GenerateMesh();
        GenerateColors();
        UpdateMaxMinVertices();
        //monterBords();
        //LisserHauteur();
        //LisserHauteur();
        GenerateWaterSphere();

        GenerateTemperature();
        GenerateAtmosphere();

        setPointsAdjacents();
        checkCoastSommets();
        Verdurize();

        ColoratePlanet();

        GenerateSommetWaterLevel();
    }

    void GenerateMesh()
    {
        foreach (TextureFace face in textureFaces)
        {
            face.ConstructMesh();
        }
    }

    void GenerateColors()
    {
        /*foreach (MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.planetColour;
        }*/
        Debug.Log("test");
        if (tempManager != null && tempManager.showTemp)
        {
            Debug.Log("Temp Mode");
            for (int i = 0; i < 6; i++)
            {
                textureFaces[i].mesh.colors = textureFaces[i].colorTemp;
            }
        }
        else
        {
            Debug.Log("Normal Mode");
            for (int i = 0; i < 6; i++)
            {
                textureFaces[i].mesh.colors = textureFaces[i].colorNormal;
            }
        }
    }

    void GenerateSommetWaterLevel()
    {
        for (int i = 0; i < 6; i++)
        {
            Debug.Log("point");
            foreach (Sommet s2 in textureFaces[i].Sommets)
            {
                s2.searchClosestCoast();
            }
        }

            
    }

    void GenerateWaterSphere()
    {
        if (waterSphereCreated) DestroyImmediate(WaterSphere);
        waterSphereCreated = true;
        CalculateWaterLevel();
        WaterSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        WaterSphere.transform.position = meshFilters[0].transform.position;
        WaterSphere.name = "Water Sphere";
        WaterSphere.transform.parent = transform;
        float multiplier = 1 / shapeGenerator.planetRadius;
        WaterSphere.transform.localScale = new Vector3(multiplier * waterLevel, multiplier * waterLevel, multiplier * waterLevel);
        WaterSphere.GetComponent<MeshRenderer>().sharedMaterial = waterMaterial;
        WaterSphere.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        WaterSphere.GetComponent<Collider>().enabled = false;
    }

    void GenerateTemperature()
    {
        tempManager = transform.gameObject.AddComponent<TempManager>();
        tempManager.Initialize(this);
    }

    void GenerateAtmosphere()
    {
        if (atmosphereCreated) DestroyImmediate(Atmosphere);
        atmosphereCreated = true;
        Atmosphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Atmosphere.transform.position = meshFilters[0].transform.position;
        Atmosphere.name = "Atmosphere";
        Atmosphere.transform.parent = transform;
        float multiplier = 1 / shapeGenerator.planetRadius;
        Atmosphere.transform.localScale = new Vector3(multiplier * atmosphereLevel, multiplier * atmosphereLevel, multiplier * atmosphereLevel);
        atmosphereNoiseFilter = Atmosphere.AddComponent<SimpleNoiseFilter>();
        atmosphereNoiseFilter.centre.x = 0.1f;
        atmosphereNoiseFilter.centre.y = 0.1f;
        atmosphereNoiseFilter.centre.z = 0.1f;

        Atmosphere.GetComponent<MeshRenderer>().sharedMaterial = atmosphereMaterial;

        CalculateAtmosphere();

        Atmosphere.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        Atmosphere.GetComponent<Collider>().enabled = false;

    }

    public void CalculateAtmosphere()
    {
        atmosphereNoiseFilter.baseRoughness = 1.1f;
        atmosphereNoiseFilter.strength = 0.2f;
        atmosphereNoiseFilter.roughness = 5;
        atmosphereNoiseFilter.persistence = 0.2f;

        float rand = Random.value/50 + 1;
        atmosphereNoiseFilter.centre.x *= rand;
        atmosphereNoiseFilter.centre.y *= rand;
        atmosphereNoiseFilter.centre.z *= rand;

        Color[] colors = new Color[Atmosphere.GetComponent<MeshFilter>().sharedMesh.vertices.Length];
        for (int i = 0; i < Atmosphere.GetComponent<MeshFilter>().sharedMesh.vertices.Length; i++)
        {
            /*if (atmosphereNoiseFilter.Evaluate(Atmosphere.GetComponent<MeshFilter>().sharedMesh.vertices[i]) >= 0.15) colors[i] = new Color(1,1,1,1f);
            else colors[i] = new Color(0, 0, 0, 0);*/
            float value = Mathf.Abs(atmosphereNoiseFilter.Evaluate(Atmosphere.GetComponent<MeshFilter>().sharedMesh.vertices[i]));
            if (value >= 0.08 && value <= 0.15) colors[i] = new Color(1, 1, 1, 0.6f);
            else colors[i] = new Color(0, 0, 0, 0);
        }

        Atmosphere.GetComponent<MeshFilter>().sharedMesh.colors = colors;
    }

    public float getTemp(int faceID, int j)
    {
        return tempManager.getTemp(faceID, j);
    }

    public static float distance(Vector3 x1, Vector3 x2)
    {
        return hauteurPoint(x1 - x2);
    }

    public void CalculateWaterLevel()
    {
        waterLevel = 0;
        for (int i = 0; i < 6; i++)
        {
            Vector3[] currentVertice = textureFaces[i].mesh.vertices;
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    int j = x + y * resolution;

                    waterLevel += hauteurPoint(currentVertice[j]);
                }
            }
        }
        waterLevel /= resolution * resolution * 6;
        atmosphereLevel = 1.2f * waterLevel;
    }

    public void ColoratePlanet()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3[] currentVertice = textureFaces[i].mesh.vertices;
            Color[] colors = new Color[currentVertice.Length];

            Color coastColor = new Color(planetColor.r * 255f / 30f, planetColor.g * 208f / 144f, planetColor.b * 5f / 45f);
            Color hillColor = new Color(planetColor.r / 2, planetColor.g / 1.5f, planetColor.b / 2);
            Color mountainColor = new Color(planetColor.r * 2, planetColor.g / 2, planetColor.b / 3);
            Color topMountainColor = new Color(planetColor.r * 2, planetColor.g / 3, planetColor.b / 1.2f);

            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    int j = x + y * resolution;

                    if (hauteurPoint(currentVertice[j]) <= 1.01 * waterLevel)
                    {
                        colors[j] = coastColor;
                    }
                    else if (hauteurPoint(currentVertice[j]) <= 1.05 * waterLevel)
                    {
                        colors[j] = planetColor;
                    }
                    else if (hauteurPoint(currentVertice[j]) <= 1.09 * waterLevel)
                    {
                        colors[j] = hillColor;
                    }
                    else if (hauteurPoint(currentVertice[j]) <= 1.12* waterLevel)
                    {
                        colors[j] = mountainColor;
                    }
                    else
                    {
                        colors[j] = topMountainColor;
                    }
                    textureFaces[i].colorNormal[j] = colors[j];
                }
            }
            textureFaces[i].colorNormal = colors;
            textureFaces[i].mesh.colors = colors;
        }
    }

    public void Verdurize()
    {
        for (int i = 0; i < 6; i++)
        {
            Sommet[] currentSommets = textureFaces[i].Sommets;
            
                        
            /*textureFaces[3].colorNormal[0] = Color.magenta;
            for (int j = 0; j < 8; j++)
            {
                Sommet sommet = textureFaces[3].Sommets[0].pointsAdjacents[j];
                int x = sommet.x;
                int y = sommet.y;
                textureFaces[sommet.faceID].colorNormal[x + resolution * y] = Color.magenta;
            }
            textureFaces[i].mesh.colors = textureFaces[i].colorNormal;*/
        }
    }

    public float getRadius()
    {
        return shapeGenerator.planetRadius;
    }

    public void setPointsAdjacents()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3[] currentVertice = textureFaces[i].mesh.vertices;
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    int j = x + y * resolution;
                    textureFaces[i].Sommets[j].updatePointsAdjacents();
                }
            }
        }
    }

    public void checkCoastSommets()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3[] currentVertice = textureFaces[i].mesh.vertices;
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    int j = x + y * resolution;
                    textureFaces[i].Sommets[j].cote = textureFaces[i].Sommets[j].isCoast();
                    //if (textureFaces[i].Sommets[j].cote) textureFaces[i].colorNormal[j] = Color.magenta;
                }
            }
            //textureFaces[i].mesh.colors = textureFaces[i].colorNormal;
        }
    }

    public void UpdateMaxMinVertices()
    {
        for (int i = 0; i < 6; i++){
            Vector3[] currentVertice = textureFaces[i].mesh.vertices;
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    int j = x + y * resolution;

                    if (hauteurPoint(currentVertice[j]) >= hauteurPoint(textureFaces[verticeMaxFaceIndex].mesh.vertices[verticeMaxIndex]))
                    {

                        verticeMaxFaceIndex = i;
                        verticeMaxIndex = j;
                    }
                    else if (hauteurPoint(currentVertice[j]) <= hauteurPoint(textureFaces[verticeMinFaceIndex].mesh.vertices[verticeMinIndex]))
                    {
                        verticeMinFaceIndex = i;
                        verticeMinIndex = j;
                    }
                }
            }
        }
        
    }

    public static float hauteurPoint(Vector3 point)
    {
        return Mathf.Sqrt(point.x * point.x + point.y * point.y + point.z * point.z);
    }

    public void monterBords()
    {
        Debug.Log("debug");

        for (int i = 0; i < 6; i++)
        {
            Vector3[] currentVertice = textureFaces[i].mesh.vertices;
            int[] currentTriangle = textureFaces[i].mesh.triangles;
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    int j = x + y * resolution;
                    float currentHeight = hauteurPoint(currentVertice[j]);
                    
                    float newHeight = currentHeight;

                    if (x == 0 && y == 0)
                        {
                            newHeight *= 2;
                        }
                        else if (x == resolution - 1 && y == resolution - 1)
                        {
                            newHeight *= 2;
                        }
                        else if (x == 0 && y == resolution - 1)
                        {
                            newHeight *= 2;
                        }
                        else if (x == resolution - 1 && y == 0)
                        {
                            newHeight *= 2;
                        }
                        else if (x == 0)
                        {
                            newHeight *= 2;
                        }
                        else if (y == 0)
                        {
                            newHeight *= 2;
                        }
                        else if (x == resolution - 1)
                        {
                            newHeight *= 2;
                        }
                        else if (y == resolution - 1)
                        {
                            newHeight *= 2;
                        }
                        currentVertice[j] *= newHeight / currentHeight;

                }
            }
            Debug.Log("Face " + i);
            textureFaces[i].mesh.vertices = currentVertice;
        }
    }

    public void LisserHauteur()
    {
        Debug.Log("debug");

        for (int i = 0; i < 6; i++)
        {
            Vector3[] currentVertice = textureFaces[i].mesh.vertices;
            int[] currentTriangle = textureFaces[i].mesh.triangles;
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    int j = x + y * resolution;
                    float currentHeight = hauteurPoint(currentVertice[j]);
                    if (currentHeight < hauteurPoint(textureFaces[verticeMaxFaceIndex].mesh.vertices[verticeMaxIndex]) && currentHeight > hauteurPoint(textureFaces[verticeMinFaceIndex].mesh.vertices[verticeMinIndex]))
                    {

                        float newHeight;

                        if (x == 0 && y == 0)
                        {
                            newHeight = ((hauteurPoint(currentVertice[j]) + hauteurPoint(currentVertice[(x + 1)]) + hauteurPoint(currentVertice[(x + 1) + (y + 1) * resolution]) + hauteurPoint(currentVertice[(x) + (y + 1) * resolution])) / 4);
                        }
                        else if (x == resolution - 1 && y == resolution - 1)
                        {
                            newHeight = ((hauteurPoint(currentVertice[j]) + hauteurPoint(currentVertice[(x - 1) + y * resolution]) + hauteurPoint(currentVertice[(x - 1) + (y - 1) * resolution]) + hauteurPoint(currentVertice[(x) + (y - 1) * resolution])) / 4);
                        }
                        else if (x == 0 && y == resolution - 1)
                        {
                            newHeight = ((hauteurPoint(currentVertice[j]) + hauteurPoint(currentVertice[((y - 1) * resolution)]) + hauteurPoint(currentVertice[(1) + (y - 1) * resolution]) + hauteurPoint(currentVertice[(1) + (y) * resolution])) / 4);
                        }
                        else if (x == resolution - 1 && y == 0)
                        {
                            newHeight = ((hauteurPoint(currentVertice[j]) + hauteurPoint(currentVertice[((x) + (1) * resolution)]) + hauteurPoint(currentVertice[(x - 1) + (1) * resolution]) + hauteurPoint(currentVertice[(x - 1)])) / 4);
                        }
                        else if (x == 0)
                        {
                            newHeight = ((hauteurPoint(currentVertice[j]) + hauteurPoint(currentVertice[((y - 1) * resolution)]) + hauteurPoint(currentVertice[(y + 1) * resolution]) + hauteurPoint(currentVertice[(1) + (y - 1) * resolution]) + hauteurPoint(currentVertice[(1) + (y) * resolution]) + hauteurPoint(currentVertice[(1) + (y + 1) * resolution])) / 6);
                        }
                        else if (y == 0)
                        {
                            newHeight = ((hauteurPoint(currentVertice[j]) + hauteurPoint(currentVertice[(x - 1)]) + hauteurPoint(currentVertice[(x + 1)]) + hauteurPoint(currentVertice[(x - 1) + resolution]) + hauteurPoint(currentVertice[(x) + resolution]) + hauteurPoint(currentVertice[(x + 1) + resolution])) / 6);
                        }
                        else if (x == resolution - 1)
                        {
                            newHeight = ((hauteurPoint(currentVertice[j]) + hauteurPoint(currentVertice[(x) + (y - 1) * resolution]) + hauteurPoint(currentVertice[(x) + (y + 1) * resolution]) + hauteurPoint(currentVertice[(x - 1) + (y - 1) * resolution]) + hauteurPoint(currentVertice[(x - 1) + (y) * resolution]) + hauteurPoint(currentVertice[(x - 1) + (y + 1) * resolution])) / 6);
                        }
                        else if (y == resolution - 1)
                        {
                            newHeight = ((hauteurPoint(currentVertice[j]) + hauteurPoint(currentVertice[(x - 1) + (y) * resolution]) + hauteurPoint(currentVertice[(x + 1) + (y) * resolution]) + hauteurPoint(currentVertice[(x - 1) + (y - 1) * resolution]) + hauteurPoint(currentVertice[(x) + (y - 1) * resolution]) + hauteurPoint(currentVertice[(x + 1) + (y - 1) * resolution])) / 6);
                        }
                        else
                        {
                            newHeight = ((hauteurPoint(currentVertice[j]) + hauteurPoint(currentVertice[(x - 1) + (y) * resolution]) + hauteurPoint(currentVertice[(x + 1) + (y) * resolution]) + hauteurPoint(currentVertice[(x - 1) + (y - 1) * resolution]) + hauteurPoint(currentVertice[(x) + (y - 1) * resolution]) + hauteurPoint(currentVertice[(x + 1) + (y - 1) * resolution]) + hauteurPoint(currentVertice[(x - 1) + (y + 1) * resolution]) + hauteurPoint(currentVertice[(x) + (y + 1) * resolution]) + hauteurPoint(currentVertice[(x + 1) + (y + 1) * resolution])) / 9);
                        }
                        currentVertice[j] *= newHeight/currentHeight;
                    }

                }
            }
            Debug.Log("Face "+i);
            textureFaces[i].mesh.vertices = currentVertice;
        }
    }

    public Vector3[] getAdjacentPoint(int faceIndex, int x, int y)
    {
        return null;
    }
}
