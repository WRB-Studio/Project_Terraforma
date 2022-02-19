using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationBuilding : Building
{
    public long housingUnits;
    public Tuple<long, long> housingUnits2; //occupid, max units+
    public float satisfaction;
    public bool isCapital;

    public override void onBuild(bool inBuildModeVal)
    {
        base.onBuild(inBuildModeVal);

        if (!inBuildMode)
        {
            Population.instance.totalApartmentUnits += housingUnits;
           
            if (name.Contains("Colony Ship"))
            {
                Population.instance.totalPopulation += housingUnits;

            }
        }

        

    }
}
