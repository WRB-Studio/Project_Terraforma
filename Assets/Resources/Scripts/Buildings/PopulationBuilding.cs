using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationBuilding : Building
{
    public Tuple<long, long> housingUnits; //occupid, max units+
    public float satisfaction;
    public bool isCapital;
}
