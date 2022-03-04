using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    public static List<Building> allInstantiatedBuildings = new List<Building>();

    public static BuildingHandler instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {

    }

    public static void udateCall()
    {
        
    }

    public static bool populationCentreBuilt()
    {
        for (int i = 0; i < allInstantiatedBuildings.Count; i++)
        {
            if (allInstantiatedBuildings[i].isPopulationCentre)
            {
                return true;
            }
        }

        return false;
    }
}
