using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : GUIInterface
{
    public Button btContinue;
    public Button btSave;
    public Button btLoad;
    public Button btExit;

    public static PauseMenu instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        if (instance.guiPanel.activeSelf)
            instance.showHideGUI();

        instance.btContinue.onClick.AddListener(delegate { onBtContinue(); });
        instance.btSave.onClick.AddListener(delegate { onbtSave(); });
        instance.btLoad.onClick.AddListener(delegate { onbtLoad(); });
        instance.btExit.onClick.AddListener(delegate { onbtExit(); });
    }



    public static void onBtContinue()
    {
        instance.showHideGUI();
    }

    public static void onbtSave()
    {
        SaveAndLoad.loadAll();
    }

    public static void onbtLoad()
    {
        SaveAndLoad.loadAll();
    }

    public static void onbtExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }


    public override void showHideGUI()
    {
        if (guiPanel.activeSelf)
        {
            guiPanel.SetActive(false);
            GameHandler.setIsPause(false);
        }
        else
        {
            guiPanel.SetActive(true);
            GameHandler.setIsPause(true);
        }
    }
}
