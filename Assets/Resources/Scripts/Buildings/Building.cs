using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum eBuildingType
    {
        None, Planetar, Orbital
    }

    public enum eBuildingCategory
    {
        None, Population, Mining, Production, Special, Defense, Terraforming,
    }

    public eBuildingType buildingType = eBuildingType.None;
    public eBuildingCategory buildingCategory = eBuildingCategory.None;

    [Header("Some bools")]
    public bool inUse;
    public bool canStandAlone;
    public bool isConnected;
    public bool oxygenAvailable;
    public bool gravityAvailable;
    public bool magneticFieldAvailable;

    [Header("Hitpoints")]
    public Vector2Int hitPoints;
    public float realAVGStaticLoad = 3.25f; //Average calculations of real building loads. (kN/m²)
    public float fictiveMaterialBonus = 1.25f; //fictive material bonus for building loads. (kN/m²)

    [Header("Needs")]
    public Vector2Int energy;//arrive, needed

    [Header("Workplaces")]
    public Vector2Int workplaces; //occupied places, max places
    public float worplaceFactor = 1;

    [Header("Construction")]
    public Tuple<int, RessourceHandler.eRessources> constructionRessources;
    public float constructionRessourcesFactor;
    public Vector2 constructionTime;//countdown, max time
    public float constructionTimeFactor;

    public float buildingAltitude;//altitude from lowest point of the planet



    public void calculateBuildingAltitude()
    {
        // = difference to smalles point
    }

    public void calculateConstructionTime()
    {
        //constructionTime = Building volume * constructionTimeFactor
    }

    public void calculateConstructionRessources()
    {
        //constructionRessources = Building volume * constructionRessourcesBalanceFactor
    }

    public void calculateWorkplaces()
    {
        //workplaces = Building volume * building footprint (x * y) / 4 * worplaceFactor
    }

    public void calculateEnergy()
    {
        //default energy consum = people * flat rate energy consum for private people 5.5 kWh

        //other building energy consum = default energy consum + building footprint (x * y) * flat rate energy consum for other buildings (25 kWh)
    }

    public void checkDestroyingBuilding()
    {
        if (hitPoints.x <= 0)
            Destroy(gameObject);
    }

    public void calculateHitPoints()
    {
        hitPoints.y = Mathf.RoundToInt(realAVGStaticLoad + fictiveMaterialBonus); // * buildingHigh);
    }

    public void repair()
    {

    }
}
