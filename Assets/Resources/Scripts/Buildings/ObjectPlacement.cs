using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPlacement : MonoBehaviour
{
    private GameObject currentDragObject;
    public float objectRotateFactor = 2;

    public Color cantBuildColor;
    public Color canBuildColor;

    public RectTransform heightPanel;

    public static ObjectPlacement instance;



    private void Awake()
    {
        instance = this;
        showHideHeightPanel(false);
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

                        showHideHeightPanel(true);
                        heightPanelUpdate(hitCol.point);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Destroy(currentDragObject);
                    currentDragObject = null;
                }
            }                        
        }

        showHideHeightPanel(false);
    }

    public void heightPanelUpdate(Vector3 worldPosition)
    {
        heightPanel.position = Camera.main.WorldToScreenPoint(worldPosition);
        heightPanel.transform.GetChild(0).GetComponent<Text>().text = System.Math.Round((Vector3.Distance(PlanetAttribute.planetModell.position, worldPosition) - PlanetAttribute.minMaxPlanetHeight.x) * 100, 2).ToString();
    }

    private void showHideHeightPanel(bool show)
    {
        if (show)
            heightPanel.gameObject.SetActive(true);
        else
            heightPanel.gameObject.SetActive(false);
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
