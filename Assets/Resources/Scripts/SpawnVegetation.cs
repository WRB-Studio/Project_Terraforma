using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVegetation : MonoBehaviour
{
    public int maxNatureObjects;

    public List<GameObject> treePrefabs;
    public List<GameObject> bushesPrefabs;
    public List<GameObject> grassPrefabs;

    public Transform natureParent;
    public static List<GameObject> instantiatedNatureObjects = new List<GameObject>();

    public static SpawnVegetation instance;




    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    public static void init()
    {

    }

    public static void updateCall()
    {

    }

    /// <summary>
    /// Instantiate nature objects only on land surfaces.
    /// </summary>
    private void instantiateTree()
    {
        //get random point around the Planet
        Vector3 randPositionAroundPlanet = UnityEngine.Random.onUnitSphere * (PlanetAttribute.getPlanetAttribute(PlanetAttribute.ePlanetAttributes.Size).currentAndTargetValue.x * 4);
        RaycastHit[] hits = Physics.RaycastAll(randPositionAroundPlanet, Vector3.zero - randPositionAroundPlanet);

        foreach (RaycastHit hitCol in hits)
        {
            if (hitCol.transform.tag == "PlanetSurface")//when pointer on planet surface
            {
                Debug.DrawRay(randPositionAroundPlanet, Vector3.zero - randPositionAroundPlanet * hitCol.distance, Color.green, 4);

                GameObject newObject = Instantiate(treePrefabs[0], natureParent);
                newObject.transform.position = hitCol.point;
                newObject.transform.rotation = Quaternion.LookRotation(Vector3.zero - newObject.transform.position);
                instantiatedNatureObjects.Add(newObject);

                return;
            }
            else
            {
                Debug.DrawRay(randPositionAroundPlanet, randPositionAroundPlanet - Vector3.zero * 5000, Color.red, 4);
            }
        }


        /*RaycastHit hit;
        if (Physics.Raycast(randPositionAroundPlanet, Vector3.zero - randPositionAroundPlanet, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(randPositionAroundPlanet, Vector3.zero - randPositionAroundPlanet * hit.distance, Color.green, 4);

            GameObject newObject = Instantiate(treePrefabs[0], natureParent);
            newObject.transform.position = hit.point;
            newObject.transform.rotation = Quaternion.LookRotation(Vector3.zero - newObject.transform.position);
            instantiatedNatureObjects.Add(newObject);
        }
        else
        {
            Debug.DrawRay(randPositionAroundPlanet, randPositionAroundPlanet - Vector3.zero * 5000, Color.red, 4);
        }*/
    }

    public void createTreesByPercentage(float percentage)
    {
        int amount = Mathf.RoundToInt(maxNatureObjects / 100 * percentage);
        Debug.Log(amount);
        while (instantiatedNatureObjects.Count < amount)
        {
            instantiateTree();
        }

        while (instantiatedNatureObjects.Count > amount)
        {
            removeRandomNatureObject();
        }
    }

    public void removeRandomNatureObject()
    {
        GameObject randomNatureObject = instantiatedNatureObjects[UnityEngine.Random.Range(0, instantiatedNatureObjects.Count - 1)];
        removeNatureObject(randomNatureObject);
    }

    public void removeNatureObject(GameObject destroyingObject)
    {
        instantiatedNatureObjects.Remove(destroyingObject);
        Destroy(destroyingObject);
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
