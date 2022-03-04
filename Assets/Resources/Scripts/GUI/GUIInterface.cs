using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIInterface : MonoBehaviour
{
    public virtual void showHideGUI()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
