using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Terraform {

	public static void Elevate(Sommet s, float ratio)
    {
        s.changeCoord(s.coord*ratio);
    }
}
