using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public long totalPopulation;

    [Header("Population habitat infos")]
    public long totalPopulationInHabitat;

    [Header("Birth infos")]
    public int oneBirthPer = 1000;
    public double totalBirthsPerSecond;

    [Header("Housing unit infos")]
    public long totalHousingUnits;

    [Header("Workplace infos")]
    public long totalWorkplaces;
    public long totalEmployed;

    [Header("Satisfaction infos")]
    public float totalSatisfaction;


    public static Population instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        
    }


    public void killNonHabitablePopulation(float percent)
    {
        percent = Math.Abs(percent);

        long populationToKill = (long)(Math.Abs(totalPopulationInHabitat - totalPopulation) / 100 * percent);
        if (populationToKill == 0)
            populationToKill = 1;

        removePopulation(populationToKill);
    }

    public static void updateCall()
    {
        instance.handleBirths();

        instance.handleSatisfaction();
    }

    public double calculateBirthPerSecond()
    {
        double satisfyedBirthPerThousend = oneBirthPer - (oneBirthPer / 100 * totalSatisfaction);
        return Math.Round(totalPopulation / satisfyedBirthPerThousend, 2);
    }

    private void handleBirths()
    {
        totalBirthsPerSecond = calculateBirthPerSecond();

        long newPopulation = (long)Math.Round(totalBirthsPerSecond, 0);

        //when birthsPerSecond larger 0 and smaller 1 birth random birth
        if (totalBirthsPerSecond < 1)
        {
            if (totalBirthsPerSecond > 0 && totalBirthsPerSecond < 1 && UnityEngine.Random.value <= totalBirthsPerSecond)
            {
                newPopulation = 1;
            }
        }

        if (getFreeHousingUnits() > 0)
        {
            if (newPopulation > 1)
            {
                addPopulation((long)UnityEngine.Random.Range(newPopulation - (newPopulation / 4), newPopulation));
            }
            else if (UnityEngine.Random.value <= 0.75f)
            {
                addPopulation(newPopulation);
            }
        }
    }

    private void handleSatisfaction()
    {
        float satisfactionByHomelessPopulation = 0;
        float satisfactionByUnemployedPopulation = 0;
        /*
        if (totalPopulation > 0)
        {
            satisfactionByHomelessPopulation = -(totalHomelessPopulation / (totalPopulation / 100));
            satisfactionByUnemployedPopulation = -(totalPopulation - totalEmployed / (totalPopulation / 100));
        }
        */
        totalSatisfaction = Mathf.Clamp(satisfactionByHomelessPopulation + satisfactionByUnemployedPopulation, -99, 99);
    }



    public void addPopulation(long newIndividuals)
    {
        totalPopulation += newIndividuals;

        fillHousingUnits(newIndividuals);
        fillWorkplaces();
    }

    public void removePopulation(long populationToKill)
    {
        Debug.LogWarning("removing people is not implemented");
    }

    public void addHousingUnits(long newHousingUnits)
    {
        totalHousingUnits += newHousingUnits;

        fillHousingUnits(getHomelessPopulation());
    }

    public void addWorkplaces(long newWorkplaces)
    {
        totalWorkplaces += newWorkplaces;

        fillWorkplaces();
    }


    public long fillHousingUnits(long individuals)
    {
        if (individuals == 0)
        {
            return 0;
        }

        long rest = 0;
        long individualsForMoving;
        long freeHousingUnits;

        for (int i = 0; i < Building.allBuilding.Count; i++)
        {
            if (Building.allBuilding[i].GetComponent<PopulationBuilding>())
            {
                PopulationBuilding currentBuilding = Building.allBuilding[i].GetComponent<PopulationBuilding>();

                freeHousingUnits = currentBuilding.maxHousingUnits - currentBuilding.occupidHousingUnits;
                if (freeHousingUnits > 0)
                {
                    rest = Math.Abs(freeHousingUnits - individuals);
                    individualsForMoving = individuals - rest;
                    currentBuilding.occupidHousingUnits += individualsForMoving;

                    individuals = rest;
                }
            }
        }

        return rest;
    }

    public void fillWorkplaces()
    {
        if(totalPopulation >= totalWorkplaces)
        {
            totalEmployed = totalWorkplaces;
        }
        else if (totalPopulation < totalWorkplaces)
        {
            totalEmployed = totalPopulation;
        }
    }


    public long getFreeHousingUnits()
    {
        if(totalPopulation < totalHousingUnits)
        {
            return totalHousingUnits - totalPopulation;
        }

        return 0;
    }

    public long getUnemployedPopulation()
    {
        long unemployed = totalWorkplaces - totalPopulation;

        if (unemployed < 0)
        {
            return Math.Abs(unemployed);
        }

        return 0;
    }

    public long getHomelessPopulation()
    {
        if(totalPopulation > totalHousingUnits)
            return Math.Abs(totalHousingUnits - totalPopulation);

        return 0;
    }

    public long getNotInHabitatPopulation()
    {
        if (totalPopulation > totalPopulationInHabitat)
            return Math.Abs(totalPopulationInHabitat - totalPopulation);

        return 0;
    }

}
