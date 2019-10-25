using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {


    public GameObject TerraformButton;
    public bool terraformMode = false;
    public GameObject ElevateButton;
    public GameObject LowerButton;

    public float ratio = 1.05f;

    public GameObject SizeButton;


    public Slider StrengthSlider;
    public Slider RoughnessMainSlider;
    public Slider RoughnessSecondarySlider;
    public InputField CenterX;
    public InputField CenterY;
    public InputField CenterZ;
    public Slider colorSlider;
    public Image colorSliderImage;

    public Sommet currentSommet;


    public Planet currentPlanet;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (terraformMode)
        {
            RaycastHit hit;
            Debug.Log(Camera.main);
            Debug.Log(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    currentPlanet.ElevateSlot(hit.triangleIndex, hit, ratio, 3);
                    currentPlanet.ColoratePlanet();
                }
                if (currentSommet != null) UnselectSommet(currentSommet);
                int index = int.Parse(hit.transform.name.Replace("face ", ""));
                int pos = currentPlanet.textureFaces[index].mesh.triangles[hit.triangleIndex * 3];

                currentSommet = currentPlanet.textureFaces[index].Sommets[pos];
                SelectSommet(currentSommet);
                currentPlanet.textureFaces[index].GetComponentInParent<MeshCollider>().enabled = false;
                currentPlanet.textureFaces[index].GetComponentInParent<MeshCollider>().enabled = true;
            }
            /*if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Debug.Log(Camera.main);
                Debug.Log(Input.mousePosition);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    currentPlanet.ElevateSlot(hit.triangleIndex, hit, ratio, 3);
                    currentPlanet.ColoratePlanet();
                }
            }*/
        }
    }

    public void LoadSettings()
    {
        StrengthSlider.value = currentPlanet.shapeGenerator.noiseFilter0.strength;
        RoughnessMainSlider.value = currentPlanet.shapeGenerator.noiseFilter0.baseRoughness;
        RoughnessSecondarySlider.value = currentPlanet.shapeGenerator.noiseFilter2.baseRoughness;
        CenterX.text = "" + currentPlanet.shapeGenerator.noiseFilter1.centre.x;
        CenterY.text = "" + currentPlanet.shapeGenerator.noiseFilter1.centre.y;
        CenterZ.text = "" + currentPlanet.shapeGenerator.noiseFilter1.centre.z;
        float h,s,v;
        Color.RGBToHSV(currentPlanet.planetColor, out h, out s, out v);
        Debug.Log("h : " + h);
        colorSlider.value = h;
        colorSliderImage.color = currentPlanet.planetColor;
    }

    public void TerraformButtonClicked()
    {
        if (terraformMode)
        {
            ElevateButton.SetActive(false);
            LowerButton.SetActive(false);
            terraformMode = false;
        }
        else
        {
            ElevateButton.SetActive(true);
            LowerButton.SetActive(true);
            terraformMode = true;
        }
    }

    public void ElevateButtonClicked()
    {
        ratio = 1.05f;
    }

    public void LowerButtonClicked()
    {
        ratio = 1f/1.05f;
    }

    public void ModifierValuesChanged()
    {
        currentPlanet.shapeGenerator.noiseFilter0.strength = StrengthSlider.value;
        currentPlanet.shapeGenerator.noiseFilter0.baseRoughness = RoughnessMainSlider.value;
        currentPlanet.shapeGenerator.noiseFilter2.baseRoughness = RoughnessSecondarySlider.value;
        float flt;
        if (float.TryParse(CenterX.text, out flt)) currentPlanet.shapeGenerator.noiseFilter1.centre.x = flt;
        if (float.TryParse(CenterY.text, out flt)) currentPlanet.shapeGenerator.noiseFilter1.centre.y = flt;
        if (float.TryParse(CenterZ.text, out flt)) currentPlanet.shapeGenerator.noiseFilter1.centre.z = flt;
        currentPlanet.RecalculateVectors();
    }

    public void SelectSommet(Sommet s)
    {
        Color red = new Color(1, 0, 0, 1);
        Color[] newColor = currentPlanet.textureFaces[s.faceID].mesh.colors;
        newColor[s.x + s.y * currentPlanet.resolution] = red;
        foreach (Sommet s2 in s.pointsAdjacents)
        {
            newColor[s2.x + s2.y * currentPlanet.resolution] = red;
        }
        if (s.coastSommet.faceID == s.faceID) newColor[s.coastSommet.x + s.coastSommet.y * currentPlanet.resolution] = red;
        else
        {
            Color[] newColor2 = currentPlanet.textureFaces[s.coastSommet.faceID].mesh.colors;
            newColor2[s.coastSommet.x + s.coastSommet.y * currentPlanet.resolution] = red;
            currentPlanet.textureFaces[s.coastSommet.faceID].mesh.colors = newColor2;
        }
        currentPlanet.textureFaces[s.faceID].mesh.colors = newColor;
    }

    public void UnselectSommet(Sommet s)
    {
        currentPlanet.textureFaces[s.faceID].mesh.colors = currentPlanet.textureFaces[s.faceID].colorNormal;
        currentPlanet.textureFaces[s.coastSommet.faceID].mesh.colors = currentPlanet.textureFaces[s.coastSommet.faceID].colorNormal;
    }

    public void ColorSliderUpdated()
    {
        currentPlanet.planetColor = Color.HSVToRGB(colorSlider.value, 1, 1);
        colorSliderImage.color = currentPlanet.planetColor;
        currentPlanet.ColoratePlanet();
    }
}
