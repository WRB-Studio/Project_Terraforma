using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public bool removeSaveGame;

    public static bool isPause;

    float updatePerSecondCounter = 0f;



    private bool isInit = false;
    public static GameHandler instance;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        init();
    }

    private void init()
    {
        if (removeSaveGame)
            SaveAndLoad.removeSaveGame();

        PlanetAttribute.init();
        PlanetAttributeEffectHandler.init();
        CameraController.init();
        SunHandler.init();
        SpawnVegetation.init();

        PopulationHandler.init();
        ResourceHandler.init();
        BuildingHandler.init();
        EnergyHandler.init();

        GUIHandler.init();
        PauseMenu.init();
        BuildingChooser.init();
        BuildingInfoGUI.init();

        isInit = true;
    }


    private void Update()
    {
        if (!isInit || isPause)
            return;

        PlanetAttribute.updateCall();

        if (Input.GetKeyDown(KeyCode.Escape))
            GUIHandler.instance.showHide(GUIHandler.instance.pauseMenuPanel);

        updatePerSecond();

    }

    private void updatePerSecond()
    {
        updatePerSecondCounter += Time.deltaTime;
        if (updatePerSecondCounter >= 1f)
        {
            updatePerSecondCounter = updatePerSecondCounter % 1f;
            PlanetAttributeEffectHandler.updateCallPerSecond();

            SpawnVegetation.updateCall();


            PopulationHandler.updateCall();
            ResourceHandler.updateCall();
            BuildingHandler.updateCall();
            EnergyHandler.updateCall();

            PopulationInfo.updateCall();
            EnergyInfo.updateCall();

            BuildingInfoGUI.updateCall();
        }
    }


    public static void setIsPause(bool pause)
    {
        isPause = pause;

        if (pause)
        {
            GUIHandler.setPlaySpeed("||");
            SunHandler.startStopCirculation(false);
        }
        else
        {
            GUIHandler.setPlaySpeed(">");
            SunHandler.startStopCirculation(true);
        }
    }


    private void OnApplicationQuit()
    {
        SaveAndLoad.saveAll();
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
