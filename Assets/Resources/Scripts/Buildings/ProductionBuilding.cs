using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    public float Productivity;

    public List<Tuple<RessourceHandler.eRessources, int>> productionResources; //the production products
    public List<Tuple<RessourceHandler.eRessources, int>> manufacturingResources; //needed products for production


    public void calculateProductivity()
    {
        //Hitpoints-%, Satisfaction, occupied worplaces-%
    }

}
