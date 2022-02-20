using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourcePairInfo
{
    public ResourceHandler.eResources resourceType;
    public int amount;

    public ResourcePairInfo(ResourceHandler.eResources resourceTypeVal, int amountVal)
    {
        resourceType = resourceTypeVal;
        amount = amountVal;
    }
}
