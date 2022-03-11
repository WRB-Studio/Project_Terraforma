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
    public long maxHousingUnits;
    public bool isCapital;
    public bool isPopulationCentre;

    [Header("Production")]
    public int storage;
    public int energy_Production_Consuming = 0; //+ = producing, - = consuming
    public float Productivity;
    public ProductionItem productionItem; //production infos

    [Header("Workplaces")]
    public int workplaces;
    public static float workplaceFactor = 10;

    [Header("Some bools")]
    public bool isActivated;
    public bool hasEnergy;
    public bool canStandAlone;
    public bool isConnected;
    public bool isInHabitat;
    public bool oxygenAvailable;
    public bool gravityAvailable;
    public bool magneticFieldAvailable;

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
    }


    public void calculateBuildingAltitude()
    {
        buildingAltitude = (float)Math.Round((Vector3.Distance(PlanetAttribute.planetModell.position, transform.position) - PlanetAttribute.minMaxPlanetHeight.x) * 100, 2);
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

    public void calculateProductivity()
    {
        //Hitpoints-%, Satisfaction, occupied worplaces-%
    }

    public void calculateNeededEnergy()
    {
        //population building energy consum = people * flat rate energy consum for private people 5.5 kWh
        //other building energy consum = default energy consum + building footprint (x * y) * flat rate energy consum for other buildings (25 kWh)
        int energyConsumeByWorkplace = Mathf.RoundToInt(workplaces * 2.5f);
        int energyConsumeByHousingUnit = Mathf.RoundToInt(maxHousingUnits * 5.5f);
        int energyConsumMining = 20;
        int energyConsumProduction = 25;
        int energyConsumSpecial = 30;
        int energyConsumTerraforming = 40;
        int energyConsumDefense = 50;
        int buildingFloorSize = Mathf.RoundToInt(getFloorSize(gameObject));

        switch (buildingCategory)
        {
            case eBuildingCategory.None:
                break;
            case eBuildingCategory.Population:
                energy_Production_Consuming = Mathf.RoundToInt(energyConsumeByHousingUnit);
                break;
            case eBuildingCategory.Mining:
                energy_Production_Consuming = Mathf.RoundToInt(energyConsumeByWorkplace + buildingFloorSize * energyConsumMining);
                break;
            case eBuildingCategory.Production:
                energy_Production_Consuming = Mathf.RoundToInt(energyConsumeByWorkplace + buildingFloorSize * energyConsumProduction);
                break;
            case eBuildingCategory.Special:
                energy_Production_Consuming = Mathf.RoundToInt(energyConsumeByWorkplace + buildingFloorSize * energyConsumSpecial);
                break;
            case eBuildingCategory.Defense:
                energy_Production_Consuming = Mathf.RoundToInt(energyConsumeByWorkplace + buildingFloorSize * energyConsumDefense);
                break;
            case eBuildingCategory.Terraforming:
                energy_Production_Consuming = Mathf.RoundToInt(energyConsumeByWorkplace + buildingFloorSize * energyConsumTerraforming);
                break;
            default:
                break;
        }

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

            //isnt a power plant
            if (energy_Production_Consuming < 0)
                calculateNeededEnergy();

            //if building has housing units
            if (maxHousingUnits > 0)
                PopulationHandler.addRemoveHousingUnits(maxHousingUnits);//add housing units

            //add population when colony ship
            if (name.Contains("Colony Ship"))
                PopulationHandler.addRemovePopulation(maxHousingUnits);

            //calculate/add worlplaces
            if (workplaces != -1)
            {
                calculateWorkplaces();
                PopulationHandler.addRemoveWorkplaces(workplaces);
            }

            BuildingChooser.instance.refreshBuildingChooser();
        }
    }

    public void removeBuilding()
    {
        BuildingHandler.removeBuilding(this);
    }


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


    public bool getIsInHabitat()
    {
        return isInHabitat;
    }

}
