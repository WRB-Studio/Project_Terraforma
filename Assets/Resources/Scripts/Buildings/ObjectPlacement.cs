using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    private GameObject currentDragObject;
    
    public static ObjectPlacement instance;



    private void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        //raycast with mouse to planet surface. Check water, other building and other objects
        string debug = "";
        if(currentDragObject != null)
        {
            //ray to planet surface to placing building
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hitCol in hits)
            {
                //when pointer on planet surface set building position to pointer
                if (hitCol.transform.tag == "PlanetSurface")
                {

                    currentDragObject.transform.position = hitCol.point;//building position to pointer position

                    //rotation to planet gravity
                    Vector3 gravityUp = (currentDragObject.transform.position - hitCol.transform.position).normalized;
                    currentDragObject.transform.rotation = Quaternion.FromToRotation(currentDragObject.transform.up, gravityUp) * currentDragObject.transform.rotation;

                    //build building on current position when no collisions
                    if (Input.GetMouseButtonUp(0))
                    {
                        currentDragObject = null;
                        break;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(currentDragObject);
                currentDragObject = null;
            }
        }
    }


    public void instantiateDragObject(GameObject dragObject)
    {
        if(currentDragObject != null)
        {
            Destroy(currentDragObject);
            currentDragObject = null;
        }

        GameObject newDragObject = Instantiate(dragObject);
        currentDragObject = newDragObject;
    }
}
