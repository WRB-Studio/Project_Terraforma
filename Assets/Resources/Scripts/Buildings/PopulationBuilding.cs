using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationBuilding : Building
{
    public long maxHousingUnits;
    public long occupidHousingUnits;
    public float satisfaction;
    public bool isCapital;

    public override void onBuild(bool inBuildModeVal)
    {
        base.onBuild(inBuildModeVal);

        if (!inBuildMode)
        {
            Population.instance.addHousingUnits(maxHousingUnits);
           
            if (name.Contains("Colony Ship"))
            {
                Population.instance.addPopulation(maxHousingUnits);
            }
        }
    }
}
