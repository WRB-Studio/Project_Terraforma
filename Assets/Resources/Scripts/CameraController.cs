//  A simple Unity C# script for orbital movement around a target gameobject
//  Author: Ashkan Ashtiani
//  Gist on Github: https://gist.github.com/3dln/c16d000b174f7ccf6df9a1cb0cef7f80

using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public float distance = 10.0f;
    public float minDistance = 10;
    public float minDistanceAdd = 10;
    public float maxDistance = 10;
    public float prevDistance;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;

    public float zoomSpeed = 1;
    
    float x = 0.0f;
    float y = 0.0f;

    public static CameraController instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        float planetSize = PlanetAttribute.getPlanetAttribute(PlanetAttribute.ePlanetAttributes.Size).currentAndTargetValue.x;
        instance.minDistance = planetSize / 2 + instance.minDistanceAdd;
        instance.maxDistance = instance.minDistance * 3;
        instance.distance = instance.maxDistance;

        var angles = instance.transform.eulerAngles;
        instance.x = angles.y;
        instance.y = angles.x;
    }

    void LateUpdate()
    {
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        if (target && Input.GetMouseButton(1))
        {
            var pos = Input.mousePosition;
            var dpiScale = 1f;
            if (Screen.dpi < 1) dpiScale = 1;
            if (Screen.dpi < 200) dpiScale = 1;
            else dpiScale = Screen.dpi / 200f;

            if (pos.x < 380 * dpiScale && Screen.height - pos.y < 250 * dpiScale) return;

            // comment out these two lines if you don't want to hide mouse curser or you have a UI button 
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);
            var rotation = Quaternion.Euler(y, x, 0);
            var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
            transform.rotation = rotation;
            transform.position = position;

        }
        else
        {
            // comment out these two lines if you don't want to hide mouse curser or you have a UI button 
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Math.Abs(prevDistance - distance) > 0.001f)
        {
            prevDistance = distance;
            var rot = Quaternion.Euler(y, x, 0);
            var po = rot * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
            transform.rotation = rot;
            transform.position = po;
        }
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }


    public static void save()
    {
        SaveAndLoad.saveCamData(instance.distance, instance.x, instance.y);
    }

    public static void load()
    {
        float[] loaded = SaveAndLoad.loadCamData();

        if (loaded != null)
        {
            instance.distance = loaded[0];
            instance.x = loaded[1];
            instance.y = loaded[2];
        }
    }
}