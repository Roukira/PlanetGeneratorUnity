using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{

    public GameObject target;//the target object
    private float speedMod = 10.0f;//a speed modifier
    private Vector3 point;//the coord to the point where the camera looks at
    public int strengthX = 0;
    public int strengthY = 0;

    void Start()
    {//Set up things on the start method

        point = target.transform.position;//get target's coords
        transform.LookAt(point);//makes the camera look to it
    }

    void Update()
    {//makes the camera rotate around "point" coords, rotating around its Y axis, 20 degrees per second times the speed modifier
        if (strengthX > 0)
        {
            transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), strengthX);
            strengthX--;
        }
        else if (strengthX <0)
        {
            transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), strengthX);
            strengthX++;
        }
        if (strengthY > 0)
        {
            transform.RotateAround(point, new Vector3(0.0f, 0.0f, 1.0f), strengthY);
            strengthY--;
        }
        else if (strengthY < 0)
        {
            transform.RotateAround(point, new Vector3(0.0f, 0.0f, 1.0f), strengthY);
            strengthY++;
        }

    }

    public void SwipeRight(float intensity)
    {
        if (strengthX > 0) strengthX /= 10;
        strengthX += (int)(intensity*20);
        if (strengthX > 100) strengthX = 100;
    }

    public void SwipeLeft(float intensity)
    {
        if (strengthX < 0) strengthX /= 10;
        strengthX -= (int)(intensity * 20);
        if (strengthX < -100) strengthX = -100;
    }

    public void SwipeDown(float intensity)
    {
        if (strengthY < 0) strengthY /= 10;
        strengthY -= (int)(intensity * 20);
        if (strengthY < -100) strengthY = -100;
    }

    public void SwipeUp(float intensity)
    {
        if (strengthY > 0) strengthY /= 10; 
        strengthY += (int)(intensity * 20);
        if (strengthY > 100) strengthY = 100;
    }
}