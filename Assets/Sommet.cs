using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sommet : MonoBehaviour{

    public Planet planet;

    public int faceID;
    public int x;
    public int y;
    public Vector3 coord;

    public Sommet[] pointsAdjacents;

    public bool cote = false;
    public float LevelWater = 0;

    public float degre = 0;
    public bool ouvert = false;
    public float distanceCoast = 0;
    public Sommet coastSommet;

    public void Initialize(Planet planet, int faceID, Vector3 coord, int x, int y)
    {
        this.planet = planet;
        this.faceID = faceID;
        this.coord = coord;
        this.x = x;
        this.y = y;

        pointsAdjacents = new Sommet[8];

        cote = isCoast();
        //updateWaterLevel();
        
    }
	
    public void changeCoord(Vector3 newCoord)
    {
        coord = newCoord;
        Vector3[] currentVertice = planet.textureFaces[faceID].mesh.vertices;
        currentVertice[x + planet.resolution * y] = newCoord;
        planet.textureFaces[faceID].mesh.vertices = currentVertice;
        cote = isCoast();
        updateWaterLevel();
    }

    public bool isCoast()
    {
        float hauteur = Planet.hauteurPoint(coord);
        return 0.999 * planet.waterLevel <= hauteur && 1.001 * planet.waterLevel >= hauteur; 
    }

    public void updateWaterLevel()
    {
        LevelWater = 1 / (Planet.hauteurPoint(coord) - planet.waterLevel);
    }

    public void searchClosestCoast()
    {
        InitDjiskra();
        //Debug.Log("first");
        for (int i = 0; i < 6; i++)
        {
            foreach (Sommet s2 in planet.textureFaces[i].Sommets)
            {
                Sommet currentSommet = searchMinDegre();
                if (ExamineDjikstra(currentSommet))
                {
                    distanceCoast = currentSommet.degre;
                    coastSommet = currentSommet;
                    return;
                }
            }
        }
        
    }

    public bool ExamineDjikstra(Sommet s)
    {
        if (s == null) return false;
        foreach (Sommet s2 in s.pointsAdjacents)
        {
            float value = s.degre + Planet.distance(s2.coord, s.coord);
            if (s2.degre > value)
            {
                s2.degre = value;
                if (!s2.ouvert) s2.ouvert = true;
                
            }
        }
        s.ouvert = false;
        if (s.cote) return true;
        return false;
    }

    public Sommet searchMinDegre()
    {
        Sommet currentSommet = null;
        float dmin = 1e20f;
        for (int i = 0; i < 6; i++)
        {
            foreach (Sommet s2 in planet.textureFaces[i].Sommets)
            {
                if (s2.ouvert && s2.degre <= dmin)
                {
                    dmin = s2.degre;
                    currentSommet = s2;
                }

            }
        }
        return currentSommet;
    }

    public void InitDjiskra()
    {
        for (int i = 0; i < 6; i++)
        {
            foreach (Sommet s2 in planet.textureFaces[i].Sommets)
            {

                s2.degre = 1e20f;
                s2.ouvert = false;
            }
        }
        degre = 0;
        ouvert = true;
        //Debug.Log("InitDjiskra");
    }

    public void updatePointsAdjacents()
    {
        int n = planet.resolution * planet.resolution;

        int[] tab = changeVariable(x, y);
        int newx1 = tab[1];
        int newy1 = tab[2];
        int newx2 = tab[4];
        int newy2 = tab[5];
        int face1 = tab[0];
        int face2 = tab[3];

        if (face1 == -1)
        {
            int LeftCenter = x - 1 + planet.resolution * y;
            int RightCenter = x + 1 + planet.resolution * y;
            int CenterUpper = x + planet.resolution * (y - 1);
            int LeftUpper = x - 1 + planet.resolution * (y - 1);
            int RightUpper = x + 1 + planet.resolution * (y - 1);
            int CenterLower = x + planet.resolution * (y + 1);
            int LeftLower = x - 1 + planet.resolution * (y + 1);
            int RightLower = x + 1 + planet.resolution * (y + 1);


            pointsAdjacents[0] = planet.textureFaces[faceID].Sommets[LeftUpper];
            pointsAdjacents[1] = planet.textureFaces[faceID].Sommets[CenterUpper];
            pointsAdjacents[2] = planet.textureFaces[faceID].Sommets[RightUpper];
            pointsAdjacents[3] = planet.textureFaces[faceID].Sommets[LeftCenter];
            pointsAdjacents[4] = planet.textureFaces[faceID].Sommets[RightCenter];
            pointsAdjacents[5] = planet.textureFaces[faceID].Sommets[LeftLower];
            pointsAdjacents[6] = planet.textureFaces[faceID].Sommets[CenterLower];
            pointsAdjacents[7] = planet.textureFaces[faceID].Sommets[RightLower];
        }
        else if (face2 == -1)
        {
            int LeftCenter = x - 1 + planet.resolution * y;
            int RightCenter = x + 1 + planet.resolution * y;
            int CenterUpper = x + planet.resolution * (y - 1);
            int LeftUpper = x - 1 + planet.resolution * (y - 1);
            int RightUpper = x + 1 + planet.resolution * (y - 1);
            int CenterLower = x + planet.resolution * (y + 1);
            int LeftLower = x - 1 + planet.resolution * (y + 1);
            int RightLower = x + 1 + planet.resolution * (y + 1);

            if (x == planet.resolution - 1)
            {
                if (newy1 == 0 || newy1 == planet.resolution - 1)
                {
                    RightCenter = newx1 + planet.resolution * newy1;
                    RightLower = newx1 + 1 + planet.resolution * newy1;
                    RightUpper = newx1 - 1 + planet.resolution * newy1;
                }
                else
                {
                    RightCenter = newx1 + planet.resolution * newy1;
                    RightLower = newx1 + planet.resolution * (newy1+1);
                    RightUpper = newx1 + planet.resolution * (newy1-1);
                }

                pointsAdjacents[0] = planet.textureFaces[faceID].Sommets[LeftUpper];
                pointsAdjacents[1] = planet.textureFaces[faceID].Sommets[CenterUpper];
                pointsAdjacents[2] = planet.textureFaces[face1].Sommets[RightUpper];
                pointsAdjacents[3] = planet.textureFaces[faceID].Sommets[LeftCenter];
                pointsAdjacents[4] = planet.textureFaces[face1].Sommets[RightCenter];
                pointsAdjacents[5] = planet.textureFaces[faceID].Sommets[LeftLower];
                pointsAdjacents[6] = planet.textureFaces[faceID].Sommets[CenterLower];
                pointsAdjacents[7] = planet.textureFaces[face1].Sommets[RightLower];
            }

            else if (x == 0)
            {
                if (newy1 == 0 || newy1 == planet.resolution - 1)
                {
                    LeftCenter = newx1 + planet.resolution * newy1;
                    LeftLower = newx1 + 1 + planet.resolution * newy1;
                    LeftUpper = newx1 - 1 + planet.resolution * newy1;
                }
                else
                {
                    LeftCenter = newx1 + planet.resolution * newy1;
                    LeftLower = newx1 + planet.resolution * (newy1 + 1);
                    LeftUpper = newx1 + planet.resolution * (newy1 - 1);
                }

                pointsAdjacents[0] = planet.textureFaces[face1].Sommets[LeftUpper];
                pointsAdjacents[1] = planet.textureFaces[faceID].Sommets[CenterUpper];
                pointsAdjacents[2] = planet.textureFaces[faceID].Sommets[RightUpper];
                pointsAdjacents[3] = planet.textureFaces[face1].Sommets[LeftCenter];
                pointsAdjacents[4] = planet.textureFaces[faceID].Sommets[RightCenter];
                pointsAdjacents[5] = planet.textureFaces[face1].Sommets[LeftLower];
                pointsAdjacents[6] = planet.textureFaces[faceID].Sommets[CenterLower];
                pointsAdjacents[7] = planet.textureFaces[faceID].Sommets[RightLower];
            }
            else if (y == 0)
            {
                if (newx1 == 0 || newx1 == planet.resolution - 1)
                {
                    LeftUpper = newx1 + planet.resolution * newy1;
                    CenterUpper = newx1 + planet.resolution * (newy1+1);
                    RightUpper = newx1 + planet.resolution * (newy1-1);
                }
                else
                {
                    LeftUpper = newx1 + planet.resolution * newy1;
                    CenterUpper = newx1 +1 + planet.resolution * newy1;
                    RightUpper = newx1 -1 + planet.resolution * newy1;
                }

                pointsAdjacents[0] = planet.textureFaces[face1].Sommets[LeftUpper];
                pointsAdjacents[1] = planet.textureFaces[face1].Sommets[CenterUpper];
                pointsAdjacents[2] = planet.textureFaces[face1].Sommets[RightUpper];
                pointsAdjacents[3] = planet.textureFaces[faceID].Sommets[LeftCenter];
                pointsAdjacents[4] = planet.textureFaces[faceID].Sommets[RightCenter];
                pointsAdjacents[5] = planet.textureFaces[faceID].Sommets[LeftLower];
                pointsAdjacents[6] = planet.textureFaces[faceID].Sommets[CenterLower];
                pointsAdjacents[7] = planet.textureFaces[faceID].Sommets[RightLower];
            }
            else
            {
                if (newx1 == 0 || newx1 == planet.resolution - 1)
                {
                    LeftLower = newx1 + planet.resolution * newy1;
                    CenterLower = newx1 + planet.resolution * (newy1 + 1);
                    RightLower = newx1 + planet.resolution * (newy1 - 1);
                }
                else
                {
                    LeftLower = newx1 + planet.resolution * newy1;
                    CenterLower = newx1 + 1 + planet.resolution * newy1;
                    RightLower = newx1 - 1 + planet.resolution * newy1;
                }

                pointsAdjacents[0] = planet.textureFaces[faceID].Sommets[LeftUpper];
                pointsAdjacents[1] = planet.textureFaces[faceID].Sommets[CenterUpper];
                pointsAdjacents[2] = planet.textureFaces[faceID].Sommets[RightUpper];
                pointsAdjacents[3] = planet.textureFaces[faceID].Sommets[LeftCenter];
                pointsAdjacents[4] = planet.textureFaces[faceID].Sommets[RightCenter];
                pointsAdjacents[5] = planet.textureFaces[face1].Sommets[LeftLower];
                pointsAdjacents[6] = planet.textureFaces[face1].Sommets[CenterLower];
                pointsAdjacents[7] = planet.textureFaces[face1].Sommets[RightLower];
            }
        }
        else
        {
            int LeftCenter = x - 1 + planet.resolution * y;
            int RightCenter = x + 1 + planet.resolution * y;
            int CenterUpper = x + planet.resolution * (y - 1);
            int LeftUpper = x - 1 + planet.resolution * (y - 1);
            int RightUpper = x + 1 + planet.resolution * (y - 1);
            int CenterLower = x + planet.resolution * (y + 1);
            int LeftLower = x - 1 + planet.resolution * (y + 1);
            int RightLower = x + 1 + planet.resolution * (y + 1);

            if (x == planet.resolution - 1 && y == planet.resolution - 1)
            {
                if (newy1 == 0)
                {
                    RightCenter = newx1 + planet.resolution * newy1;
                    RightLower = newx1 + planet.resolution * (newy1+1);
                    RightUpper = newx1 + planet.resolution * (newy1+2);
                    if (newy2 == 0)
                    {
                        CenterLower = newx2 + planet.resolution * newy2;
                        LeftLower = newx2 + planet.resolution * (newy2 + 1);
                    }
                    else
                    {
                        CenterLower = newx2 + planet.resolution * newy2;
                        LeftLower = newx2 + planet.resolution * (newy2-1);
                    }
                }
                else
                {
                    RightCenter = newx1 + planet.resolution * newy1;
                    RightLower = newx1 + planet.resolution * (newy1-1);
                    RightUpper = newx1 + planet.resolution * (newy1-2);
                    if (newy2 == 0)
                    {
                        CenterLower = newx2 + planet.resolution * newy2;
                        LeftLower = newx2 + planet.resolution * (newy2 + 1);
                    }
                    else
                    {
                        CenterLower = newx2 + planet.resolution * newy2;
                        LeftLower = newx2 + planet.resolution * (newy2 - 1);
                    }
                }

                if (RightLower >= n && RightLower >= 0)
                {
                    Debug.Log("danger");
                    if (newx2 > 0) RightLower = newx2 - 1 + planet.resolution * newy2;
                    else if (newx2 < planet.resolution - 1) RightLower = newx2 + 2 + planet.resolution * newy2;
                    else if (newy2 > 0) RightLower = newx2 + planet.resolution * (newy2 - 1);
                    else RightLower = newx2 + planet.resolution * (newy2 + 2);
                    pointsAdjacents[7] = planet.textureFaces[face2].Sommets[RightLower];
                }
                else
                {
                    pointsAdjacents[7] = planet.textureFaces[face1].Sommets[RightLower];
                }

                pointsAdjacents[0] = planet.textureFaces[faceID].Sommets[LeftUpper];
                pointsAdjacents[1] = planet.textureFaces[faceID].Sommets[CenterUpper];
                pointsAdjacents[2] = planet.textureFaces[face1].Sommets[RightUpper];
                pointsAdjacents[3] = planet.textureFaces[faceID].Sommets[LeftCenter];
                pointsAdjacents[4] = planet.textureFaces[face1].Sommets[RightCenter];
                pointsAdjacents[5] = planet.textureFaces[face2].Sommets[LeftLower];
                pointsAdjacents[6] = planet.textureFaces[face2].Sommets[CenterLower];
                
            }

            else if (x == 0 && y == 0)
            {
                if (newy1 == 0)
                {
                    CenterUpper = newx1 + planet.resolution * newy1;
                    RightUpper = newx1 + planet.resolution * (newy1+1);
                    LeftUpper = newx1 + planet.resolution * (newy1+2);
                    if (newy2 == 0)
                    {
                        LeftCenter = newx2 + planet.resolution * newy2;
                        LeftLower = newx2 + planet.resolution * (newy2 + 1);
                    }
                    else
                    {
                        LeftCenter = newx2 + planet.resolution * newy2;
                        LeftLower = newx2 + planet.resolution * (newy2-1);
                    }
                }
                else
                {
                    CenterUpper = newx1 + planet.resolution * newy1;
                    RightUpper = newx1 + planet.resolution * (newy1 - 1);
                    LeftUpper = newx1 + planet.resolution * (newy1 - 2);
                    if (newy2 == 0)
                    {
                        LeftCenter = newx2 + planet.resolution * newy2;
                        LeftLower = newx2 + planet.resolution * (newy2 + 1);
                    }
                    else
                    {
                        LeftCenter = newx2 + planet.resolution * newy2;
                        LeftLower = newx2 + planet.resolution * (newy2-1);
                    }
                }

                if (LeftUpper >= n && LeftUpper >= 0)
                {
                    Debug.Log("danger");
                    if (newx2 > 0) LeftUpper = newx2 - 1 + planet.resolution * newy2;
                    else if (newx2 < planet.resolution - 1) LeftUpper = newx2 + 2 + planet.resolution * newy2;
                    else if (newy2 > 0) LeftUpper = newx2 + planet.resolution * (newy2 - 1);
                    else LeftUpper = newx2 + planet.resolution * (newy2 + 2);
                    Debug.Log(LeftUpper);
                    pointsAdjacents[0] = planet.textureFaces[face2].Sommets[LeftUpper];
                }
                else
                {
                    pointsAdjacents[0] = planet.textureFaces[face1].Sommets[LeftUpper];
                }

                pointsAdjacents[1] = planet.textureFaces[face1].Sommets[CenterUpper];
                pointsAdjacents[2] = planet.textureFaces[face1].Sommets[RightUpper];
                pointsAdjacents[3] = planet.textureFaces[face2].Sommets[LeftCenter];
                pointsAdjacents[4] = planet.textureFaces[faceID].Sommets[RightCenter];
                pointsAdjacents[5] = planet.textureFaces[face2].Sommets[LeftLower];
                pointsAdjacents[6] = planet.textureFaces[faceID].Sommets[CenterLower];
                pointsAdjacents[7] = planet.textureFaces[faceID].Sommets[RightLower];
            }
            else if (x == planet.resolution -1 && y == 0)
            {
                if (newy1 == 0)
                {
                    CenterUpper = newx1 + planet.resolution * newy1;
                    RightUpper = newx1 + planet.resolution * (newy1+1);
                    LeftUpper = newx1 + planet.resolution * (newy1+2);
                    if (newy2 == 0)
                    {
                        RightCenter = newx2 + planet.resolution * newy2;
                        RightLower = newx2 + planet.resolution * (newy2 + 1);
                    }
                    else
                    {
                        RightCenter = newx2 + planet.resolution * newy2;
                        RightLower = newx2 + planet.resolution * (newy2-1);
                    }
                }
                else
                {
                    CenterUpper = newx1 + planet.resolution * newy1;
                    RightUpper = newx1 + planet.resolution * (newy1 - 1);
                    LeftUpper = newx1 + planet.resolution * (newy1 - 2);
                    if (newy2 == 0)
                    {
                        RightCenter = newx2 + planet.resolution * newy2;
                        RightLower = newx2 + planet.resolution * (newy2 + 1);
                    }
                    else
                    {
                        RightCenter = newx2 + planet.resolution * newy2;
                        RightLower = newx2 + planet.resolution * (newy2-1);
                    }
                }

                if (RightUpper >= n && RightUpper >= 0)
                {
                    Debug.Log("danger");
                    if (newx2 > 0) RightUpper = newx2 - 1 + planet.resolution * newy2;
                    else if (newx2 < planet.resolution - 1) RightUpper = newx2 + 2 + planet.resolution * newy2;
                    else if (newy2 > 0) RightUpper = newx2 + planet.resolution * (newy2 - 1);
                    else RightUpper = newx2 + planet.resolution * (newy2 + 2);
                    pointsAdjacents[2] = planet.textureFaces[face2].Sommets[RightUpper];
                }
                else
                {
                    pointsAdjacents[2] = planet.textureFaces[face1].Sommets[RightUpper];
                }

                pointsAdjacents[0] = planet.textureFaces[face1].Sommets[LeftUpper];
                pointsAdjacents[1] = planet.textureFaces[face1].Sommets[CenterUpper];
                pointsAdjacents[3] = planet.textureFaces[faceID].Sommets[LeftCenter];
                pointsAdjacents[4] = planet.textureFaces[face2].Sommets[RightCenter];
                pointsAdjacents[5] = planet.textureFaces[faceID].Sommets[LeftLower];
                pointsAdjacents[6] = planet.textureFaces[faceID].Sommets[CenterLower];
                pointsAdjacents[7] = planet.textureFaces[face2].Sommets[RightLower];
            }
            else
            {
                if (newy1 == 0)
                {
                    LeftCenter = newx1 + planet.resolution * newy1;
                    LeftUpper = newx1 + planet.resolution * (newy1+1);
                    LeftLower = newx1 + planet.resolution * (newy1+2);
                    if (newy2 == 0)
                    {
                        CenterLower = newx2 + planet.resolution * newy2;
                        RightLower = newx2 + planet.resolution * (newy2 + 1);
                    }
                    else
                    {
                        CenterLower = newx2 + planet.resolution * newy2;
                        RightLower = newx2 + planet.resolution * (newy2 - 1);
                    }
                }
                else
                {
                    LeftCenter = newx1 + planet.resolution * newy1;
                    LeftUpper = newx1 + planet.resolution * (newy1 - 1);
                    LeftLower = newx1 + planet.resolution * (newy1 - 2);
                    if (newy2 == 0)
                    {
                        CenterLower = newx2 + planet.resolution * newy2;
                        RightLower = newx2 + planet.resolution * (newy2 + 1);
                    }
                    else
                    {
                        CenterLower = newx2 + planet.resolution * newy2;
                        RightLower = newx2 + planet.resolution * (newy2 - 1);
                    }
                }

                if (LeftLower >= n && LeftLower >= 0)
                {
                    Debug.Log("danger");
                    if (newx2 > 0) LeftLower = newx2 - 1 + planet.resolution * newy2;
                    else if (newx2 < planet.resolution - 1) LeftLower = newx2 + 2 + planet.resolution * newy2;
                    else if (newy2 > 0) LeftLower = newx2 + planet.resolution * (newy2 - 1);
                    else LeftLower = newx2 + planet.resolution * (newy2 + 2);
                    pointsAdjacents[5] = planet.textureFaces[face2].Sommets[LeftLower];
                }
                else
                {
                    pointsAdjacents[5] = planet.textureFaces[face1].Sommets[LeftLower];
                }

                pointsAdjacents[0] = planet.textureFaces[face1].Sommets[LeftUpper];
                pointsAdjacents[1] = planet.textureFaces[faceID].Sommets[CenterUpper];
                pointsAdjacents[2] = planet.textureFaces[faceID].Sommets[RightUpper];
                pointsAdjacents[3] = planet.textureFaces[face1].Sommets[LeftCenter];
                pointsAdjacents[4] = planet.textureFaces[faceID].Sommets[RightCenter];
                pointsAdjacents[6] = planet.textureFaces[face2].Sommets[CenterLower];
                pointsAdjacents[7] = planet.textureFaces[face2].Sommets[RightLower];
            }
        }
    }

    public int[] changeVariable(int x, int y)
    {
        int[] tab = new int[6];
        tab[0] = -1;
        tab[1] = -1;
        tab[2] = -1;
        tab[3] = -1;
        tab[4] = -1;
        tab[5] = -1;

        int n = planet.resolution;
        if (faceID == 0)
        {
            if (x == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 3;
                    tab[1] = n - 1 - y;
                    tab[2] = 0;
                }
                else
                {
                    tab[3] = 3;
                    tab[4] = n - 1 - y;
                    tab[5] = 0;
                }
            }
            if (y == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 4;
                    tab[1] = n - 1;
                    tab[2] = n - 1 - x;
                }
                else
                {
                    tab[3] = 4;
                    tab[4] = n - 1;
                    tab[5] = n - 1 - x;
                }
            }
            if (x == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 2;
                    tab[1] = y;
                    tab[2] = 0;
                }
                else
                {
                    tab[3] = 2;
                    tab[4] = y;
                    tab[5] = 0;
                }              
            }
            if (y == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 5;
                    tab[1] = 0;
                    tab[2] = n - 1 - x;
                }
                else
                {
                    tab[3] = 5;
                    tab[4] = 0;
                    tab[5] = n - 1 - x;
                }
            }
        }
        else if (faceID == 1)
        {
            if (x == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 2;
                    tab[1] = y;
                    tab[2] = n - 1;
                }
                else
                {
                    tab[3] = 2;
                    tab[4] = y;
                    tab[5] = n - 1;
                }                
            }
            if (y == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 4;
                    tab[1] = 0;
                    tab[2] = x;
                }
                else
                {
                    tab[3] = 4;
                    tab[4] = 0;
                    tab[5] = x;
                }
            }
            if (x == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 3;
                    tab[1] = n - 1 - y;
                    tab[2] = n - 1;
                }
                else
                {
                    tab[3] = 3;
                    tab[4] = n - 1 - y;
                    tab[5] = n - 1;
                }
            }
            if (y == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 5;
                    tab[1] = n - 1;
                    tab[2] = x;
                }
                else
                {
                    tab[3] = 5;
                    tab[4] = n - 1;
                    tab[5] = x;
                }
            }
        }
        else if (faceID == 2)
        {
            if (x == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 5;
                    tab[1] = y;
                    tab[2] = n - 1;
                }
                else
                {
                    tab[3] = 5;
                    tab[4] = y;
                    tab[5] = n - 1;
                }
            }
            if (y == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 0;
                    tab[1] = 0;
                    tab[2] = x;
                }
                else
                {
                    tab[3] = 0;
                    tab[4] = 0;
                    tab[5] = x;
                } 
            }
            if (x == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 4;
                    tab[1] = n - 1 - y;
                    tab[2] = n - 1;
                }
                else
                {
                    tab[3] = 4;
                    tab[4] = n - 1 - y;
                    tab[5] = n - 1;
                }
            }
            if (y == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 1;
                    tab[1] = n - 1;
                    tab[2] = x;
                }
                else
                {
                    tab[3] = 1;
                    tab[4] = n - 1;
                    tab[5] = x;
                } 
            }
        }
        else if (faceID == 3)
        {
            if (x == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 4;
                    tab[1] = n - 1 - y;
                    tab[2] = 0;
                }
                else
                {
                    tab[3] = 4;
                    tab[4] = n - 1 - y;
                    tab[5] = 0;
                }    
            }
            if (y == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 0;
                    tab[1] = n - 1;
                    tab[2] = n - 1 - x;
                }
                else
                {
                    tab[3] = 0;
                    tab[4] = n - 1;
                    tab[5] = n - 1 - x;
                }  
            }
            if (x == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 5;
                    tab[1] = y;
                    tab[2] = 0;
                }
                else
                {
                    tab[3] = 5;
                    tab[4] = y;
                    tab[5] = 0;
                }   
            }
            if (y == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 1;
                    tab[1] = 0;
                    tab[2] = n - 1 - x;
                }
                else
                {
                    tab[3] = 1;
                    tab[4] = 0;
                    tab[5] = n - 1 - x;
                }  
            }
        }
        else if (faceID == 4)
        {
            if (x == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 0;
                    tab[1] = n - 1 - y;
                    tab[2] = 0;
                }
                else
                {
                    tab[3] = 0;
                    tab[4] = n - 1 - y;
                    tab[5] = 0;
                }
            }
            if (y == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 3;
                    tab[1] = n - 1;
                    tab[2] = n - 1 - x;
                }
                else
                {
                    tab[3] = 3;
                    tab[4] = n - 1;
                    tab[5] = n - 1 - x;
                }
            }
            if (x == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 1;
                    tab[1] = y;
                    tab[2] = 0;
                }
                else
                {
                    tab[3] = 1;
                    tab[4] = y;
                    tab[5] = 0;
                }
            }
            if (y == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 2;
                    tab[1] = 0;
                    tab[2] = n - 1 - x;
                }
                else
                {
                    tab[3] = 2;
                    tab[4] = 0;
                    tab[5] = n - 1 - x;
                }
            }
        }
        else if (faceID == 5)
        {
            if (x == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 1;
                    tab[1] = y;
                    tab[2] = n - 1;
                }
                else
                {
                    tab[3] = 1;
                    tab[4] = y;
                    tab[5] = n - 1;
                }
            }
            if (y == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 3;
                    tab[1] = 0;
                    tab[2] = x;
                }
                else
                {
                    tab[3] = 3;
                    tab[4] = 0;
                    tab[5] = x;
                }
            }
            if (x == 0)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 0;
                    tab[1] = n - 1 - y;
                    tab[2] = n - 1;
                }
                else
                {
                    tab[3] = 0;
                    tab[4] = n - 1 - y;
                    tab[5] = n - 1;
                }   
            }
            if (y == n - 1)
            {
                if (tab[0] == -1)
                {
                    tab[0] = 2;
                    tab[1] = n - 1;
                    tab[2] = x;
                }
                else
                {
                    tab[3] = 2;
                    tab[4] = n - 1;
                    tab[5] = x;
                }
            }
        }
        return tab;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
