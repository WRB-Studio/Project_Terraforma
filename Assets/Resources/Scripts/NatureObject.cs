using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatureObject : MonoBehaviour
{   

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Water")
            SpawnVegetation.instance.removeNatureObject(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Water")
            SpawnVegetation.instance.removeNatureObject(gameObject);
    }

}
