using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyHandler : MonoBehaviour
{
    public static Int64 consume;
    public static Int64 available;

    public static List<Building> powerPlants = new List<Building>();
    public static List<Building> consumerBuilding = new List<Building>();

    public static EnergyHandler instance;


    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {

    }

    public static void updateCall()
    {
        available = getAvailableEnergy();
        consume = getEnergyConsume();        
    }

    public static Int64 getAvailableEnergy()
    {
        int tmpEnergy = 0;

        Building[] buildings = powerPlants.ToArray();
        for (int i = 0; i < buildings.Length; i++)
        {
            if (buildings[i].isActivated && buildings[i].energy_Production_Consuming > 0)
                tmpEnergy += buildings[i].energy_Production_Consuming;
        }
        return tmpEnergy;
    }

    public static Int64 getEnergyConsume()
    {
        int tmpEnergy = 0;

        Building[] buildings = consumerBuilding.ToArray();
        for (int i = 0; i < buildings.Length; i++)
        {
            if (buildings[i].isActivated && buildings[i].energy_Production_Consuming < 0)
                tmpEnergy -= buildings[i].energy_Production_Consuming;
        }
        return tmpEnergy;
    }


    public static void addBuilding(Building newBuilding)
    {
        if (newBuilding.energy_Production_Consuming > 0)
        {
            powerPlants.Add(newBuilding);
        }
        else if (newBuilding.energy_Production_Consuming < 0)
        {
            consumerBuilding.Add(newBuilding);

            //Check enough energy available
            if (consume + newBuilding.energy_Production_Consuming > available)
                newBuilding.hasEnergy = false;
            else
                newBuilding.hasEnergy = true;
        }
    }

    public static void removeBuilding(Building removeBuilding)
    {
        if (removeBuilding.energy_Production_Consuming > 0)
            powerPlants.Remove(removeBuilding);
        else if (removeBuilding.energy_Production_Consuming < 0)
            consumerBuilding.Remove(removeBuilding);
    }

}
