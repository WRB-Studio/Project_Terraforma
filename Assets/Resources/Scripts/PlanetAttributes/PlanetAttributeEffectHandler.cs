using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetAttributeEffectHandler : MonoBehaviour
{
    private static List<PlanetAttributeEffect> currentEffects = new List<PlanetAttributeEffect>();

    public static PlanetAttributeEffectHandler instance;



    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        showEffectsForAttribute(PlanetAttribute.ePlanetAttributes.None);
    }


    public static PlanetAttributeEffect createEffect(PlanetAttributeEffect.eEffectReason eEffectReasonVal, PlanetAttribute.ePlanetAttributes affectedAttributeVal, long strengthVal, float startDurationVal)
    {
        if (refreshEffect(eEffectReasonVal, affectedAttributeVal, strengthVal, startDurationVal))
            return null;

        GameObject newEffectGO = new GameObject("PlanetAttributeEffect");
        newEffectGO.transform.parent = instance.transform;
        PlanetAttributeEffect newEffect = newEffectGO.AddComponent<PlanetAttributeEffect>();
        newEffect.initEffect(eEffectReasonVal, affectedAttributeVal, strengthVal, startDurationVal,
                             Instantiate(GUIHandler.instance.planetAttributeEffectGUIElementPrefab,
                                         GUIHandler.instance.planetAttributeEffectsParent.transform).GetComponent<PlanetAttributeEffectGUIElement>());

        currentEffects.Add(newEffect);

        return newEffect;
    }

    public static bool refreshEffect(PlanetAttributeEffect.eEffectReason eEffectReasonVal, PlanetAttribute.ePlanetAttributes affectedAttributeVal, long strengthVal, float startDurationVal)
    {
        for (int i = 0; i < currentEffects.Count; i++)
        {
            if (currentEffects[i].effectReason == eEffectReasonVal && currentEffects[i].affectedAttribute == affectedAttributeVal)
            {
                if (currentEffects[i].startDuration == -1) //effect is still valid and counter will be refreshed.
                {
                    currentEffects[i].initEffect(eEffectReasonVal, affectedAttributeVal, strengthVal, startDurationVal, null);
                    return true;
                }
            }
        }
        return false;
    }

    public static void removeEffect(PlanetAttributeEffect removingEffect)
    {
        Destroy(removingEffect.effectGUIElement.gameObject);
        currentEffects.Remove(removingEffect);
        Destroy(removingEffect.gameObject);
    }


    public static void updateCallPerSecond()
    {
        for (int i = 0; i < currentEffects.ToArray().Length; i++)
        {
            PlanetAttributeEffect currentEffect = currentEffects[i];

            currentEffect.updateCall();
        }
    }

    public static void showEffectsForAttribute(PlanetAttribute.ePlanetAttributes planetAttribute)
    {
        if (planetAttribute == PlanetAttribute.ePlanetAttributes.None)
        {
            GUIHandler.instance.planetAttributeEffectsParent.SetActive(false);
            return;
        }

        GUIHandler.instance.planetAttributeEffectsParent.SetActive(true);

        int count = 0;

        for (int i = 0; i < currentEffects.Count; i++)
        {
            currentEffects[i].effectGUIElement.gameObject.SetActive(false);

            if (currentEffects[i].affectedAttribute == planetAttribute)
            {
                currentEffects[i].effectGUIElement.gameObject.SetActive(true);
                count++;
            }
        }

        if (count <= 0)
            GUIHandler.instance.planetAttributeEffectsParent.SetActive(false);

    }


    public static List<PlanetAttributeEffect> getCurrentEffects()
    {
        return currentEffects;
    }

    public static float getTrendAmountOf(PlanetAttribute.ePlanetAttributes effectOn)
    {
        float trend = 0;

        for (int i = 0; i < currentEffects.Count; i++)
        {
            if (currentEffects[i].affectedAttribute == effectOn)
            {
                trend += currentEffects[i].strength;
            }
        }
        return trend;
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
