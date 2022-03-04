using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    public static Vector2 resStorageCapacity; //x = current; y = max

    public enum eResources
    {
        iron, copper, nickel,
        gold, silicon, cobalt,
        titanium,

        constructionComponents, fabricationComponents,
        chips, crystonics,
        transmissionComponents, energyCells,
        food,

        totalEnergy, energyConsume,
    }

    private static List<Tuple<eResources, int, RessourcesGUIElement>> ressourceType = new List<Tuple<eResources, int, RessourcesGUIElement>>();

    public static ResourceHandler instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        //generate ressources
        for (int i = 0; i < Enum.GetNames(typeof(eResources)).Length; i++)
        {
            RessourcesGUIElement newResGUIElement = Instantiate(GUIHandler.instance.ressourceGUIElementPrefab, GUIHandler.instance.ressourcePanel.transform).GetComponent<RessourcesGUIElement>();
            ressourceType.Add(new Tuple<eResources, int, RessourcesGUIElement>((eResources)i, 0, newResGUIElement));
            newResGUIElement.itemName.text = ((eResources)i).ToString();
            newResGUIElement.currentValue.text = "0";
            newResGUIElement.income.text = "0";
            newResGUIElement.expenses.text = "0";
        }

        GUIHandler.instance.txtStorage.text = resStorageCapacity.x + " / " + resStorageCapacity.y;

    }

    public static void addResource(ResourcePairInfo resPair)
    {
        eResources resType = resPair.resourceType;
        int value = resPair.amount;

        if (resType != eResources.totalEnergy && resType != eResources.energyConsume)//!energy need no storage!
        {
            if (value > 0 && (resStorageCapacity.x + value) > resStorageCapacity.y)
                value = (int)(resStorageCapacity.x + value - resStorageCapacity.y);

            resStorageCapacity.x += Mathf.Clamp(resStorageCapacity.x + value, 0, resStorageCapacity.y);
            GUIHandler.instance.txtStorage.text = resStorageCapacity.x + " / " + resStorageCapacity.y;
        }

        int resTypeIndex = getRessourceTypeIndex(resType);
        Tuple<eResources, int, RessourcesGUIElement> ressource = ressourceType[resTypeIndex];
        int newValue = ressource.Item2 + value;
        if (newValue <= 0)
            newValue = 0;
        ressource.Item3.currentValue.text = Mathf.RoundToInt(newValue).ToString();

        ressourceType[resTypeIndex] = new Tuple<eResources, int, RessourcesGUIElement>(resType, newValue, ressource.Item3);


        /*ressource.Item3.income.text = ressource.Item2.ToString("f2");
        ressource.Item3.expenses.text = ressource.Item2.ToString("f2");*/
    }

    public static Tuple<eResources, int, RessourcesGUIElement> getRessourceType(eResources resType)
    {
        for (int i = 0; i < ressourceType.Count; i++)
        {
            if (ressourceType[i].Item1 == resType)
                return ressourceType[i];
        }

        return null;
    }

    public static int getRessourceTypeIndex(eResources resType)
    {
        for (int i = 0; i < ressourceType.Count; i++)
        {
            if (ressourceType[i].Item1 == resType)
                return i;
        }

        return -1;
    }


    public static void addStorage(int storageExtend)
    {
        resStorageCapacity.y += storageExtend;
        GUIHandler.instance.txtStorage.text = resStorageCapacity.x + " / " + resStorageCapacity.y;
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
