using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    private static int savegame = 0;

    public enum eSLKeywords
    {
        CamData,
        SunData,

    }


    public static void saveAll()
    {
        PlanetAttribute.save();
        PlanetAttributeEffectHandler.save();
        ResourceHandler.save();
        CameraController.save();
        GameHandler.save();
        GUIHandler.save();
        SpawnVegetation.save();
        SunHandler.save();
    }

    public static void loadAll()
    {
        PlanetAttribute.load();
        PlanetAttributeEffectHandler.load();
        ResourceHandler.load();
        CameraController.load();
        GameHandler.load();
        GUIHandler.load();
        SpawnVegetation.load();
        SunHandler.load();
    }


    public static void removeSaveGame()
    {
        PlayerPrefs.DeleteAll();
    }

    public static bool checkSaveGameExists()
    {
        return PlayerPrefs.HasKey(savegame + "_" + eSLKeywords.CamData.ToString());
    }

    /// <summary>
    /// Save camera pose.
    /// </summary>
    /// <param name="camPosition"></param>
    /// <param name="camRotationEuler"></param>
    public static void saveCamData(float distance, float x, float y)
    {
        string dataString = distance + "_" + x + "_" + y;

        PlayerPrefs.SetString(savegame + "_" + eSLKeywords.CamData.ToString(), dataString);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load camera pose.
    /// </summary>
    /// <returns>float[0] = distance; float[1] = x; float[2] = y</returns>
    public static float[] loadCamData()
    {
        string loadedData = PlayerPrefs.GetString(savegame + "_" + eSLKeywords.CamData.ToString(), null);
        if (loadedData == null)
            return null;

        string[] seperatedString = loadedData.Split('_');

        float[] convertedData = new float[3];
        convertedData[0] = float.Parse(seperatedString[0]);
        convertedData[1] = float.Parse(seperatedString[1]);
        convertedData[2] = float.Parse(seperatedString[2]);

        return convertedData;
    }



    public static void saveSunData(float animatorState)
    {
        PlayerPrefs.SetFloat(savegame + "_" + eSLKeywords.SunData.ToString(), animatorState);
        PlayerPrefs.Save();
    }

    public static float loadSunData()
    {
        float loadedData = PlayerPrefs.GetFloat(savegame + "_" + eSLKeywords.SunData.ToString(), 0);

        return loadedData;
    }



}
