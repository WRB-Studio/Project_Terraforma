using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanetAttributeGUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image imgSymbol;
    public Image imgTrend;
    public Slider sliderValue;
    public Text txtTitle;
    public Text txtCurrentValue;
    public Text txtCurrentTrend;
    public Text txtMinValue;
    public Text txtMaxValue;
    public PlanetAttribute planetAttributeScrp;

    public void init(PlanetAttribute planetAttributeScrpVal)
    {
        planetAttributeScrp = planetAttributeScrpVal;

        txtTitle.text = planetAttributeScrpVal.planetAttribute.ToString();

        if (planetAttributeScrp.planetAttribute == PlanetAttribute.ePlanetAttributes.Population)
        {
            sliderValue.minValue = 0;
            sliderValue.maxValue = Population.instance.totalHousingUnits;
            sliderValue.value = Population.instance.totalPopulation;
            txtMinValue.text = sliderValue.minValue.ToString();
            txtMaxValue.text = sliderValue.maxValue.ToString();
            txtCurrentValue.text = sliderValue.value.ToString("f2");
        }
        else
        {
            sliderValue.minValue = planetAttributeScrpVal.minMaxValue.x;
            sliderValue.maxValue = planetAttributeScrpVal.minMaxValue.y;
            sliderValue.value = planetAttributeScrpVal.currentAndTargetValue.x;
            txtMinValue.text = sliderValue.minValue.ToString();
            txtMaxValue.text = sliderValue.maxValue.ToString();
            txtCurrentValue.text = sliderValue.value.ToString("f2");
            setTrend(0);
        }

        setTrend(0);
    }

    public void updateCall()
    {
        if (planetAttributeScrp.planetAttribute == PlanetAttribute.ePlanetAttributes.Population)
        {
            sliderValue.value = Population.instance.totalPopulation;

            txtCurrentValue.text = Population.instance.totalPopulation.ToString();
            setTrendForPopulation(Population.instance.totalBirthsPerSecond);

            sliderValue.maxValue = Population.instance.totalHousingUnits;
            txtMaxValue.text = sliderValue.maxValue.ToString();

            //celsius
            txtCurrentValue.text = "Total: " + Population.instance.totalPopulation + "\t\t In habitat: " + Population.instance.totalPopulationInHabitat + "\t\t Not in Habitat: " + Population.instance.getNotInHabitatPopulation();
            setTrend(PlanetAttributeEffectHandler.getTrendAmountOf(planetAttributeScrp.planetAttribute));
        }
        else
        {
            sliderValue.value = planetAttributeScrp.currentAndTargetValue.x;

            txtCurrentValue.text = sliderValue.value.ToString("f2");
            setTrend(PlanetAttributeEffectHandler.getTrendAmountOf(planetAttributeScrp.planetAttribute));

            if (planetAttributeScrp.planetAttribute == PlanetAttribute.ePlanetAttributes.Water)
            {
                txtCurrentValue.text = "Water: " + sliderValue.value.ToString() + "\t\tIce:" + PlanetAttribute.iceValue;
            }

            if (planetAttributeScrp.planetAttribute == PlanetAttribute.ePlanetAttributes.Temperature)
            {
                //celsius
                txtCurrentValue.text = sliderValue.value + "mK\t\t" + (sliderValue.value / 1000 - 273.15f).ToString("f2") + "°C";
                setTrend(PlanetAttributeEffectHandler.getTrendAmountOf(planetAttributeScrp.planetAttribute));
            }
        }
    }

    public void setTrend(float trendValue)
    {
        txtCurrentTrend.text = trendValue.ToString();

        if (trendValue > 0)
        {
            imgTrend.color = Color.green;
            imgTrend.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (trendValue < 0)
        {
            imgTrend.color = Color.red;
            imgTrend.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            imgTrend.color = new Color(0, 0, 0, 0);
            imgTrend.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void setTrendForPopulation(double trendValue)
    {
        txtCurrentTrend.text = trendValue.ToString();

        if (trendValue > 0)
        {
            imgTrend.color = Color.green;
            imgTrend.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (trendValue < 0)
        {
            imgTrend.color = Color.red;
            imgTrend.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            imgTrend.color = new Color(0, 0, 0, 0);
            imgTrend.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        PlanetAttributeEffectHandler.showEffectsForAttribute(planetAttributeScrp.planetAttribute);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlanetAttributeEffectHandler.showEffectsForAttribute(PlanetAttribute.ePlanetAttributes.None);
    }
}
