using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetAttributeEffect : MonoBehaviour
{
    public eEffectReason effectReason = eEffectReason.None;
    public PlanetAttribute.ePlanetAttributes affectedAttribute = PlanetAttribute.ePlanetAttributes.None;
    public PlanetAttributeEffectGUIElement effectGUIElement;

    public long strength;
    public float startDuration;
    public float currentDuration;

    public enum eEffectReason
    {
        None,

        building,

        low_Gravity, medium_Gravity, high_Gravity,
        low_MagneticField, medium_MagneticField, high_MagneticField,
        very_low_Temperature, low_Temperature, medium_Temperature, high_Temperature, very_high_Temperature, low_temperature_freezing_water, very_low_temperature_freezing_water, high_Temperature_melting_gletcher, very_high_Temperature_melting_gletcher,
        low_AirPressure, medium_AirPressure, high_AirPressure, very_low_AirPressure_freezing_water,
        low_Oxygen, medium_Oxygen, high_Oxygen,
        low_Water, medium_Water, high_Water, 
        low_Biomass, medium_Biomass, high_Biomass, biomass_generating_oxygen,
        low_Population, medium_Population, high_Population,

        bad_environmental_conditions,

        PopulationCapacity,
        PopulationHigherCapacity,

        Meteor, Solarstorm, Earthquake, Invasion
    }

    public void initEffect(eEffectReason eEffectReasonVal, PlanetAttribute.ePlanetAttributes affectedAttributeVal, long strengthVal, float startDurationVal, PlanetAttributeEffectGUIElement effectGUIElementVal)
    {
        effectReason = eEffectReasonVal;
        affectedAttribute = affectedAttributeVal;
        strength = strengthVal;
        startDuration = startDurationVal;

        if (startDuration < 0)
            currentDuration = 2;
        else
            currentDuration = startDurationVal;

        if (effectGUIElementVal != null)
            effectGUIElement = effectGUIElementVal;

        effectGUIElement.txtEffectInfo.text = strength.ToString() + " " + eEffectReasonVal.ToString();
    }

    public void updateCall()
    {
        //time counting
        currentDuration -= 1;
        if (currentDuration < 0)
        {
            PlanetAttributeEffectHandler.removeEffect(this);
            return;
        }

        //affects-----------------------------
        float tmpEffectStrength;

        //affect by melting gletcher
        if (effectReason == eEffectReason.high_Temperature_melting_gletcher || effectReason == eEffectReason.very_high_Temperature_melting_gletcher)
        {
            if (PlanetAttribute.iceValue <= 0)
            {
                PlanetAttributeEffectHandler.removeEffect(this);
                return;
            }

            tmpEffectStrength = Math.Abs(strength);

            //calculate strength by ice amount
            if (PlanetAttribute.iceValue - tmpEffectStrength < 0)
                tmpEffectStrength = Math.Abs(PlanetAttribute.iceValue - tmpEffectStrength);
            
            //increase water 
            PlanetAttribute attribute = PlanetAttribute.getPlanetAttribute(affectedAttribute);
            attribute.currentAndTargetValue.x += tmpEffectStrength;//increase water

            //decrease ice
            PlanetAttribute.iceValue -= tmpEffectStrength;//decrease ice

        }//affect by freezing water
        else if (effectReason == eEffectReason.low_temperature_freezing_water || effectReason == eEffectReason.very_low_AirPressure_freezing_water)
        {
            float waterValue = PlanetAttribute.getPlanetAttribute(PlanetAttribute.ePlanetAttributes.Water).currentAndTargetValue.x;
            if (waterValue <= 0)
            {
                PlanetAttributeEffectHandler.removeEffect(this);
                return;
            }

            tmpEffectStrength = Math.Abs(strength);

            //calculate strength by water amount
            if (waterValue - tmpEffectStrength < 0)
                tmpEffectStrength = Math.Abs(waterValue - tmpEffectStrength);

            //decrease water 
            PlanetAttribute attribute = PlanetAttribute.getPlanetAttribute(affectedAttribute);
            attribute.currentAndTargetValue.x -= tmpEffectStrength;

            //increase ice
            PlanetAttribute.iceValue += tmpEffectStrength;
        }//very low temperature kills population in non habitable zones
        else if(effectReason == eEffectReason.very_low_Temperature && affectedAttribute == PlanetAttribute.ePlanetAttributes.Population)
        {
            PopulationHandler.instance.killNonHabitablePopulation(strength);
        }
        else//affects by all other reasons
        {
            PlanetAttribute attribute = PlanetAttribute.getPlanetAttribute(affectedAttribute);
            attribute.currentAndTargetValue.x += strength;
        }
    }

}
