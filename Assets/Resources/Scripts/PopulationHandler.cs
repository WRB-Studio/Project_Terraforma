using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationHandler : MonoBehaviour
{
    public enum eAddRemovePopulationReason
    {
        None, byBirth, byEvent, placedBuilding,
        removedBuilding, destroyedBuilding, byOtherKill,
    }

    private static double birthratePer1000 = 0.67;
    public static double BirthratePer1000 { get => birthratePer1000; }


    private static Int64 population;
    public static Int64 Population { get => population; }


    private static Int64 housingUnits;
    public static Int64 HousingUnits { get => housingUnits; }


    private static Int64 workplaces;
    public static Int64 Workplaces { get => workplaces; }


    public static List<Building> housingUnitBuildings = new List<Building>();
    public static List<Building> populationCentres = new List<Building>();

    public static PopulationHandler instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {

    }

    public static void updateCall()
    {
        if (population > 0)
            addRemovePopulation(birthsUpdate(), eAddRemovePopulationReason.byBirth);
    }



    public static void addRemovePopulation(Int64 addValue, eAddRemovePopulationReason reason)
    {
        switch (reason)
        {
            case eAddRemovePopulationReason.None:
                population += addValue;
                break;
            case eAddRemovePopulationReason.byBirth:
                population += addValue;
                break;
            case eAddRemovePopulationReason.byEvent:
                population += addValue;
                break;
            case eAddRemovePopulationReason.placedBuilding:
                population += addValue;
                break;
            case eAddRemovePopulationReason.removedBuilding:
                break;
            case eAddRemovePopulationReason.destroyedBuilding:
                if (population >= housingUnits)//means that all housing units occupied.
                {
                    population += addValue;
                }
                else//Remove only the percentage of the population living in housing units.
                {
                    
                    float percent = population / (housingUnits / 100);
                    population += Mathf.RoundToInt(addValue / 100 * percent);
                }
                break;
            case eAddRemovePopulationReason.byOtherKill:
                population += addValue;
                break;
            default:
                break;
        }

        if (population < 0)
            population = 0;
    }

    public static void addRemoveHousingUnits(Int64 addValue)
    {
        housingUnits += addValue;

        if (housingUnits < 0)
            housingUnits = 0;
    }

    public static void addRemoveWorkplaces(Int64 addValue)
    {
        workplaces += addValue;

        if (workplaces < 0)
            workplaces = 0;
    }


    public static double getBirths()
    {
        return BirthratePer1000 / 1000f * population;
    }

    private static Int64 birthsUpdate()
    {
        if (population == 0)
            return 0;

        double birthrate = getBirths();

        if ((birthrate > -1 && birthrate < 1) && UnityEngine.Random.value <= Math.Abs(getBirths()))//if birthrate smaller 1 use the % chance
        {
            if (birthrate < 0)
                return -1;
            else
                return 1;
        }
        else if (birthrate <= -1 || birthrate >= 1)//if birthrate >= -+1 => add population
        {
            Int64 births = Mathf.RoundToInt((float)birthrate);
            births = (Int64)UnityEngine.Random.Range(births - (births / 3), births); //66% - 100% of possible births

            return births;
        }

        return 0;
    }


    public static Int64 getHomeless()
    {
        Int64 homeless = population - housingUnits;

        if (homeless < 0)
            return 0;

        return homeless;
    }


    public void killNonHabitablePopulation(float percent)
    {
        Building[] allBuildings = BuildingHandler.allInstantiatedBuildings.ToArray();
        Int64 tmpVal = 0;
        for (int i = 0; i < allBuildings.Length; i++)//get not in habitat living population
        {
            if (!allBuildings[i].GetComponent<Building>().getIsInHabitat())
                tmpVal += allBuildings[i].GetComponent<Building>().housingUnits.x;
        }

        if (tmpVal > 0)
        {
            tmpVal = (Int64)(Math.Abs(tmpVal - Population) / 100 * Math.Abs(percent));//calculate a part of killing population
            if (tmpVal == 0)
                tmpVal = 1;

            addRemovePopulation(-tmpVal, eAddRemovePopulationReason.byOtherKill);
        }
    }


    public static void registerBuilding(Building newBuilding)
    {
        if (newBuilding.housingUnits.y > 0)
        {
            housingUnitBuildings.Add(newBuilding);

            addRemoveHousingUnits(newBuilding.housingUnits.y);//add housing units

            //add population when population is inserted (e.g. colony ships)
            if (newBuilding.housingUnits.x > 0)
                addRemovePopulation(newBuilding.housingUnits.x, eAddRemovePopulationReason.placedBuilding);
        }

        if (newBuilding.isPopulationCentre)
            populationCentres.Add(newBuilding);

        //add workplaces
        if (newBuilding.workplaces > -1)
            addRemoveWorkplaces(workplaces);
    }

    public static void deregisterBuilding(Building removeBuilding, eAddRemovePopulationReason reason)
    {
        if (removeBuilding.housingUnits.y > 0)
        {
            housingUnitBuildings.Remove(removeBuilding);

            addRemoveHousingUnits(-removeBuilding.housingUnits.y);//add housing units
        }

        if (removeBuilding.isPopulationCentre)
            populationCentres.Remove(removeBuilding);

        //add workplaces
        if (workplaces != -1)
            addRemoveWorkplaces(-workplaces);

        switch (reason)
        {
            case eAddRemovePopulationReason.removedBuilding:
                break;
            case eAddRemovePopulationReason.destroyedBuilding:
                addRemovePopulation(-removeBuilding.housingUnits.y, reason);
                break;
            default:
                break;
        }
    }

}
