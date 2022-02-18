using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIInterface : MonoBehaviour
{
    public GameObject guiPanel;



    public virtual void showHideGUI()
    {
        if (guiPanel.activeSelf)
            guiPanel.SetActive(false);
        else
            guiPanel.SetActive(true);
    }
}
