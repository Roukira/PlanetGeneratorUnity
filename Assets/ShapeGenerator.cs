using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator : MonoBehaviour{

    public float planetRadius = 0.5f;

    public SimpleNoiseFilter noiseFilter0;
    public SimpleNoiseFilter noiseFilter1;
    public RidgidNoiseFilter noiseFilter2;

    /*public ShapeGenerator(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }*/

    private void Start()
    {
        

    }

    public void Initiliaze()
    {
        Debug.Log("hello");
        noiseFilter0 = gameObject.AddComponent<SimpleNoiseFilter>();

        noiseFilter1 = gameObject.AddComponent<SimpleNoiseFilter>();

        noiseFilter2 = gameObject.AddComponent<RidgidNoiseFilter>();
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float heightElevation = 0;

        firstLayerValue = noiseFilter0.Evaluate(pointOnUnitSphere);
        heightElevation = firstLayerValue;
        heightElevation += noiseFilter1.Evaluate(pointOnUnitSphere)*firstLayerValue;
        heightElevation += noiseFilter2.Evaluate(pointOnUnitSphere) * firstLayerValue;

        return pointOnUnitSphere * planetRadius * (1+heightElevation);
    }
}
