using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoGUI : GUIInterface
{
    public Building selectedBuilding;

    public Text txtBuildingname;

    public Slider sliderHitpoints;
    public Text txtHitpoints;

    public Text txtBuildingType;
    public Text txtBuildingCategory;
    public Text txtCapital;

    public Text txtTech;
    public Text txtHousingUnits;
    public Text txtWorkplaces;

    public Text txtEnergy;
    public Text txtHasEnergy;

    public Text txtStorage;
    public Text txtProductionItem;

    public Text txtIsActive;
    public Text txtIsConnected;
    public Text txtInHabitat;

    public Text txtOxygen;
    public Text txtGravity;
    public Text txtMagneticField;

    public static BuildingInfoGUI instance;



    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        
    }

    public void refreshBuildingInfo()
    {
        if (selectedBuilding == null)
            return;

        txtBuildingname.text = selectedBuilding.name;

        sliderHitpoints.minValue = 0;
        sliderHitpoints.maxValue = selectedBuilding.hitPoints.y;
        sliderHitpoints.value = selectedBuilding.hitPoints.x;
        txtHitpoints.text = selectedBuilding.hitPoints.x.ToString();

        txtBuildingType.text = "Type: " + selectedBuilding.buildingType.ToString();
        txtBuildingCategory.text = "Category: " + selectedBuilding.buildingCategory.ToString();
        txtTech.text = "Tech: " + selectedBuilding.buildingTech.ToString();

        txtCapital.text = "Capital: " + selectedBuilding.isCapital.ToString();

        txtHousingUnits.text = "Housing units: " + selectedBuilding.housingUnits.y.ToString();
        txtWorkplaces.text = "Workplaces: " + selectedBuilding.workplaces.ToString();

        txtEnergy.text = "Energy: " + selectedBuilding.energy_Production_Consuming.ToString();
        txtHasEnergy.text = "Has energy: " + selectedBuilding.HasEnergy.ToString();

        txtStorage.text = "Storage: " + selectedBuilding.storage.ToString();
        txtProductionItem.text = "Production: " + selectedBuilding.productionItem.ToString();

        txtIsActive.text = "Active : " + selectedBuilding.IsActivated.ToString();
        txtIsConnected.text = "Connected: " + selectedBuilding.isConnected.ToString();
        txtInHabitat.text = "Habitat: " + selectedBuilding.inHabitat.ToString();

        txtOxygen.text = "Oxygen: " + selectedBuilding.oxygenAvailable.ToString();
        txtGravity.text = "Gravity: " + selectedBuilding.gravityAvailable.ToString();
        txtMagneticField.text = "Magnetic field: " + selectedBuilding.magneticFieldAvailable.ToString();      

    }


}
