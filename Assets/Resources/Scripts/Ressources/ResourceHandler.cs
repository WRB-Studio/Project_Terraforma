using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    public enum eResources
    {
        None,

        iron, copper, nickel,
        gold, silicon, cobalt,
        titanium,

        constructionComponents, fabricationComponents,
        chips, crystonics,
        transmissionComponents, energyCells,
        food,
    }


    private static List<Tuple<eResources, int, RessourcesGUIElement>> ressourceType = new List<Tuple<eResources, int, RessourcesGUIElement>>();

    public static Vector2 resStorageCapacity; //x = current; y = max

    public static List<Building> productionBuildings = new List<Building>();
    public static List<Building> storageBuildings = new List<Building>();

    public static List<ProductionItem> productionList = new List<ProductionItem>();

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
            if (((eResources)i) == eResources.None)
                continue;

            RessourcesGUIElement newResGUIElement = Instantiate(GUIHandler.instance.ressourceGUIElementPrefab, GUIHandler.instance.ressourcePanel.transform).GetComponent<RessourcesGUIElement>();
            ressourceType.Add(new Tuple<eResources, int, RessourcesGUIElement>((eResources)i, 0, newResGUIElement));
            newResGUIElement.itemName.text = ((eResources)i).ToString();
            newResGUIElement.currentValue.text = "0";
            newResGUIElement.income.text = "0";
            newResGUIElement.expenses.text = "0";
        }

        GUIHandler.instance.txtStorage.text = resStorageCapacity.x + " / " + resStorageCapacity.y;

    }


    public static void updateCall()
    {
        productionUpdate();
    }

    public static void productionUpdate()
    {
        for (int productionItemIndex = 0; productionItemIndex < productionList.Count; productionItemIndex++)
        {
            productionList[productionItemIndex].updateCall();
        }
    }


    public static bool canStore(int amount)
    {
        if (amount + resStorageCapacity.x > resStorageCapacity.y)
            return false;

        return true;
    }

    public static void addStorage(int storageExtend)
    {
        resStorageCapacity.y += storageExtend;
        GUIHandler.instance.txtStorage.text = resStorageCapacity.x + " / " + resStorageCapacity.y;
    }

    public static void addResource(ResourcePairInfo resPair)
    {
        if (resPair.resourceType == eResources.None)
            return;

        resStorageCapacity.x += resPair.amount;
        GUIHandler.instance.txtStorage.text = resStorageCapacity.x + " / " + resStorageCapacity.y;

        int resType = getRessourceTypeIndex(resPair.resourceType);
        Tuple<eResources, int, RessourcesGUIElement> ressource = ressourceType[resType];
        int newValue = ressource.Item2 + resPair.amount;

        ressource.Item3.currentValue.text = Mathf.RoundToInt(newValue).ToString();

        ressourceType[resType] = new Tuple<eResources, int, RessourcesGUIElement>(resPair.resourceType, newValue, ressource.Item3);
    }

    public static void removeRessource(ResourcePairInfo resPair)
    {
        resStorageCapacity.x -= resPair.amount;
        GUIHandler.instance.txtStorage.text = resStorageCapacity.x + " / " + resStorageCapacity.y;

        int resType = getRessourceTypeIndex(resPair.resourceType);
        Tuple<eResources, int, RessourcesGUIElement> ressource = ressourceType[resType];
        int newValue = ressource.Item2 - resPair.amount;
        if (newValue < 0)
            newValue = 0;
        ressource.Item3.currentValue.text = Mathf.RoundToInt(newValue).ToString();

        ressourceType[resType] = new Tuple<eResources, int, RessourcesGUIElement>(resPair.resourceType, newValue, ressource.Item3);
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

    public static bool resourceAvailable(eResources resType, int amount)
    {
        Tuple<eResources, int, RessourcesGUIElement> resPair = getRessourceType(resType);

        if (resPair.Item2 >= amount)
            return true;

        return false;
    }



    public static void registerBuilding(Building newBuilding)
    {
        if (newBuilding.productionItem != null)
        {
            productionBuildings.Add(newBuilding);
            productionList.Add(newBuilding.productionItem);
        }

        if (newBuilding.storage > 0)
        {
            storageBuildings.Add(newBuilding);
            addStorage(newBuilding.storage);
        }
    }

    public static void deregisterBuilding(Building removeBuilding)
    {
        if (removeBuilding.productionItem != null)
        {
            productionBuildings.Remove(removeBuilding);
        }

        if (removeBuilding.storage > 0)
        {
            storageBuildings.Remove(removeBuilding);
            addStorage(-removeBuilding.storage);
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
