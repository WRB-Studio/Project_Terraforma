using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChooser : GUIInterface
{
    public GameObject goBuildingMenu;
    public GameObject goTech;
    public GameObject goCategory;
    public GameObject goBuildingChooser;

    public Transform buildingList;
    public Transform buildingsTec1;
    public Transform buildingsTec2;
    public Transform buildingsTec3;
    public Transform buildingsTec4;

    //public Button btTec1;
    //public Button btTec2;
    //public Button btTec3;
    //public Button btTec4;

    public Button btPopulation;
    public Button btMining;
    public Button btProduction;
    public Button btSpecial;
    public Button btDefense;
    public Button btTerraforming;

    public Building.eBuildingCategory currentActiveCategory = Building.eBuildingCategory.Population;

    private List<GameObject> allBuildingButtons = new List<GameObject>();

    public static BuildingChooser instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        instance.initButtons();

        //list all building buttons
        for (int i = 0; i < instance.buildingList.childCount; i++)
        {
            instance.allBuildingButtons.Add(instance.buildingList.GetChild(i).gameObject); ;
        }

        instance.sortBuildingList();

        instance.showCategory(Building.eBuildingCategory.Population);
    }


    private void sortBuildingList()
    {
        for (int i = 0; i < instance.allBuildingButtons.Count; i++)
        {
            GameObject btBuildingGo = instance.allBuildingButtons[i];
            Building buildingScrp = btBuildingGo.GetComponent<BuildingButton>().buildingPrefab.GetComponent<Building>();

            if (buildingScrp.buildingTech == Building.eTechType.Tech_1)
                btBuildingGo.transform.SetParent(instance.buildingsTec1);

            if (buildingScrp.buildingTech == Building.eTechType.Tech_2)
                btBuildingGo.transform.SetParent(instance.buildingsTec2);

            if (buildingScrp.buildingTech == Building.eTechType.Tech_3)
                btBuildingGo.transform.SetParent(instance.buildingsTec3);

            if (buildingScrp.buildingTech == Building.eTechType.Tech_4)
                btBuildingGo.transform.SetParent(instance.buildingsTec4);
        }
    }

    private void initButtons()
    {
        //btTec1.onClick.AddListener(delegate { showHide(Building.eTechType.Tech_1, currentActiveCategory); });
        //btTec2.onClick.AddListener(delegate { showHide(Building.eTechType.Tech_2, currentActiveCategory); });
        //btTec3.onClick.AddListener(delegate { showHide(Building.eTechType.Tech_3, currentActiveCategory); });
        //btTec4.onClick.AddListener(delegate { showHide(Building.eTechType.Tech_4, currentActiveCategory); });

        btPopulation.onClick.AddListener(delegate { showCategory(Building.eBuildingCategory.Population); });
        btMining.onClick.AddListener(delegate { showCategory(Building.eBuildingCategory.Mining); });
        btProduction.onClick.AddListener(delegate { showCategory(Building.eBuildingCategory.Production); });
        btSpecial.onClick.AddListener(delegate { showCategory(Building.eBuildingCategory.Special); });
        btDefense.onClick.AddListener(delegate { showCategory(Building.eBuildingCategory.Defense); });
        btTerraforming.onClick.AddListener(delegate { showCategory(Building.eBuildingCategory.Terraforming); });
    }

    private void showCategory(Building.eBuildingCategory category)
    {
        currentActiveCategory = category;

        for (int i = 0; i < allBuildingButtons.Count; i++)
        {
            GameObject btBuildingGo = allBuildingButtons[i];
            Building buildingScrp = btBuildingGo.GetComponent<BuildingButton>().buildingPrefab.GetComponent<Building>();

            if(buildingScrp.buildingCategory == category && buildingScrp.unlocked)
            {
                btBuildingGo.gameObject.SetActive(true);
            }
            else
            {
                btBuildingGo.gameObject.SetActive(false);
            }
        }
    }

    private void showHideCategory(Building.eBuildingCategory category, bool show)
    {
        switch (category)
        {
            case Building.eBuildingCategory.None:
                break;
            case Building.eBuildingCategory.Population:
                btPopulation.gameObject.SetActive(show);
                break;
            case Building.eBuildingCategory.Mining:
                btMining.gameObject.SetActive(show);
                break;
            case Building.eBuildingCategory.Production:
                btProduction.gameObject.SetActive(show);
                break;
            case Building.eBuildingCategory.Special:
                btSpecial.gameObject.SetActive(show);
                break;
            case Building.eBuildingCategory.Defense:
                btDefense.gameObject.SetActive(show);
                break;
            case Building.eBuildingCategory.Terraforming:
                btTerraforming.gameObject.SetActive(show);
                break;
            default:
                break;
        }
    }

    private void showHideTech(Building.eTechType tech, bool show)
    {
        switch (tech)
        {
            case Building.eTechType.None:
                break;
            case Building.eTechType.Tech_1:
                buildingsTec1.gameObject.SetActive(show);
                break;
            case Building.eTechType.Tech_2:
                buildingsTec2.gameObject.SetActive(show);
                break;
            case Building.eTechType.Tech_3:
                buildingsTec3.gameObject.SetActive(show);
                break;
            case Building.eTechType.Tech_4:
                buildingsTec4.gameObject.SetActive(show);
                break;
            default:
                break;
        }
    }

    private void checkBuildingButtonInteractible()
    {
        if (BuildingHandler.populationCentreBuilt())
        {
            for (int i = 0; i < allBuildingButtons.Count; i++)
            {
                GameObject btBuildingGo = allBuildingButtons[i];
                Building buildingScrp = btBuildingGo.GetComponent<BuildingButton>().buildingPrefab.GetComponent<Building>();

                btBuildingGo.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            for (int i = 0; i < allBuildingButtons.Count; i++)
            {
                GameObject btBuildingGo = allBuildingButtons[i];
                Building buildingScrp = btBuildingGo.GetComponent<BuildingButton>().buildingPrefab.GetComponent<Building>();

                if (!buildingScrp.isPopulationCentre)
                    btBuildingGo.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void refreshBuildingChooser()
    {
        checkBuildingButtonInteractible();

        showCategory(currentActiveCategory);
    }

    public override void showHideGUI()
    {
        if (GUIHandler.instance.buildingMenuPanel.activeSelf)
        {
            GUIHandler.instance.buildingMenuPanel.SetActive(false);
        }
        else
        {
            GUIHandler.instance.buildingMenuPanel.SetActive(true);
            refreshBuildingChooser();
        }
    }

}
