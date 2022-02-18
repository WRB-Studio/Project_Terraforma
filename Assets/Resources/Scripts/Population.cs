using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public long totalPopulation;

    [Header("Population habitat infos")]
    public long totalPopulationOutOfHabitat;
    public long totalPopulationInHabitat;

    [Header("Birth infos")]
    public int oneBirthPer = 1000;
    public long totalBirthsPerSecond;

    [Header("Apartment infos")]
    public long totalApartmentUnits;
    public long totalOccupiedApartmentUnits;
    public long totalFreeApartmentUnits;

    [Header("Homeless infos")]
    public long totalHomelessPopulation;
    public long totalHomelessLivingSpace;

    [Header("Job infos")]
    public long totalJobs;
    public long totalEmployed;
    public long totalUnemployed;

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

        long populationToKill = (long)(totalPopulationOutOfHabitat / 100 * percent);
        if (populationToKill == 0)
            populationToKill = 1;

        totalPopulationOutOfHabitat -= populationToKill;
    }

    public static void updateCall()
    {
        instance.handleBirths();

        instance.calculateHomeless();

        instance.calculateEmployed();

        instance.handleSatisfaction();
    }

    private void handleBirths()
    {
        float satisfyedBirthPerThousend = oneBirthPer - (oneBirthPer / 100 * totalSatisfaction);
        float totalBirthsPerSecondFloat = totalPopulation / satisfyedBirthPerThousend;
        totalBirthsPerSecond = (long)totalBirthsPerSecondFloat;

        long newPopulation = totalBirthsPerSecond;

        //when birthsPerSecond larger 0 and smaller 1 birth random birth
        if (totalBirthsPerSecondFloat < 1)
        {
            if (totalBirthsPerSecondFloat > 0 && totalBirthsPerSecondFloat < 1 && UnityEngine.Random.value <= totalBirthsPerSecondFloat)
            {
                newPopulation = 1;
            }
        }

        if (totalFreeApartmentUnits > 0)
        {
            if (newPopulation > 1)
            {
                totalPopulation += (int)UnityEngine.Random.Range(newPopulation - (newPopulation / 4), newPopulation);
                //totalHomelessPopulation += newPopulation;
                //totalUnemployed += newPopulation;
            }
            else if (UnityEngine.Random.value <= 0.75f)
            {
                totalPopulation += newPopulation;
                //totalHomelessPopulation += newPopulation;
                //totalUnemployed += newPopulation;
            }
        }
    }

    private void calculateHomeless()
    {
/*
        //calculate homeless population
        if (totalPopulation > totalApartmentUnits)
        {
            totalHomelessPopulation = totalPopulation - totalApartmentUnits;
        }

        //fill empty apartment units with homeless population
        totalFreeApartmentUnits = totalApartmentUnits - totalOccupiedApartmentUnits;
        if (totalFreeApartmentUnits > 0 && totalHomelessPopulation > 0)
        {
            long populationForApartments = 0;

            if(totalFreeApartmentUnits > totalHomelessPopulation)
            {
                populationForApartments = totalFreeApartmentUnits - totalHomelessPopulation;
            }
            else if(totalFreeApartmentUnits < totalHomelessPopulation || totalFreeApartmentUnits == totalHomelessPopulation)
            {
                populationForApartments = totalFreeApartmentUnits;
            }

            totalHomelessPopulation -= populationForApartments;
            totalOccupiedApartmentUnits += populationForApartments;
            totalFreeApartmentUnits -= populationForApartments;
        }
*/
    }

    private void calculateEmployed()
    {
        /*
        //calculate employedpopulation
        if (totalPopulation > totalJobs)
        {
            totalUnemployed = totalPopulation - totalJobs;
        }

        //fill free jobs with unemployed population
        long freeJobs = totalJobs - totalEmployed;
        if (freeJobs > 0 && totalUnemployed > 0)
        {
            long populationForJobs = 0;

            if (freeJobs > totalUnemployed)
            {
                populationForJobs = freeJobs - totalUnemployed;
            }
            else if (freeJobs < totalUnemployed || freeJobs == totalUnemployed)
            {
                populationForJobs = freeJobs;
            }

            totalUnemployed -= populationForJobs;
            totalJobs -= populationForJobs;
        }
        */
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
}
