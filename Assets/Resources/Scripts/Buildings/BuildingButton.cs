using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public GameObject buildingPrefab;


    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(delegate { ObjectPlacement.instance.instantiateDragObject(buildingPrefab); });
    }


}
