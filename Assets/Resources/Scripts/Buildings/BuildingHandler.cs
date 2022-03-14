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

    public static void updateCall()
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

    public static void registerBuilding(Building newBuilding)
    {
        allInstantiatedBuildings.Add(newBuilding);

        EnergyHandler.registerBuilding(newBuilding);
        ResourceHandler.registerBuilding(newBuilding);
        PopulationHandler.registerBuilding(newBuilding);
    }

    public static void deregisterBuilding(Building removeBuilding, PopulationHandler.eAddRemovePopulationReason reason)
    {
        allInstantiatedBuildings.Remove(removeBuilding);

        EnergyHandler.deregisterBuilding(removeBuilding);
        ResourceHandler.deregisterBuilding(removeBuilding);
        PopulationHandler.deregisterBuilding(removeBuilding, reason);

        Destroy(removeBuilding);
    }
}
