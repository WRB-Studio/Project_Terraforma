using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceHandler : MonoBehaviour
{
    public int maxStartStorageCapacity;
    public static Vector2 resStorageCapacity; //x = current; y = max

    public enum eRessources
    {
        iron, copper, nickel,
        gold, silicon, cobalt,
        titanium,

        constructionComponents, fabricationComponents,
        chips, crystonics,
        transmissionComponents, energyCells,
        food
    }

    private static List<Tuple<eRessources, int, RessourcesGUIElement>> ressourceType = new List<Tuple<eRessources, int, RessourcesGUIElement>>();

    public static RessourceHandler instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        resStorageCapacity.y = instance.maxStartStorageCapacity;

        //generate ressources
        for (int i = 0; i < Enum.GetNames(typeof(eRessources)).Length; i++)
        {
            RessourcesGUIElement newResGUIElement = Instantiate(GUIHandler.instance.ressourceGUIElementPrefab, GUIHandler.instance.ressourceGUIParent.transform).GetComponent<RessourcesGUIElement>();
            ressourceType.Add(new Tuple<eRessources, int, RessourcesGUIElement>((eRessources)i, 0, newResGUIElement));
            newResGUIElement.itemName.text = ((eRessources)i).ToString();
            newResGUIElement.currentValue.text = "0";
            newResGUIElement.income.text = "0";
            newResGUIElement.expenses.text = "0";
        }

        GUIHandler.instance.txtStorage.text = resStorageCapacity.x + " / " + resStorageCapacity.y;

    }


    public static void modifyResType(eRessources resType, int value)
    {
        if (value > 0 && (resStorageCapacity.x + value) > resStorageCapacity.y)
            value = (int)(resStorageCapacity.x + value - resStorageCapacity.y);

        resStorageCapacity.x += Mathf.Clamp(resStorageCapacity.x + value, 0, resStorageCapacity.y);
        GUIHandler.instance.txtStorage.text = resStorageCapacity.x + " / " + resStorageCapacity.y;

        Tuple<eRessources, int, RessourcesGUIElement> ressource = getRessourceType(resType);
        int newValue = ressource.Item2 + value;
        if (newValue <= 0)
            newValue = 0;
        ressource = new Tuple<eRessources, int, RessourcesGUIElement>(resType, newValue, ressource.Item3);

        ressource.Item3.currentValue.text = ressource.Item2.ToString("f2");

        /*ressource.Item3.income.text = ressource.Item2.ToString("f2");
        ressource.Item3.expenses.text = ressource.Item2.ToString("f2");*/
    }

    public static Tuple<eRessources, int, RessourcesGUIElement> getRessourceType(eRessources resType)
    {
        for (int i = 0; i < ressourceType.Count; i++)
        {
            if (ressourceType[i].Item1 == resType)
                return ressourceType[i];
        }

        return null;
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
