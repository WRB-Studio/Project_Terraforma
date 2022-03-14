using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulationInfo : MonoBehaviour
{
    public Text txtBirthrate;
    public Text txtPopulation;
    public Text txtHousingUnits;
    public Text txtHomeless;
    public Text txtWorkplaces;
    public Text txtEmploed;
    public Text txtSatisfaction;

    public static PopulationInfo instance;



    private void Awake()
    {
        instance = this;
    }

    public static void updateCall()
    {
        instance.txtBirthrate.text = Math.Round(PopulationHandler.getBirths(), 2).ToString();

        instance.txtPopulation.text = PopulationHandler.Population.ToString();
        instance.txtHousingUnits.text = PopulationHandler.HousingUnits.ToString();
        instance.txtHomeless.text = PopulationHandler.getHomeless().ToString();
        instance.txtWorkplaces.text = PopulationHandler.Workplaces.ToString();
    }

}
