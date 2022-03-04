using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIHandler : MonoBehaviour
{
    [Header("GUI main panel")]
    public GameObject pauseMenuPanel;
    public GameObject planetAttributesPanel;
    public GameObject ressourcePanel;
    public GameObject buildingMenuPanel;

    [Header("Planet attribute properties")]
    public GameObject planetAttributeElementParent;
    public GameObject planetAttributeGUIElementPrefab;
    public GameObject planetAttributeEffectsParent;
    public Text txtTerraformingFactor;

    [Header("Ressource GUI properties")]
    public Text txtStorage;
    public GameObject ressourceGUIElementPrefab;

    [Header("Quick menu")]
    public Button btPlaySpeed;
    public Button btPlanetTerraforming;
    public Button btRessources;
    public Button btBuildMenue;

    [Header("Planet attribute effect properties")]
    public GameObject planetAttributeEffectGUIElementPrefab;


    public static GUIHandler instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        instance.initButtons();
    }

    public void initButtons()
    {
        instance.btPlaySpeed.onClick.AddListener(delegate { setNextPlaySpeed(); });

        instance.btPlanetTerraforming.onClick.AddListener(delegate { showHide(planetAttributesPanel); });
        instance.btRessources.onClick.AddListener(delegate { showHide(ressourcePanel); });
        instance.btBuildMenue.onClick.AddListener(delegate { BuildingChooser.instance.showHideGUI(); });

        planetAttributesPanel.SetActive(false);
        ressourcePanel.SetActive(false);
        buildingMenuPanel.SetActive(false);
    }

    public void showHide(GameObject guiObject)
    {
        if (guiObject.activeSelf)
        {
            guiObject.SetActive(false);
        }
        else
        {
            guiObject.SetActive(true);
        }
    }

    public static void setNextPlaySpeed()
    {
        if (instance.btPlaySpeed.transform.GetChild(0).GetComponent<Text>().text == "| |")
        {
            setPlaySpeed(">");
            if (GameHandler.isPause) 
                GameHandler.setIsPause(false);
        }
        else if (instance.btPlaySpeed.transform.GetChild(0).GetComponent<Text>().text == ">")
        {
            setPlaySpeed("| |");
            if (!GameHandler.isPause) 
                GameHandler.setIsPause(true);
        }
    }

    public static void setPlaySpeed(string playSpeedSymbol)
    {
        if(playSpeedSymbol == "| |")
        {
            instance.btPlaySpeed.transform.GetChild(0).GetComponent<Text>().text = playSpeedSymbol;

        }
        else if (playSpeedSymbol == ">")
        {
            instance.btPlaySpeed.transform.GetChild(0).GetComponent<Text>().text = playSpeedSymbol;
        }
    }

    public static void save()
    {
        Debug.Log("Saving ist not implemented!");
    }

    public static void load()
    {
        Debug.Log("Loadeing ist not implemented!");
    }
}
