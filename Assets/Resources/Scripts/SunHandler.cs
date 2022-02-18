using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunHandler : MonoBehaviour
{
    public float circularPeriodInSeconds = 1;

    public static SunHandler instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        startStopCirculation(true);
    }

    public static void startStopCirculation(bool start)
    {
        if (start)
        {
            instance.GetComponent<Animator>().speed = 1 / instance.circularPeriodInSeconds;
        }
        else
        {
            instance.GetComponent<Animator>().speed = 0;
        }

    }

    private void OnApplicationQuit()
    {
        save();
    }

    public static void save()
    {
        SaveAndLoad.saveSunData(instance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    public static void load()
    {
        float loaded = SaveAndLoad.loadSunData();
        instance.GetComponent<Animator>().Play("Sunrotation", 0, loaded);
    }
}
