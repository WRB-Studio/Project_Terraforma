using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetAttribute : MonoBehaviour
{
    public enum ePlanetAttributes
    {
        None,
        Gravitation,
        MagneticField,
        Temperature,
        AirPressure,
        Oxygen,
        Water,
        Biomass,
        Population,
        Size
    }

    public static Transform planetModell;
    public static Transform watermodell;

    public static PlanetAttribute[] allPlanetAttributes;

    public ePlanetAttributes planetAttribute = ePlanetAttributes.None;

    public static Vector2 minMaxPlanetHeight;

    public Vector2 currentAndTargetValue;
    public Vector2 minMaxValue;

    public static float terraformingSpeedFactor = 1;
    private static float maxTerraformingFactor = 4;
    private static float maxAttributeAffect = 10;

    public static bool planetIsHabitableForHumanoids = false;

    public static float iceValue; //ice amount value

    private PlanetAttributeGUIElement planetAttributeGUIElement;

    private static bool isInit;

    public static PlanetAttribute instance;




    private void Awake()
    {
        instance = this;
    }

    public static void init()
    {
        if (isInit)
            return;

        isInit = true;

        planetModell = instance.transform.Find("PlanetModell");
        watermodell = planetModell.transform.Find("WaterModell");

        allPlanetAttributes = instance.GetComponents<PlanetAttribute>();

        calculateAndSetTerraformingFactor();

        //createGUIElements
        for (int i = 0; i < allPlanetAttributes.Length; i++)
        {
            PlanetAttribute curPlanetAttribute = allPlanetAttributes[i];

            //set planet size
            if (curPlanetAttribute.planetAttribute == ePlanetAttributes.Size)
            {
                curPlanetAttribute.currentAndTargetValue.x /= 100;
                instance.transform.Find("PlanetModell").transform.localScale *= curPlanetAttribute.currentAndTargetValue.x;
                continue;
            }

            //set symbol in Planet attribute UI
            Sprite[] symbols = Resources.LoadAll<Sprite>("Sprites/Symbols/Terraforming");
            Sprite currentSymbol = null;
            for (int x = 0; x < symbols.Length; x++)
            {
                if (symbols[x].name == curPlanetAttribute.planetAttribute.ToString())
                    currentSymbol = symbols[x];
            }

            //create planet attribute gui element
            PlanetAttributeGUIElement newElement = Instantiate(GUIHandler.instance.planetAttributeGUIElementPrefab, GUIHandler.instance.planetAttributeElementParent.transform).GetComponent<PlanetAttributeGUIElement>();
            newElement.init(curPlanetAttribute, currentSymbol);
            curPlanetAttribute.planetAttributeGUIElement = newElement;
        }

        calculateMinMaxPlanetHeight();
    }

    public static void updateCall()
    {
        for (int i = 0; i < allPlanetAttributes.Length; i++)
        {
            PlanetAttribute planetAttribute = allPlanetAttributes[i];

            if (planetAttribute.planetAttribute == ePlanetAttributes.Size)
                continue;

            //clamp current value between min and max value.
            planetAttribute.currentAndTargetValue.x = Mathf.Clamp(planetAttribute.currentAndTargetValue.x, planetAttribute.minMaxValue.x, planetAttribute.minMaxValue.y);

            planetAttribute.planetAttributeGUIElement.updateCall();

            planetAttribute.handleAttributes();
        }

    }

    public static void calculateMinMaxPlanetHeight()
    {
        Mesh mesh = planetModell.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        float minHeight = Mathf.Infinity;
        float maxHeight = 0;
        float currentDistance;

        for (int i = 0; i < vertices.Length; i++)
        {
            currentDistance = Vector3.Distance(planetModell.position, planetModell.TransformVector(vertices[i]));

            if (currentDistance < minHeight)
                minHeight = currentDistance;

            if (currentDistance > maxHeight)
                maxHeight = currentDistance;
        }
        minMaxPlanetHeight = new Vector2(minHeight, maxHeight);
    }

    public static PlanetAttribute getPlanetAttribute(ePlanetAttributes searchedPlanetAttribute)
    {
        for (int i = 0; i < allPlanetAttributes.Length; i++)
        {
            if (allPlanetAttributes[i].planetAttribute == searchedPlanetAttribute)
                return allPlanetAttributes[i];
        }

        return null;
    }

    private static void calculateAndSetTerraformingFactor()
    {
        //a terraforming speed factor by planet size
        float planetSizeInSquareKm = getPlanetAttribute(ePlanetAttributes.Size).currentAndTargetValue.x * Mathf.PI;
        terraformingSpeedFactor = maxTerraformingFactor - ((getPlanetAttribute(ePlanetAttributes.Size).getCurrentValueInPercent() / 100) * (maxTerraformingFactor - 0.1f));
        GUIHandler.instance.txtTerraformingFactor.text = "Terraformingfactor: " + terraformingSpeedFactor.ToString();
    }

    public float getCurrentValueInPercent()
    {
        return (currentAndTargetValue.x - minMaxValue.x) / ((minMaxValue.y - minMaxValue.x) / 100);
    }

    public float getValueInPercent(float value, Vector2 minMax)
    {
        return (value - minMax.x) / ((minMax.y - minMax.x) / 100);
    }

    public float getDifferenceBetweenAttributesInPercent(ePlanetAttributes attribute1, ePlanetAttributes attribute2, bool unsigned = false)
    {
        float difference = getPlanetAttribute(attribute1).getCurrentValueInPercent() - getPlanetAttribute(attribute2).getCurrentValueInPercent();

        if (unsigned)
            difference = Math.Abs(difference);

        return difference;
    }

    public float getDifferenceBetweenValuesInPercent(float val1, float val2, bool unsigned = false)
    {
        float difference = val1 - val2;

        if (unsigned && difference < 0)
            difference /= -1;

        return difference;
    }

    public float getStrengthByDifferenceWithTolerance(float differenceInPercent, float toleranceInPercent)
    {
        return (float)Math.Round(addAffectClampAndTerraformingFactor(differenceInPercent - toleranceInPercent), 0);
    }

    public float getStrengthByLowArea(float currentValInPercent, float lowPercentArea)
    {
        return (float)Math.Round(addAffectClampAndTerraformingFactor(-(100 - currentValInPercent / (lowPercentArea / 100) / 10)), 0);
    }

    public float getStrengthByHighArea(float currentValInPercent, float lowPercentArea, float highPercentArea)
    {
        return (float)Math.Round(addAffectClampAndTerraformingFactor(currentValInPercent - highPercentArea) / (lowPercentArea / 100), 0);
    }

    public float addAffectClampAndTerraformingFactor(float value)
    {
        value = value / (100 / maxAttributeAffect) * terraformingSpeedFactor;

        if (value > maxAttributeAffect * terraformingSpeedFactor)
            getMaxAffect();

        return value;
    }

    public static float getMaxAffect()
    {
        return (float)Math.Round(maxAttributeAffect * terraformingSpeedFactor, 0);
    }


    public void handleAttributes()
    {
        switch (planetAttribute)
        {
            case ePlanetAttributes.None:
                break;
            case ePlanetAttributes.Gravitation:
                gravityHandling();
                break;
            case ePlanetAttributes.MagneticField:
                magneticFieldHandling();
                break;
            case ePlanetAttributes.Temperature:
                temperatureHandling();
                break;
            case ePlanetAttributes.AirPressure:
                airPressureHandling();
                break;
            case ePlanetAttributes.Oxygen:
                oxygenHandling();
                break;
            case ePlanetAttributes.Water:
                waterHandling();
                break;
            case ePlanetAttributes.Biomass:
                biomassHandling();
                break;
            case ePlanetAttributes.Population:
                populationHandling();
                break;
            case ePlanetAttributes.Size:
                sizeHandling();
                break;
            default:
                break;
        }

        /*else
        {
            if (currentValInPercent <= lowAreaInPercent) //if current value in % is <= 33 %, calculate deduction.
            {
                effectStrength = -(100 - (currentValInPercent / (lowAreaInPercent / 100)) / 10) * terraformingEStrengthFactor * terraformingSpeedFactor; //-(currentValInPercent / (lowAreaInPercent / 100));
                effectStrength = (float)Math.Round(effectStrength, 2);
            }
            else if (currentValInPercent >= highAreaInPercent) //if current value in % is >= 66 %, calculate addition.
            {
                effectStrength = +(((currentValInPercent - highAreaInPercent) / (lowAreaInPercent / 100)) / 10) * terraformingEStrengthFactor * terraformingSpeedFactor;
                effectStrength = (float)Math.Round(effectStrength, 2);

            }
        }*/
    }

    public void gravityHandling()
    {
        //Gravity low => Air Pressure-, Oxygen-, Water-
        //Gravity high => Air Pressure+

        float currentValInPercent = getCurrentValueInPercent();
        float otherAttributeValueInPercent = 0;
        float area = 0;
        float effectStrength = 0;
        float differenceInPercent = 0;
        Vector2 differenceAreas = new Vector2(0, 0); //min difference, max difference; when min difference no affects; when max difference highest affect 
        PlanetAttributeEffect.eEffectReason reason = PlanetAttributeEffect.eEffectReason.None;

        #region Gravity <=> Air pressure
        differenceInPercent = getDifferenceBetweenAttributesInPercent(ePlanetAttributes.Gravitation, ePlanetAttributes.AirPressure, true);
        otherAttributeValueInPercent = getPlanetAttribute(ePlanetAttributes.AirPressure).getCurrentValueInPercent();
        differenceAreas = new Vector2(0.5f, 5);

        //if pressure in high difference to gravity
        if (differenceInPercent >= differenceAreas.x && differenceInPercent <= differenceAreas.y)
        {
            area = differenceAreas.y / 100;
            effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor(differenceInPercent / area), 0);

            //if gravity higher then airpressure 
            if (otherAttributeValueInPercent < currentValInPercent)
            {
                reason = PlanetAttributeEffect.eEffectReason.high_Gravity;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.AirPressure, (long)+effectStrength, -1);
            }//if gravity smaller then airpressure 
            else if (otherAttributeValueInPercent > currentValInPercent)
            {
                reason = PlanetAttributeEffect.eEffectReason.low_Gravity;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.AirPressure, (long)-effectStrength, -1);
            }
        }//if pressure in very high difference to gravity
        else if (differenceInPercent > differenceAreas.y)
        {
            //if gravity higher then airpressure 
            if (otherAttributeValueInPercent < currentValInPercent)
            {
                reason = PlanetAttributeEffect.eEffectReason.high_Gravity;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.AirPressure, (long)getMaxAffect(), -1);
            }//if gravity smaller then airpressure 
            else if (otherAttributeValueInPercent > currentValInPercent)
            {
                reason = PlanetAttributeEffect.eEffectReason.low_Gravity;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.AirPressure, (long)-getMaxAffect(), -1);
            }
        }
        #endregion

        #region Gravity <=> Oxygen
        differenceInPercent = getDifferenceBetweenAttributesInPercent(ePlanetAttributes.Gravitation, ePlanetAttributes.Oxygen, true);
        otherAttributeValueInPercent = getPlanetAttribute(ePlanetAttributes.Oxygen).getCurrentValueInPercent();
        differenceAreas = new Vector2(0.5f, 5);

        //if oxygen in high difference to gravity
        if (differenceInPercent >= differenceAreas.x && differenceInPercent <= differenceAreas.y)
        {
            area = differenceAreas.y / 100;
            effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor(differenceInPercent / area), 0);

            //if gravity higher then oxygen 
            if (otherAttributeValueInPercent < currentValInPercent)
            {
                //reason = PlanetAttributeEffect.eEffectReason.high_Gravity;
                //PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Oxygen, +effectStrength, -1);
            }//if gravity smaller then oxygen 
            else if (otherAttributeValueInPercent > currentValInPercent)
            {
                reason = PlanetAttributeEffect.eEffectReason.low_Gravity;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Oxygen, (long)-effectStrength, -1);
            }
        }//if oxygen in very high difference to gravity
        else if (differenceInPercent > differenceAreas.y)
        {
            //if gravity higher then oxygen 
            if (otherAttributeValueInPercent < currentValInPercent)
            {
                //reason = PlanetAttributeEffect.eEffectReason.high_Gravity;
                //PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Oxygen, getMaxAffect(), -1);*/
            }//if gravity smaller then oxygen 
            else if (otherAttributeValueInPercent > currentValInPercent)
            {
                reason = PlanetAttributeEffect.eEffectReason.low_Gravity;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Oxygen, (long)-getMaxAffect(), -1);
            }
        }
        #endregion

        #region Gravity <=> Water
        differenceInPercent = getDifferenceBetweenAttributesInPercent(ePlanetAttributes.Gravitation, ePlanetAttributes.Water, true);
        otherAttributeValueInPercent = getPlanetAttribute(ePlanetAttributes.Water).getCurrentValueInPercent();
        differenceAreas = new Vector2(0.5f, 5);

        //if oxygen in high difference to gravity
        if (differenceInPercent >= differenceAreas.x && differenceInPercent <= differenceAreas.y)
        {
            area = differenceAreas.y / 100;
            effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor(differenceInPercent / area), 0);

            //if gravity higher then water 
            if (otherAttributeValueInPercent < currentValInPercent)
            {
                //reason = PlanetAttributeEffect.eEffectReason.high_Gravity;
                //PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, +effectStrength, -1);
            }//if gravity smaller then oxygen 
            else if (otherAttributeValueInPercent > currentValInPercent)
            {
                reason = PlanetAttributeEffect.eEffectReason.low_Gravity;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, (long)-effectStrength, -1);
            }
        }//if water in very high difference to gravity
        else if (differenceInPercent > differenceAreas.y)
        {
            //if gravity higher then water 
            if (otherAttributeValueInPercent < currentValInPercent)
            {
                //reason = PlanetAttributeEffect.eEffectReason.high_Gravity;
                //PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, getMaxAffect(), -1);*/
            }//if gravity smaller then water 
            else if (otherAttributeValueInPercent > currentValInPercent)
            {
                reason = PlanetAttributeEffect.eEffectReason.low_Gravity;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, (long)-getMaxAffect(), -1);
            }
        }
        #endregion
    }

    private void magneticFieldHandling()
    {
        //Magnetic field low => Biomass-, Population- (if not in save zones) 
        //Magnetic field high => 
        float currentValInPercent = getCurrentValueInPercent();
        float effectStrength = 0;
        float lowAreaInPercent = 33;
        float highAreaInPercent = 66;
        PlanetAttributeEffect.eEffectReason reason = PlanetAttributeEffect.eEffectReason.None;

        #region Magnetic field <=> Biomass

        //if magnetic field <= 33%
        if (currentValInPercent <= lowAreaInPercent)
        {
            reason = PlanetAttributeEffect.eEffectReason.low_MagneticField;
            effectStrength = getStrengthByLowArea(currentValInPercent, lowAreaInPercent);
            PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Biomass, (long)-effectStrength, -1);
            PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Population, (long)-effectStrength, -1);
        }
        else if (currentValInPercent >= 66)//if magnetic field >= 66%
        {
            //reason = PlanetAttributeEffect.eEffectReason.high_MagneticField;
            //effectStrength = getStrengthByHighArea(currentValInPercent, lowAreaInPercent, highAreaInPercent);
        }
        #endregion
    }

    private void temperatureHandling()
    {
        //Temperature low => Biomass-, ice+(water-)
        //Temperature high => Biomass-, Water-, ice-

        float currentValInPercent = getCurrentValueInPercent();
        float effectStrength = 0;
        float area;

        float currentBiomassInPercent = getPlanetAttribute(ePlanetAttributes.Biomass).getCurrentValueInPercent();
        PlanetAttributeEffect.eEffectReason reason = PlanetAttributeEffect.eEffectReason.None;

        Vector2 optimalArea = new Vector2(285500, 288500);//optimal temperatur area
        Vector2 plantNormalTemperaturArea = new Vector2(283000, 306000);//habitable temperatur area for normal temperatur resistant plants (9.85°C - 32.85°C)
        Vector2 plantlowTemperaturArea = new Vector2(253000, 283000);//habitable temperatur area for low temperatur resistant plants (-20.15°C - 9.85°C)
        Vector2 plantHighTemperaturArea = new Vector2(306000, 333000);//habitable temperatur area for high temperatur resistant plants (32.85°C - 59.85°C)

        #region temperature <=> Biomass
        //habitable temperatur area for normal temperatur resistant plants
        if (currentAndTargetValue.x >= plantNormalTemperaturArea.x && currentAndTargetValue.x <= plantNormalTemperaturArea.y)
        {

        }//habitable temperatur area for low temperatur resistant plants
        else if (currentAndTargetValue.x < plantlowTemperaturArea.y)
        {
            if (currentAndTargetValue.x >= plantlowTemperaturArea.x && currentAndTargetValue.x < plantlowTemperaturArea.y)
            {
                area = (plantlowTemperaturArea.y - plantlowTemperaturArea.x) / 100;
                effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor(100 - (currentAndTargetValue.x - plantlowTemperaturArea.x) / area), 0);

                reason = PlanetAttributeEffect.eEffectReason.low_Temperature;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Biomass, (long)-effectStrength, -1);
            }
            else
            {
                reason = PlanetAttributeEffect.eEffectReason.very_low_Temperature;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Biomass, (long)Math.Round(-maxAttributeAffect * terraformingSpeedFactor * 2, 0), -1);
            }

        }//habitable temperatur area for high temperatur resistant plants
        else if (currentAndTargetValue.x > plantHighTemperaturArea.x)
        {
            if (currentAndTargetValue.x > plantHighTemperaturArea.x && currentAndTargetValue.x <= plantHighTemperaturArea.y)
            {
                area = (plantHighTemperaturArea.x - plantHighTemperaturArea.y) / 100;
                effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor(100 - (currentAndTargetValue.x - plantHighTemperaturArea.y) / area), 0);

                reason = PlanetAttributeEffect.eEffectReason.high_Temperature;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Biomass, (long)-effectStrength, -1);
            }
            else
            {
                reason = PlanetAttributeEffect.eEffectReason.very_high_Temperature;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Biomass, (long)Math.Round(-maxAttributeAffect * terraformingSpeedFactor * 2, 0), -1);
            }
        }
        #endregion

        #region Temperatur <=> Water/Ice
        /*
         When ice exist:
            Temperature between 15,35°C and 59,85°C gletcher is melting. Larger than 59,85°C gletcher melting faster.
         when ice not existing:
            Temperature between 15,35°C and 59,85°C water dries. Larger than 59,85°C water dries faster.

         When water exist:
            Temperature between -20,15°C and 12,35°C water is freezing. Smaller than -20,15°C water freezing faster.
         */

        //temperatur larger 15.35°C
        if (currentAndTargetValue.x > optimalArea.y)
        {
            //temperature between 15.35°C - 59.85°C gletcher is melting to water and water is drying out
            if (currentAndTargetValue.x > optimalArea.y && currentAndTargetValue.x <= plantHighTemperaturArea.y)
            {
                area = plantHighTemperaturArea.y - optimalArea.y;
                effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor((currentAndTargetValue.x - optimalArea.y) / (area / 100)), 0);

                if (iceValue > 0)//gletcher melting
                {
                    reason = PlanetAttributeEffect.eEffectReason.high_Temperature_melting_gletcher;
                    PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, (long)Math.Round(+effectStrength / 2, 0), -1);
                }
                else
                {
                    //water dries
                    reason = PlanetAttributeEffect.eEffectReason.high_Temperature;
                    PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, (long)-effectStrength, -1);
                }
            }//temperatur larger then 59.85°C gletcher melting and water drying faster
            else if (currentAndTargetValue.x > plantHighTemperaturArea.y)
            {
                area = plantHighTemperaturArea.y - optimalArea.y;
                effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor((100 / maxAttributeAffect) * terraformingSpeedFactor), 0);

                Debug.Log(effectStrength);
                if (iceValue > 0) //gletcher melting double
                {
                    reason = PlanetAttributeEffect.eEffectReason.very_high_Temperature_melting_gletcher;
                    PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, (long)+effectStrength, -1);
                }
                else
                {
                    //water dries double
                    reason = PlanetAttributeEffect.eEffectReason.very_high_Temperature;
                    PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, (long)Math.Round(-effectStrength * 2, 0), -1);
                }
            }
        }
        else if (currentAndTargetValue.x < optimalArea.x)//temperatur smaller 12,35°C
        {
            float water = getPlanetAttribute(ePlanetAttributes.Water).currentAndTargetValue.x;

            //temperature between -20,15 - 12,35°C water freezing
            if (currentAndTargetValue.x >= plantlowTemperaturArea.x && currentAndTargetValue.x < optimalArea.x && water > 0)
            {
                area = optimalArea.x - plantlowTemperaturArea.x;
                effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor(100 - (currentAndTargetValue.x - plantlowTemperaturArea.x) / (area / 100)), 0);

                reason = PlanetAttributeEffect.eEffectReason.low_temperature_freezing_water;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, (long)-effectStrength, -1);
            }//temperatur smaller then -20,15°C water is freezing double
            else if (currentAndTargetValue.x < plantlowTemperaturArea.x && water > 0)
            {
                reason = PlanetAttributeEffect.eEffectReason.very_low_temperature_freezing_water;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, -(long)Math.Round((100 / maxAttributeAffect) * terraformingSpeedFactor * 2), -1);
            }
        }
        #endregion
    }

    private void airPressureHandling()
    {
        //Air Pressure low => Temperature-
        //Air Pressure high => Temperature+

        /*
        Affects:
         Pa <   => thiner atmosphere                        => Temperatur-
         Pa >   => thicker atmosphere                       => Temperatur+
         Temperatur / Pa                                    =  0,046 mK/Pa
         Pa < 600                                           => Water to Gas/Ice
         Pa > 600 and Temperatur 200500 mK - 399.500 mK     => Part of Gas/Ice to water.
         10000 Pa - 190000 Pa                               => for Plants and Microbes
         50000 Pa - 150000 Pa                               => for Human and Animals

        Visuals:
         Pa > 0	        => Clouds start forming, but cannot be seen until Pressure is higher.
         Pa > 42,000    => The atmosphere outline can be seen.
         Pa > 110,000   => Clouds start to turn opaque and expand.
         Pa = 150,000   => The atmosphere outline is at it's thickest - Above that point, it fades.
         Pa > 190,000   => The atmosphere outline cannot be seen.
         Pa > 200,000   => The surface is hidden by the clouds.
        */

        float currentValInPercent = getCurrentValueInPercent();
        float otherAttributeValueInPercent = 0;
        float area = 0;
        float effectStrength = 0;
        float differenceInPercent = 0;
        Vector2 differenceAreas = new Vector2(0, 0); //min difference, max difference; when min difference no affects; when max difference highest affect 
        PlanetAttributeEffect.eEffectReason reason = PlanetAttributeEffect.eEffectReason.None;

        #region Air pressure & oxygen <=> Temperature
        /*
            calculate temperature that generated by air pressure and oxygen.
            More air pressure & more oxygen => more Atmosphere => Global warming effect          
            simple description: air pressure + oxygen = temperature
         */

        float oxygenAndPressureFactorForTemperature = 0.9258f;
        float temperaturByOxygen = oxygenAndPressureFactorForTemperature * getPlanetAttribute(ePlanetAttributes.Oxygen).minMaxValue.y / 2;
        float temperaturByAirPressure = oxygenAndPressureFactorForTemperature * minMaxValue.y / 2;
        float expectedTemperature = temperaturByOxygen + temperaturByAirPressure;
        float currentTemperature = getPlanetAttribute(ePlanetAttributes.Temperature).currentAndTargetValue.x;
        Vector2 minMaxTemperature = getPlanetAttribute(ePlanetAttributes.Temperature).minMaxValue;

        differenceInPercent = getDifferenceBetweenValuesInPercent(getValueInPercent(expectedTemperature, minMaxTemperature), getValueInPercent(currentTemperature, minMaxTemperature), true);
        differenceAreas = new Vector2(0.01f, 1f);

        //if expected temperature in high difference to current temperature
        if (differenceInPercent >= differenceAreas.x && differenceInPercent <= differenceAreas.y)
        {
            area = differenceAreas.y / 100;
            effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor(differenceInPercent / area), 0);

            //if expected temperature higher then current temperature 
            if (expectedTemperature > currentTemperature)
            {
                reason = PlanetAttributeEffect.eEffectReason.high_AirPressure;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Temperature, (long)+effectStrength, -1);
            }//if expected temperature smaller then current temperature 
            else if (expectedTemperature < currentTemperature)
            {
                reason = PlanetAttributeEffect.eEffectReason.low_AirPressure;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Temperature, (long)-effectStrength, -1);
            }
        }//if expected temperature in very high difference to current temperature
        else if (differenceInPercent > differenceAreas.y)
        {
            //if expected temperature higher then current temperature 
            if (otherAttributeValueInPercent < currentValInPercent)
            {
                reason = PlanetAttributeEffect.eEffectReason.high_AirPressure;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Temperature, (long)getMaxAffect(), -1);
            }//if expected temperature smaller then current temperature 
            else if (otherAttributeValueInPercent > currentValInPercent)
            {
                reason = PlanetAttributeEffect.eEffectReason.low_AirPressure;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Temperature, (long)-getMaxAffect(), -1);
            }
        }
        #endregion

        #region Air pressure <=> Water / Ice
        /*
         Water and gas to ice when air pressure smaller 60000.
         Ice to Water when air pressure larger 600 and  temperature 
         */
        Vector2 minMaxWaterToIceByPressure = new Vector2(55000, 60000);

        //Water to ice when pressure is smaller 60000
        if (currentAndTargetValue.x >= minMaxWaterToIceByPressure.x && currentAndTargetValue.x <= minMaxWaterToIceByPressure.y)
        {
            area = minMaxWaterToIceByPressure.y - minMaxWaterToIceByPressure.x;
            effectStrength = Math.Abs((float)Math.Round(addAffectClampAndTerraformingFactor((currentAndTargetValue.x - minMaxWaterToIceByPressure.y) / (area / 100)), 0));

            reason = PlanetAttributeEffect.eEffectReason.very_low_AirPressure_freezing_water;
            PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, (long)-effectStrength, -1);
        }
        else if (currentAndTargetValue.x < minMaxWaterToIceByPressure.x)
        {
            reason = PlanetAttributeEffect.eEffectReason.very_low_AirPressure_freezing_water;
            PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Water, (long)-getMaxAffect(), -1);
        }
        #endregion
    }

    private void oxygenHandling()
    {
        /*
         //Oxygen low => Biomass-, (Population- (if not in save zones))
         //Oxygen high => Biomass-, (Population- (if not in save zones))

         low oxygen red atmosphere
         optimal oxygen blue atmosphere
         high oxygen yellow atmosphere
        */

        float currentVal = currentAndTargetValue.x;
        float effectStrength = 0;
        PlanetAttributeEffect.eEffectReason reason = PlanetAttributeEffect.eEffectReason.None;

        Vector2 minMaxOxygenForPlants = new Vector2(100000, 320000); //optimal oxygen area for plants
        float area = 50000;
        float areaLow = minMaxOxygenForPlants.x - area;
        float areaHigh = minMaxOxygenForPlants.y + area;

        //oxygen is to low => biomass is decreasing
        if (currentVal >= areaLow && currentVal <= minMaxOxygenForPlants.x)
        {
            effectStrength = Math.Abs((float)Math.Round(addAffectClampAndTerraformingFactor(100 - (currentVal - area) / (area / 100)), 0));

            reason = PlanetAttributeEffect.eEffectReason.low_Oxygen;
            PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Biomass, (long)-effectStrength, -1);
        }//oxygen is to high => biomass is decreasing
        else if (currentVal >= minMaxOxygenForPlants.y && currentVal <= areaHigh)
        {
            effectStrength = Math.Abs((float)Math.Round(addAffectClampAndTerraformingFactor((currentVal - minMaxOxygenForPlants.y) / (area / 100)), 0));

            reason = PlanetAttributeEffect.eEffectReason.high_Oxygen;
            PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Biomass, (long)-effectStrength, -1);
        }//oygen is to very low or very high => biomass is decreasing
        else if (currentVal < areaLow || currentVal > areaHigh)
        {
            reason = PlanetAttributeEffect.eEffectReason.low_Oxygen;
            PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Biomass, (long)-getMaxAffect(), -1);
        }
    }

    private void waterHandling()
    {
        /*
        Water low => Biomass-, (Temperature+)
        Water high => Biomass-, (Temperature-)      
        */
        float currentVal = currentAndTargetValue.x;
        float currentValInPercent = getCurrentValueInPercent();
        float otherAttributeValue = getPlanetAttribute(ePlanetAttributes.Biomass).currentAndTargetValue.x;
        float otherAttributeValueInPercent = getPlanetAttribute(ePlanetAttributes.Biomass).getCurrentValueInPercent();
        float area = 0;
        float effectStrength = 0;
        float differenceInPercent = 0;
        Vector2 differenceAreas = new Vector2(0, 0); //min difference, max difference; when min difference no affects; when max difference highest affect 
        PlanetAttributeEffect.eEffectReason reason = PlanetAttributeEffect.eEffectReason.None;

        differenceInPercent = otherAttributeValueInPercent - currentValInPercent;
        differenceAreas = new Vector2(4f, 10f);

        #region water <=> biomass
        /*
         When biomasse min 4% larger than water => biomass-. 
         Simple description: not enough water plants die.
         */

        //if biomass is larger than water
        if (otherAttributeValueInPercent > currentValInPercent)
        {
            //if difference is in defined area
            if (differenceInPercent >= differenceAreas.x && differenceInPercent <= differenceAreas.y)
            {
                area = differenceAreas.y / 100;
                effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor(differenceInPercent / area), 0);

                reason = PlanetAttributeEffect.eEffectReason.low_Water;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Biomass, (long)-effectStrength, -1);
            }//if difference is very large
            else if (differenceInPercent > differenceAreas.y)
            {
                reason = PlanetAttributeEffect.eEffectReason.low_Water;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Biomass, (long)-getMaxAffect(), -1);
            }
        }
        #endregion

        #region Water <=> Temperature
        //???
        #endregion

        //water size
        float maxDifference = 0.025f;
        Vector2 minMaxSize = new Vector2(1f - maxDifference, 1f + maxDifference);
        area = minMaxSize.y - minMaxSize.x;
        float newSize = area / 100 * getCurrentValueInPercent() + minMaxSize.x;
        watermodell.transform.localScale = new Vector3(newSize, newSize, newSize);
    }

    private void biomassHandling()
    {
        //Biomass low => Oxygen-
        //Biomass high => Oxygen+
        //Generate Biomass between 33 - 66% of all other attributes

        float currentVal = currentAndTargetValue.x;
        float currentValInPercent = getCurrentValueInPercent();
        float effectStrength = 0;

        float currentBiomassInPercent = getPlanetAttribute(ePlanetAttributes.Biomass).getCurrentValueInPercent();
        PlanetAttributeEffect.eEffectReason reason = PlanetAttributeEffect.eEffectReason.None;
        float area = 50;

        effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor(currentValInPercent / (area / 100)), 0);

        //biomass is generating oxygen
        if (effectStrength > 0)
        {
            reason = PlanetAttributeEffect.eEffectReason.biomass_generating_oxygen;
            PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Oxygen, (long)+effectStrength, -1);
        }


        /*
        float otherValueInPercent = getPlanetAttribute(ePlanetAttributes.Oxygen).getCurrentValueInPercent();
        float differenceInPercent = Math.Abs(otherValueInPercent - currentValInPercent);
        Vector2 toleranceAreaInPercent = new Vector2(10, 20);        

        //if difference is larger the tolerance
        if(differenceInPercent > toleranceAreaInPercent.x)
        {
            //oxygen larger than biomass => decrease oxygen
            if (otherValueInPercent > currentBiomassInPercent)
            {
                area = toleranceAreaInPercent.y / 100;
                effectStrength = (float)Math.Round(addAffectClampAndTerraformingFactor(differenceInPercent / area), 0);

                reason = PlanetAttributeEffect.eEffectReason.low_Biomass;
                PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Oxygen, -effectStrength, -1);
            }
        }
        */

        SpawnVegetation.instance.createTreesByPercentage(getCurrentValueInPercent());
    }

    public void populationHandling()
    {
        //Population low =>
        //Population high => Oxygen-, Air Pressure+, Temperature+, Population+
        long currentVal = Population.instance.totalPopulation;
        float effectStrength = 0;
        PlanetAttributeEffect.eEffectReason reason = PlanetAttributeEffect.eEffectReason.None;

        float planetSize = getPlanetAttribute(ePlanetAttributes.Size).currentAndTargetValue.x;
        //1000000000 (1 Milliarde) humanoids need 1% of 210000 oxygen by an Planet size of 12800.

        // (1 Milliarde / 12800) * (oxygen / 100)
        float planetSizeInPercent = planetSize / (12800 / 100);
        long peopleOxygenRatioPerPlanetSize = (long)(1000000000 / 100 * planetSizeInPercent);

        //decrease oxygen by population amount
        if (currentVal >= peopleOxygenRatioPerPlanetSize)
        {
            effectStrength = currentVal / peopleOxygenRatioPerPlanetSize;

            reason = PlanetAttributeEffect.eEffectReason.high_Population;
            PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Oxygen, (long)-effectStrength, -1);
        }

        //check habitable life
        Vector2 habitableTemperaturAreaForHumanoids = new Vector2(237000, 337000);//habitable temperatur area for humanoids (-36,15°C - 63,85°C)
        float currentTemperature = getPlanetAttribute(ePlanetAttributes.Temperature).currentAndTargetValue.x;

        //habitable temperatur area for humanoids
        if (currentTemperature >= habitableTemperaturAreaForHumanoids.x && currentTemperature <= habitableTemperaturAreaForHumanoids.y)
        {
            planetIsHabitableForHumanoids = true;
        }
        else //non habitable temperatur area for humanoids
        {
            planetIsHabitableForHumanoids = false;

            long peopleToKill = (long)(currentVal / 100 * 0.05f); //kill 0,05% of people (per second)

            reason = PlanetAttributeEffect.eEffectReason.very_low_Temperature;
            PlanetAttributeEffectHandler.createEffect(reason, ePlanetAttributes.Population, -peopleToKill, -1);
        }

    }

    public void sizeHandling()
    {

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
