using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    public GameObject goBuildingMenu;
    public GameObject goTech;
    public GameObject goCategory;
    public GameObject goBuildingChooser;
    public GameObject goBuildingList;

    public Button btTec1;
    public Button btTec2;
    public Button btTec3;
    public Button btTec4;

    public Button btPopulation;
    public Button btMining;
    public Button btProduction;
    public Button btSpecial;
    public Button btDefense;
    public Button btTerraforming;

    public Building.eTechType currentActiveTec = Building.eTechType.Tech_1;
    public Building.eBuildingCategory currentActiveCategory = Building.eBuildingCategory.Population;

    public static BuildingMenu instance;


    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        instance.initButtons();

        instance.showHide(Building.eTechType.Tech_1, Building.eBuildingCategory.Population);
    }

    private void initButtons()
    {
        btTec1.onClick.AddListener(delegate { showHide(Building.eTechType.Tech_1, currentActiveCategory); });
        btTec2.onClick.AddListener(delegate { showHide(Building.eTechType.Tech_2, currentActiveCategory); });
        btTec3.onClick.AddListener(delegate { showHide(Building.eTechType.Tech_3, currentActiveCategory); });
        btTec4.onClick.AddListener(delegate { showHide(Building.eTechType.Tech_4, currentActiveCategory); });

        btPopulation.onClick.AddListener(delegate { showHide(currentActiveTec, Building.eBuildingCategory.Population); });
        btMining.onClick.AddListener(delegate { showHide(currentActiveTec, Building.eBuildingCategory.Mining); });
        btProduction.onClick.AddListener(delegate { showHide(currentActiveTec, Building.eBuildingCategory.Production); });
        btSpecial.onClick.AddListener(delegate { showHide(currentActiveTec, Building.eBuildingCategory.Special); });
        btDefense.onClick.AddListener(delegate { showHide(currentActiveTec, Building.eBuildingCategory.Defense); });
        btTerraforming.onClick.AddListener(delegate { showHide(currentActiveTec, Building.eBuildingCategory.Terraforming); });
    }

    private void showHide(Building.eTechType tecType, Building.eBuildingCategory category)
    {
        for (int i = 0; i < goBuildingList.transform.childCount;i++)
        {
            Building building = goBuildingList.transform.GetChild(i).GetComponent<BuildingButton>().buildingPrefab.GetComponent<Building>();

            if(building.buildingCategory == category && building.buildingTech == tecType)
            {
                goBuildingList.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                goBuildingList.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

}
