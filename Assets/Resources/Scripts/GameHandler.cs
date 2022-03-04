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

    public void init()
    {
        if (removeSaveGame)
            SaveAndLoad.removeSaveGame();

        PlanetAttribute.init();
        PlanetAttributeEffectHandler.init();
        CameraController.init();
        SunHandler.init();
        Population.init();
        SpawnVegetation.init();

        GUIHandler.init();
        BuildingMenu.init();
        ResourceHandler.init();
        PauseMenu.init();

        isInit = true;
    }



    private void Update()
    {
        if (!isInit || isPause)
            return;

        PlanetAttribute.updateCall();

        if (Input.GetKeyDown(KeyCode.Escape))
            PauseMenu.instance.showHideGUI();

        updatePerSecond();

    }

    private void updatePerSecond()
    {
        updatePerSecondCounter += Time.deltaTime;
        if (updatePerSecondCounter >= 1f)
        {
            updatePerSecondCounter = updatePerSecondCounter % 1f;
            PlanetAttributeEffectHandler.updateCallPerSecond();

            Population.updateCall();

            SpawnVegetation.updateCall();
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
        throw new NotImplementedException();
    }

    public static void load()
    {
        throw new NotImplementedException();
    }
}
