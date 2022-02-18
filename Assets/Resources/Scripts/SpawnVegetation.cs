using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVegetation : MonoBehaviour
{
    public List<GameObject> treePrefabs;
    public List<GameObject> bushesPrefabs;
    public List<GameObject> grassPrefabs;

    public Transform natureParent;



    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            instantiateTree();
        }
    }

    private void instantiateTree()
    {
        //get random point around the Planet
        Vector3 randPositionAroundPlanet = UnityEngine.Random.onUnitSphere * (PlanetAttribute.getPlanetAttribute(PlanetAttribute.ePlanetAttributes.Size).currentAndTargetValue.x * 4);

        RaycastHit hit;
        if (Physics.Raycast(randPositionAroundPlanet, Vector3.zero - randPositionAroundPlanet, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(randPositionAroundPlanet, Vector3.zero - randPositionAroundPlanet * hit.distance, Color.green, 4);

            GameObject newObject = Instantiate(treePrefabs[0], natureParent);
            newObject.transform.position = hit.point;
            newObject.transform.rotation = Quaternion.LookRotation(Vector3.zero - newObject.transform.position);
        }
        else
        {
            Debug.DrawRay(randPositionAroundPlanet, randPositionAroundPlanet - Vector3.zero * 5000, Color.red, 4);
        }
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
