using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    private GameObject currentDragObject;
    public float objectRotateFactor = 2;

    public Color cantBuildColor;
    public Color canBuildColor;

    public static ObjectPlacement instance;



    private void Awake()
    {
        instance = this;
    }


    private IEnumerator dragAndPlacementCoroutine()
    {
        float placeableCountdown = 0.25f;

        while (currentDragObject != null)
        {
            if (placeableCountdown > 0)
                placeableCountdown -= Time.deltaTime;

            yield return null;

            //object dragging
            if (currentDragObject != null)
            {
                //ray to planet surface to placing building
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray);

                foreach (RaycastHit hitCol in hits)
                {
                    if (hitCol.transform.tag == "PlanetSurface")//when pointer on planet surface
                    {
                        currentDragObject.transform.position = hitCol.point;//building position to ray pointer

                        //rotation to planet gravity
                        Vector3 gravityUp = (currentDragObject.transform.position - hitCol.transform.position).normalized;
                        currentDragObject.transform.rotation = Quaternion.FromToRotation(currentDragObject.transform.up, gravityUp) * currentDragObject.transform.rotation;

                        //object rotation by mouse scroll wheel
                        if (Input.GetKey(KeyCode.LeftShift))
                            currentDragObject.transform.Rotate(0, Input.mouseScrollDelta.y * objectRotateFactor, 0);

                        //place building on current ray point when no collisions
                        if (placeableCountdown <= 0 && currentDragObject.GetComponent<Building>() != null && currentDragObject.GetComponent<Building>().isBuildable() && Input.GetMouseButtonUp(0))
                        {
                            if (currentDragObject.GetComponent<Building>() != null)//place building
                                currentDragObject.GetComponent<Building>().onBuild(false);

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
    }

    public void startObjectPlacement(GameObject dragObject)
    {
        if (currentDragObject != null)
        {
            Destroy(currentDragObject);
            currentDragObject = null;
        }

        GameObject newDragObject = Instantiate(dragObject);
        currentDragObject = newDragObject;

        if (currentDragObject.GetComponent<Building>() != null)
        {
            currentDragObject.GetComponent<Building>().onBuild(true);
        }

        StartCoroutine(dragAndPlacementCoroutine());
    }
}
