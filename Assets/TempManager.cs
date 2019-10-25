using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempManager : MonoBehaviour {

    public Planet planet;

    public bool showTemp = false;

    public int tempRatioCst = 1;

    public Vector3 poleNord;
    public Vector3 poleSud;

    public float tempMax = float.MinValue;
    public float tempMin = float.MaxValue;
    
    public void Initialize(Planet planet)
    {
        this.planet = planet;
        //creerEquateur();
        //lissageTemperature();
        //jolieGlaciation();
        //jolieGlaciation();
        GenerateTempYoucef();
    }

    public float getTemp(int faceID, int j)
    {
        return planet.textureFaces[faceID].temperature[j];
    }

    public void setTemp(float temp, int faceID, int j)
    {
        planet.textureFaces[faceID].temperature[j] = temp;
    }
    /*
    public void creerEquateur()
    {
        float distEquateur;
        int milieu = planet.resolution / 2 + (planet.resolution) * (planet.resolution / 2);
        for (int i = 0; i < 6; i++)
        {
            float[] currentTempTab = planet.textureFaces[i].temperature;
            for (int x = 0; x < planet.resolution; x++)
            {
                for (int y = planet.resolution-1; y >=0; y--)
                {
                    int j = x + y * planet.resolution;
                    distEquateur = Mathf.Min(Planet.distance(planet.textureFaces[i].mesh.vertices[j], planet.textureFaces[2].mesh.vertices[milieu]), Planet.distance(planet.textureFaces[i].mesh.vertices[j], planet.textureFaces[3].mesh.vertices[milieu]), Planet.distance(planet.textureFaces[i].mesh.vertices[j], planet.textureFaces[4].mesh.vertices[milieu]), Planet.distance(planet.textureFaces[i].mesh.vertices[j], planet.textureFaces[5].mesh.vertices[milieu]));
                    setTemp(45 - distEquateur * tempRatioCst / planet.resolution*6,i,j);

                }
            }
        }
    }

    public void lissageTemperature()
    {
        for (int i = 0; i < longueur; ++i)
        {
            for (int j = largeur - 1; j > 0; --j)
            {
                if (carte[i][j].terre)
                {
                    carte[i][j].setTemperature(realisticTempTerre(pixelsTemp(i, j, carte[i][j].getNiveau())));
                }

                else
                {
                    carte[i][j].setTemperature(realisticTempMer(pixelsTemp(i, j, carte[i][j].getNiveau())));

                    ((Mer)carte[i][j]).setLvlchlorophyle(this.lvlChlorophyle(i, j));

                }

                if (minTemp > carte[i][j].getTemperature())
                    minTemp = carte[i][j].getTemperature();

                if (maxTemp < carte[i][j].getTemperature())
                    maxTemp = carte[i][j].getTemperature();
            }
        }
    }

    public void jolieGlaciation()
    {
        for (int i = 0; i < longueur; ++i)
        {
            for (int j = largeur - 1; j > 0; --j)
            {
                if (carte[i][j].terre)
                {
                    carte[i][j].setTemperature(realisticTempTerre(pixelsTempG(i, j, carte[i][j].getNiveau())));
                }

                else
                {
                    carte[i][j].setTemperature(realisticTempMer(pixelsTempG(i, j, carte[i][j].getNiveau())));
                }

                if (minNiveau > carte[i][j].getNiveau())
                    minNiveau = carte[i][j].getNiveau();

                if (maxNiveau < carte[i][j].getNiveau())
                    maxNiveau = carte[i][j].getNiveau();
            }
        }
    }
    */

    public void GenerateTempYoucef()
    {
        int milieu = planet.resolution / 2 + (planet.resolution) * (planet.resolution / 2);
        poleNord = planet.textureFaces[0].mesh.vertices[milieu];
        poleSud = planet.textureFaces[1].mesh.vertices[milieu];

        float hauteurMin = Planet.hauteurPoint(planet.textureFaces[planet.verticeMinFaceIndex].mesh.vertices[planet.verticeMinIndex]) - planet.waterLevel;
        float hauteurMax = Planet.hauteurPoint(planet.textureFaces[planet.verticeMaxFaceIndex].mesh.vertices[planet.verticeMaxIndex]) - planet.waterLevel;
        
        float tempMultiplier = 1.85f;
        float tempHeightMult = 2f;

        for (int i = 0; i < 6; i++)
        {
            float[] currentTempTab = planet.textureFaces[i].temperature;
            for (int x = 0; x < planet.resolution; x++)
            {
                for (int y = planet.resolution - 1; y >= 0; y--)
                {
                    int j = x + y * planet.resolution;

                    float hauteur = Mathf.Abs(Planet.hauteurPoint(planet.textureFaces[i].mesh.vertices[j])-planet.waterLevel);
                    float hauteurRelative = (hauteur-hauteurMin) / (hauteurMax - hauteurMin);

                    float distanceNormal = Mathf.Min(Planet.distance(poleNord.normalized, planet.textureFaces[i].mesh.vertices[j].normalized), Planet.distance(poleSud.normalized, planet.textureFaces[i].mesh.vertices[j].normalized));
                    float realRadius = planet.waterLevel * 2;
                    
                    //if (hauteurRelative >= 0.7) Debug.Log(hauteurRelative);
                    currentTempTab[j] = -50 * tempHeightMult * hauteurRelative - 25 + realRadius * distanceNormal * tempMultiplier * 15;
                    //Debug.Log(currentTempTab[j]);
                    planet.textureFaces[i].colorTemp[j] = tempColor(currentTempTab[j]);
                    //Debug.Log(planet.textureFaces[i].colorTemp[j]);
                    //Debug.Log(planet.textureFaces[i].colorTemp[j]);

                    if (currentTempTab[j] > tempMax) tempMax = currentTempTab[j];
                    if (currentTempTab[j] < tempMin) tempMin = currentTempTab[j];
                }
            }
            //Debug.Log("tempMin : " + tempMin);
            //Debug.Log("tempMoy : " + (tempMin+tempMax)/2);
            //Debug.Log("temp1tiers : " + (tempMin + tempMax) / 3);
            //Debug.Log("temp2tiers : " + 2 * (tempMin + tempMax) / 3);
            //Debug.Log("tempMax : " + tempMax);
        }
    }
    
    public Color tempColor2(float temperature)
    {
        float lvlBlue = 0;
        float lvlGreen = 0;
        float lvlRed = 0;
        float tempMoy = (tempMax+tempMin) / 2;
        float temp1tiers = (tempMax - tempMin) / 3 + tempMin;
        float temp2tiers = 2*(tempMax - tempMin) / 3 + tempMin;
        int i = 0;
        int j = 0;
        int k = 0;
        if (temperature >= tempMin && temperature <= tempMoy)
        {
            i++;
            lvlBlue = 1 - Mathf.Abs((temperature - tempMin) / (tempMoy - tempMin));
        }
        if (temperature >= temp1tiers && temperature <= temp2tiers)
        {
            j++;
            if (temperature >= temp1tiers && temperature <= tempMoy)
            {
                lvlGreen = Mathf.Abs((temperature - tempMin) / (tempMoy - tempMin));
            }
            else
            {
                lvlGreen = 1 - Mathf.Abs((temperature - tempMoy) / (tempMax - tempMoy));
            }
    
        }
        if (temperature >= tempMoy && temperature <= tempMax)
        {
            k++;
            lvlRed = Mathf.Abs((temperature - tempMoy) / (tempMax - tempMoy));
        }
        if (j >= 1)
        {

            //Debug.Log("blue : " + i);
            //Debug.Log("green : " + j);
            //Debug.Log("Red : " + k);
        }
        return new Color(lvlBlue, lvlGreen, lvlRed, 1f);
    }

    public Color tempColor(float temperature)
    {
        float lvlr = 0;
        float lvlg = 0;
        float lvlb = 0;
        double b;
        double a;
        if (temperature <= 0)
        {
            a = -255.0 / 20.0;
            b = 50;
            lvlr = 0;
            lvlb = (int)(a * temperature + b);
            if (lvlb > 255)
                lvlb = 255;
            if (lvlb < 0)
                lvlb = 0;
            lvlg = 180 - lvlb;
            if (lvlg > 255)
                lvlg = 255;
            if (lvlg < 0)
                lvlg = 0;
        }

        if (temperature > 0)
        {
            a = 255.0 / 50.0;
            b = 0;
            lvlb = 0;
            lvlr = (int)(a * temperature + b);
            if (lvlr > 255)
                lvlr = 255;
            if (lvlr < 0)
                lvlr = 0;
            lvlg = 180 - lvlr;
            if (lvlg > 255)
                lvlg = 255;
            if (lvlg < 0)
                lvlg = 0;
        }

        return new Color(lvlr/255f, lvlg/255f, lvlb/255f,1);
    }
}
