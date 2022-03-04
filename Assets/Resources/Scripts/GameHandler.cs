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
        StartCoroutine(initCoroutine());
    }

    private IEnumerator initCoroutine()
    {
        yield return new WaitForSeconds(1);

        if (removeSaveGame)
            SaveAndLoad.removeSaveGame();

        PlanetAttribute.init();
        PlanetAttributeEffectHandler.init();
        CameraController.init();
        SunHandler.init();
        Population.init();
        SpawnVegetation.init();

        GUIHandler.init();
        ResourceHandler.init();
        PauseMenu.init();
        BuildingHandler.init();
        BuildingChooser.init();


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

            Population.updateCall();

            SpawnVegetation.updateCall();

            BuildingHandler.udateCall();
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
