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
        EnergyHandler.addBuilding(newBuilding);
        ResourceHandler.addBuilding(newBuilding);
    }

    public static void removeBuilding(Building removeBuilding)
    {
        allInstantiatedBuildings.Remove(removeBuilding);
        EnergyHandler.removeBuilding(removeBuilding);
        ResourceHandler.removeBuilding(removeBuilding);

        if (removeBuilding.maxHousingUnits > 0)
            PopulationHandler.addRemoveHousingUnits(-removeBuilding.maxHousingUnits);

        if (removeBuilding.workplaces > 0)
            PopulationHandler.addRemoveWorkplaces(-removeBuilding.workplaces);

    }
}
