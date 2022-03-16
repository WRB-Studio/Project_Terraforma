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

    public enum eTechType
    {
        None, Tech_1, Tech_2, Tech_3, Tech_4,
    }


    public eBuildingType buildingType = eBuildingType.None;
    public eBuildingCategory buildingCategory = eBuildingCategory.None;
    public eTechType buildingTech = eTechType.None;
    public bool unlocked;


    [Header("Hitpoints")]
    public Vector2Int hitPoints;
    public float realAVGStaticLoad = 3.25f; //Average calculations of real building loads. (kN/m²)
    public float fictiveMaterialBonus = 1.25f; //fictive material bonus for building loads. (kN/m²)


    [Header("Population")]
    public Vector2Int housingUnits;//occupied by start; available housing units
    public bool isCapital;
    public bool isPopulationCentre;


    [Header("Workplaces")]
    public int workplaces;
    public static float workplaceFactor = 10;


    [Header("Production")]
    public int storage;
    public int energy_Production_Consuming = 0; //+ = producing, - = consuming
    public ProductionItem productionItem = null; //production infos


    [Header("Some bools")]
    public bool canStandAlone;
    public bool canDisabled;

    private bool isActivated;
    public bool IsActivated { get { return isActivated; } set { setIsActivated(value); } }

    private bool hasEnergy;
    public bool HasEnergy { get; set; }

    private bool isConnected;
    public bool IsConnected { get; set; }

    private bool inHabitat;
    public bool IsInHabitat { get { return inHabitat; } set { getIsInHabitat(); } }

    private bool oxygenAvailable;
    public bool OxygenAvailable { get; set; }

    private bool gravityAvailable;
    public bool GravityAvailable { get; set; }

    private bool magneticFieldAvailable;
    public bool MagneticFieldAvailable { get; set; }


    [Header("Construction")]
    public Tuple<int, ResourceHandler.eResources> constructionRessources;
    public float constructionRessourcesFactor;
    public Vector2 constructionTime;//countdown, max time
    public float constructionTimeFactor;

    public float buildingAltitude;//altitude from lowest point of the planet

    //Build mode
    public bool inBuildMode;
    public bool isColliding;
    private Material[] originMaterials;



    private void Awake()
    {
        originMaterials = GetComponent<MeshRenderer>().materials;
        init();
    }


    #region start initializations

    private void init()
    {
        calculateHitPoints();

        //calculate workplaces
        if (workplaces != -1)
            calculateWorkplaces();

        calculateEnergy();

        calculateConstructionTime();

        calculateConstructionRessources();

        IsActivated = true;
    }


    public void calculateBuildingAltitude()
    {
        buildingAltitude = (float)Math.Round((Vector3.Distance(PlanetAttribute.planetModell.position, transform.position) - PlanetAttribute.minMaxPlanetHeight.x) * 100, 2);
    }

    public void calculateConstructionTime()
    {
        constructionTime.y = getVolumeOfBuilding(gameObject) * constructionTimeFactor;
    }

    public void calculateConstructionRessources()
    {
        //constructionRessources = Building volume * constructionRessourcesBalanceFactor
    }

    public void calculateWorkplaces()
    {
        //workplaces = Building volume * (building footprint (x * y) / 4) * worplaceFactor
        float result = 0;
        if (workplaces != -1)
            result = (getVolumeOfBuilding(gameObject) * (getFloorSize(gameObject) / 4) * workplaceFactor);

        /*
        Debug.Log("Volume =" + getVolumeOfBuilding(gameObject) + "\n" +
                  "Floor size = " + getFloorSize(gameObject) + " (/4 = "+ getFloorSize(gameObject) / 4 + ")\n" + 
                  "Workplacefactor = " + workplaceFactor + "\n" + 
                  "Volume * (Floor size / 4) * workplacefactor = " + result);
        */

        workplaces = (int)Math.Round(result, 0);
    }

    public void calculateEnergy()
    {
        //population building energy consum = people * flat rate energy consum for private people 5.5 kWh
        //other building energy consum = default energy consum + building footprint (x * y) * flat rate energy consum for other buildings (25 kWh)
        int energyProductionBYPowerPlant = Mathf.RoundToInt(workplaces * 1.5f * 30);
        int energyConsumeByWorkplace = Mathf.RoundToInt(workplaces * 5.5f);
        int energyConsumeByHousingUnit = Mathf.RoundToInt(housingUnits.y * 2.5f);
        int energyConsumPopulation = 10;
        int energyConsumMining = 20;
        int energyConsumProduction = 25;
        int energyConsumSpecial = 30;
        int energyConsumTerraforming = 40;
        int energyConsumDefense = 50;
        int buildingFloorSize = Mathf.RoundToInt(getFloorSize(gameObject));
        int buildingVolume = Mathf.RoundToInt(getVolumeOfBuilding(gameObject));

        
        if (energy_Production_Consuming < 0)//isnt a power plant
        {
            switch (buildingCategory)
            {
                case eBuildingCategory.None:
                    break;
                case eBuildingCategory.Population:
                    energy_Production_Consuming = -Mathf.RoundToInt(energyConsumeByHousingUnit + buildingVolume * energyConsumPopulation);
                    break;
                case eBuildingCategory.Mining:
                    energy_Production_Consuming = -Mathf.RoundToInt(energyConsumeByWorkplace + buildingVolume * energyConsumMining);
                    break;
                case eBuildingCategory.Production:
                    energy_Production_Consuming = -Mathf.RoundToInt(energyConsumeByWorkplace + buildingVolume * energyConsumProduction);
                    break;
                case eBuildingCategory.Special:
                    energy_Production_Consuming = -Mathf.RoundToInt(energyConsumeByWorkplace + buildingVolume * energyConsumSpecial);
                    break;
                case eBuildingCategory.Defense:
                    energy_Production_Consuming = -Mathf.RoundToInt(energyConsumeByWorkplace + buildingVolume * energyConsumDefense);
                    break;
                case eBuildingCategory.Terraforming:
                    energy_Production_Consuming = -Mathf.RoundToInt(energyConsumeByWorkplace + buildingVolume * energyConsumTerraforming);
                    break;
                default:
                    break;
            }
        }
        else if (energy_Production_Consuming > 0)//is power plant
        {
            energy_Production_Consuming = +Mathf.RoundToInt(energyProductionBYPowerPlant + buildingVolume); 
        }
    }

    public void calculateHitPoints()
    {
        hitPoints.y = (int)Math.Round(realAVGStaticLoad + fictiveMaterialBonus * getVolumeOfBuilding(gameObject), 0); // * buildingHigh);
        hitPoints.x = hitPoints.y;
    }

    public float getFloorSize(GameObject buildingObject)
    {
        //Fetch the Collider from the GameObject
        Collider m_Collider = buildingObject.GetComponent<Collider>();

        //Fetch the size of the Collider volume
        Vector3 m_Size = m_Collider.bounds.size;

        return m_Size.x * m_Size.z;
    }

    public float getVolumeOfBuilding(GameObject buildingObject)
    {
        Mesh mesh = buildingObject.GetComponent<MeshFilter>().mesh;

        float volume = 0;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
        return Mathf.Abs(volume);
    }

    public float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;

        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }

    #endregion


    public bool getIsInHabitat()
    {
        return inHabitat;
    }

    private void setIsActivated(bool activate)
    {
        if (!canDisabled)
        {
            isActivated = true;
            activateWorkplaces(true);
            activateHousingUnits(true);
            return;
        }

        if (activate)
        {
            isActivated = true;
            activateWorkplaces(true);
            activateHousingUnits(true);

            //if (hasEnergy)
            //{
            //    isActivated = true;
            //    activateWorkplaces(true);
            //    activateHousingUnits(true);
            //}
            //else
            //{
            //    isActivated = false;
            //    activateWorkplaces(false);
            //    activateHousingUnits(false);
            //}
        }
        else
        {
            isActivated = false;
            activateWorkplaces(false);
            activateHousingUnits(false);
        }
    }

    private void activateWorkplaces(bool activate)
    {
        if (activate)
        {
            PopulationHandler.addRemoveWorkplaces(workplaces);
        }
        else
        {
            PopulationHandler.addRemoveWorkplaces(-workplaces);
        }
    }

    private void activateHousingUnits(bool activate)
    {
        if (activate)
        {
            PopulationHandler.addRemoveHousingUnits(housingUnits.y);
        }
        else
        {
            PopulationHandler.addRemoveHousingUnits(-housingUnits.y);
        }
    }

    public void repair()
    {

    }

    public void checkDestroyingBuilding(PopulationHandler.eAddRemovePopulationReason reason)
    {
        if (hitPoints.x <= 0)
            BuildingHandler.deregisterBuilding(this, reason);
    }



    #region build methodes

    public bool isBuildable()
    {
        Material[] materials;

        if (isColliding)
        {
            //set red color when collisions
            materials = GetComponent<MeshRenderer>().materials;
            foreach (Material material in materials)
                material.SetColor("_BaseColor", ObjectPlacement.instance.cantBuildColor);
            GetComponent<MeshRenderer>().materials = materials;
            return false;
        }
        else
        {
            //set green color when no collisions
            materials = GetComponent<MeshRenderer>().materials;
            foreach (Material material in materials)
                material.SetColor("_BaseColor", ObjectPlacement.instance.canBuildColor);
            GetComponent<MeshRenderer>().materials = materials;
            return true;
        }
    }

    public virtual void onBuild(bool inBuildModeVal)
    {
        if (inBuildModeVal)
        {
            inBuildMode = true;

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.freezeRotation = true;

            isBuildable();
        }
        else
        {
            inBuildMode = false;

            Destroy(GetComponent<Rigidbody>());
            GetComponent<MeshRenderer>().materials = originMaterials;

            calculateBuildingAltitude();//building height on planet

            ////calculate workplaces
            //if (workplaces != -1)
            //    calculateWorkplaces();

            if (productionItem != null)
                productionItem.building = this;

            IsActivated = true;


            BuildingHandler.registerBuilding(this);
            BuildingChooser.instance.refreshBuildingChooser();
        }
    }

    public void removeBuilding(PopulationHandler.eAddRemovePopulationReason reason)
    {
        BuildingHandler.deregisterBuilding(this, reason);
    }

    #endregion


    private void OnCollisionEnter(Collision collision)
    {
        if (inBuildMode && collision.gameObject.tag == "Building")
        {
            isColliding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (inBuildMode && collision.gameObject.tag == "Building")
        {
            isColliding = false;
        }
    }


    private void OnMouseDown()
    {
        if (GUIHandler.instance.buildingInfoPanel.activeSelf && BuildingInfoGUI.instance.selectedBuilding == this)
        {
            BuildingInfoGUI.instance.showHideGUI();
        }
        else if (GUIHandler.instance.buildingInfoPanel.activeSelf && BuildingInfoGUI.instance.selectedBuilding != this)
        {
            BuildingInfoGUI.instance.selectedBuilding = this;
            BuildingInfoGUI.instance.refreshBuildingInfo();
        }
        else if (!GUIHandler.instance.buildingInfoPanel.activeSelf)
        {
            BuildingInfoGUI.instance.showHideGUI();
            BuildingInfoGUI.instance.selectedBuilding = this;
            BuildingInfoGUI.instance.refreshBuildingInfo();
        }
    }

}
