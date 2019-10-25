using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter : MonoBehaviour{

    Noise noise = new Noise();

    public float strength = 1;
    [Range(1, 8)]
    public int numLayers = 1;
    public float baseRoughness = 1;
    public float roughness = 2;
    public float persistence = .5f;
    public Vector3 centre = new Vector3(0, 0, 0);
    public float minValue = 0;

    private void Start()
    {

    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + centre);
            noiseValue += (v + 1) * .5f * amplitude;
            frequency *= roughness;
            amplitude *= persistence;
        }
        noiseValue = Mathf.Max(0, noiseValue - minValue);
        return noiseValue * strength;
    }
}