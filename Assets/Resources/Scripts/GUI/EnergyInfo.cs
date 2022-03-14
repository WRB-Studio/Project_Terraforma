using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyInfo : MonoBehaviour
{   
    public Text txtEnergy;

    public static EnergyInfo instance;



    private void Awake()
    {
        instance = this;
    }


    public static void updateCall()
    {
        instance.txtEnergy.text = EnergyHandler.getEnergyConsume() + " / " + EnergyHandler.getAvailableEnergy();        
    }

}
