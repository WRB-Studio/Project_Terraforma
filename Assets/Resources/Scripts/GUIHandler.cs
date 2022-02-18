using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIHandler : MonoBehaviour
{
    [Header("Planet attribute properties")]
    public GameObject planetAttributeGUIParent;
    public GameObject planetAttributeGUIElementPrefab;
    public Text txtTerraformingFactor;

    [Header("Ressource GUI properties")]
    public GameObject ressourceGUIParent;
    public Text txtStorage;
    public GameObject ressourceGUIElementPrefab;

    [Header("PlaySpeed properties")]
    public Button btPlaySpeed;

    [Header("Planet attribute effect properties")]
    public GameObject planetAttributeEffectGUIParent;
    public GameObject planetAttributeEffectGUIElementPrefab;


    public static GUIHandler instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        instance.btPlaySpeed.onClick.AddListener(delegate { setNextPlaySpeed(); });
    }

    public static void setNextPlaySpeed()
    {
        if (instance.btPlaySpeed.transform.GetChild(0).GetComponent<Text>().text == "||")
        {
            setPlaySpeed(">");
            if(GameHandler.isPause)
                GameHandler.setIsPause(false);
        }
        else if (instance.btPlaySpeed.transform.GetChild(0).GetComponent<Text>().text == ">")
        {
            setPlaySpeed(">>");
            if (GameHandler.isPause) 
                GameHandler.setIsPause(false);
        }
        else if (instance.btPlaySpeed.transform.GetChild(0).GetComponent<Text>().text == ">>")
        {
            setPlaySpeed("||");
            if (!GameHandler.isPause) 
                GameHandler.setIsPause(true);
        }
    }

    public static void setPlaySpeed(string playSpeedSymbol)
    {
        if(playSpeedSymbol == "||")
        {
            instance.btPlaySpeed.transform.GetChild(0).GetComponent<Text>().text = playSpeedSymbol;

        }
        else if (playSpeedSymbol == ">")
        {
            instance.btPlaySpeed.transform.GetChild(0).GetComponent<Text>().text = playSpeedSymbol;

        }
        else if (playSpeedSymbol == ">>")
        {
            instance.btPlaySpeed.transform.GetChild(0).GetComponent<Text>().text = playSpeedSymbol;

        }
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
