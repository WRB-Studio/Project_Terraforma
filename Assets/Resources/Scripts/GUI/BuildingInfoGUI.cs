using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoGUI : GUIInterface
{
    public Building selectedBuilding;

    public Text txtBuildingname;

    public Slider sliderHitpoints;

    public Text txtBuildingType;
    public Text txtBuildingCategory;
    public Image imgCapital;

    public Text txtTech;
    public Text txtHousingUnits;
    public Text txtWorkplaces;

    public Text txtEnergy;
    public Image imgEnergy;

    public Text txtStorage;
    public Text txtComponents;
    public Text txtProduction;
    public Slider productionTimeSlider;
    public Text txtProductionTime; 
    public Image imgProductionGear;
    public GameObject productionContainer;


    public Image imgPower;
    public Image imgIsConnected;
    public Image imgInHabitat;

    public Image imgOxygen;
    public Image imgGravity;
    public Image imgMagneticField;


    public static BuildingInfoGUI instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {

    }

    public static void updateCall()
    {
        if (GUIHandler.instance.buildingInfoPanel.activeSelf && instance.selectedBuilding != null)
        {
            instance.refreshBuildingInfo();
        }
    }

    public void refreshBuildingInfo()
    {
        if (selectedBuilding == null)
            return;

        txtBuildingname.text = selectedBuilding.name;

        sliderHitpoints.minValue = 0;
        sliderHitpoints.maxValue = selectedBuilding.hitPoints.y;
        sliderHitpoints.value = selectedBuilding.hitPoints.x;

        txtBuildingType.text = "Type: " + selectedBuilding.buildingType.ToString();
        txtBuildingCategory.text = "Category: " + selectedBuilding.buildingCategory.ToString();
        txtTech.text = "Tech: " + selectedBuilding.buildingTech.ToString();

        //capital handling
        if (selectedBuilding.isPopulationCentre)
        {
            imgCapital.gameObject.SetActive(true);

            if (selectedBuilding.isCapital)
                imgCapital.color = Color.white;
            else
                imgCapital.color = Color.grey;
        }
        else
        {
            imgCapital.gameObject.SetActive(false);
        }

        //housing units handling
        if (selectedBuilding.housingUnits.y > 0)
        {
            txtHousingUnits.transform.parent.gameObject.SetActive(true);
            txtHousingUnits.text = selectedBuilding.housingUnits.y.ToString();
        }
        else
        {
            txtHousingUnits.transform.parent.gameObject.SetActive(false);
        }

        //workplace handling
        if (selectedBuilding.workplaces > 0)
        {
            txtWorkplaces.transform.parent.gameObject.SetActive(true);
            txtWorkplaces.text = selectedBuilding.workplaces.ToString();
        }
        else
        {
            txtWorkplaces.transform.parent.gameObject.SetActive(false);
        }

        //Energy handlings
        txtEnergy.text = selectedBuilding.energy_Production_Consuming.ToString();
        if (selectedBuilding.HasEnergy)
            imgEnergy.color = Color.green;
        else
            imgEnergy.color = Color.red;

        //storeag handling
        if (selectedBuilding.storage > 0)
        {
            txtStorage.transform.parent.gameObject.SetActive(true);
            txtStorage.text = selectedBuilding.storage.ToString();
        }
        else
        {
            txtStorage.transform.parent.gameObject.SetActive(false);
        }

        //Production handling
        if(selectedBuilding.productionItem.input.Length != 0 || selectedBuilding.productionItem.output.resourceType != ResourceHandler.eResources.None)
        {
            productionContainer.SetActive(true);

            for (int i = 0; i < selectedBuilding.productionItem.input.Length; i++)
            {
                txtComponents.text = selectedBuilding.productionItem.input[i].resourceType.ToString() + "\n";
            }

            txtProduction.text = selectedBuilding.productionItem.output.resourceType.ToString();

            productionTimeSlider.minValue = 0;
            productionTimeSlider.maxValue = selectedBuilding.productionItem.productionTime;
            productionTimeSlider.value = selectedBuilding.productionItem.Countdown;

            txtProductionTime.text = selectedBuilding.productionItem.Countdown.ToString();

            if(!selectedBuilding.productionItem.IsHolding || selectedBuilding.productionItem.IsProducing)
            {
                imgProductionGear.color = Color.green;
                imgProductionGear.GetComponent<Animator>().speed = 1;
            }
            else
            {
                imgProductionGear.color = Color.red;
                imgProductionGear.GetComponent<Animator>().speed = 0;
            }
        }
        else
        {
            productionContainer.SetActive(false);
        }

        //Power handling
        if (selectedBuilding.IsActivated)
        {
            imgPower.color = Color.green;
        }
        else
        {
            imgPower.color = Color.red;
        }

        //Connected handling
        if (selectedBuilding.IsConnected)
        {
            imgIsConnected.color = Color.green;
        }
        else
        {
            imgIsConnected.color = Color.red;
        }

        //in Habitat handling
        if (selectedBuilding.canStandAlone)
        {
            imgInHabitat.transform.parent.gameObject.SetActive(true);
            if (selectedBuilding.IsInHabitat)
            {
                imgInHabitat.color = Color.green;
            }
            else
            {
                imgInHabitat.color = Color.red;
            }
        }
        else
        {
            imgInHabitat.transform.parent.gameObject.SetActive(false);
        }

        //oxygen handling
        if (selectedBuilding.OxygenAvailable)
        {
            imgOxygen.color = Color.green;
        }
        else
        {
            imgOxygen.color = Color.red;
        }

        //gravity handling
        if (selectedBuilding.GravityAvailable)
        {
            imgGravity.color = Color.green;
        }
        else
        {
            imgGravity.color = Color.red;
        }

        //magnetic field handling
        if (selectedBuilding.MagneticFieldAvailable)
        {
            imgMagneticField.color = Color.green;
        }
        else
        {
            imgMagneticField.color = Color.red;
        }

    }

    

}
