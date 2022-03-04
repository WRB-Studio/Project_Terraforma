using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public GameObject buildingPrefab;


    private void Awake()
    {
        transform.GetChild(0).GetComponent<Text>().text = buildingPrefab.name.Split('_')[1];

        GetComponent<Button>().onClick.AddListener(delegate { ObjectPlacement.instance.startObjectPlacement(buildingPrefab); });
    }


}
