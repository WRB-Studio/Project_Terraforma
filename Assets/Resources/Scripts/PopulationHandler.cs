using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationHandler : MonoBehaviour
{
    public double defaultBirthsPerThousend = 1;


    private static Int64 population;
    public static long Population { get => population; }
        

    private static Int64 housingUnits;
    public static long HousingUnits { get => housingUnits; }
       

    private static Int64 workplaces;
    public static long Workplaces { get => workplaces; }
        

    private static Int64 employed;
    public static long Employed { get => employed; }


    private static float satisfaction;
    public static float Satisfaction { get => satisfaction; }


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
        addRemovePopulation(getBirths());

        satisfaction = getSatisfaction();
    }



    public static void addRemovePopulation(Int64 addValue)
    {
        population += addValue;
    }

    public static void addRemoveHousingUnits(Int64 addValue)
    {
        housingUnits += addValue;
    }

    public static void addRemoveWorkplaces(Int64 addValue)
    {
        workplaces += addValue;
    }


    public static double getBirthratePerThousend()
    {
        //The calculation of births per thousand, takes into account satisfaction.
        return instance.defaultBirthsPerThousend + (instance.defaultBirthsPerThousend * getSatisfaction());
    }

    private static Int64 getBirths()
    {
        double birthrate = getBirthratePerThousend();

        if (birthrate < 1 && UnityEngine.Random.value <= birthrate)//if birthrate smaller 1 use the % chance
        {
            return 1;
        }
        else if (birthrate >= 1)//uif birthrate >= 1 add population
        {
            Int64 births = (Int64)(birthrate * (Population / 1000));
            births = (Int64)UnityEngine.Random.Range(births - (births / 3), births); //66% - 100% of possible births

            return births;
        }

        return 0;
    }

    public static Int64 getHomeless()
    {
        if (Population > HousingUnits)
            return Math.Abs(HousingUnits - Population);

        return 0;
    }

    public static Int64 getFreeHousingUnits()
    {
        if (Population < HousingUnits)
            return HousingUnits - Population;

        return 0;
    }

    public static Int64 getEmployed()
    {
        return population - getUnemployed();
    }

    public static Int64 getUnemployed()
    {
        long unemployed = Workplaces - Population;

        if (unemployed < 0)
            return Math.Abs(unemployed);

        return 0;
    }

    public static float getSatisfaction()
    {
        float homelessInPercent = getHomeless() / (population / 100);//homeless in percent
        if (homelessInPercent == 0)
            homelessInPercent = 10;

        float unemployedInPercent = getUnemployed() / (population / 100);//unemployed in percent
        if (homelessInPercent == 0)
            homelessInPercent = 10;

        float avg = (homelessInPercent + unemployedInPercent) / 2;

        return Mathf.Clamp(avg, -99, 99);
    }


    public void killNonHabitablePopulation(float percent)
    {
        Building[] allBuildings = BuildingHandler.allInstantiatedBuildings.ToArray();
        Int64 tmpVal = 0;
        for (int i = 0; i < allBuildings.Length; i++)
        {
            if (!allBuildings[i].GetComponent<Building>().getIsInHabitat())
                tmpVal += allBuildings[i].GetComponent<Building>().maxHousingUnits;
        }

        if(tmpVal > 0)
        {
            tmpVal = (Int64)(Math.Abs(tmpVal - Population) / 100 * Math.Abs(percent));
            if (tmpVal == 0)
                tmpVal = 1;

            addRemovePopulation(-tmpVal);
        }


    }

}
