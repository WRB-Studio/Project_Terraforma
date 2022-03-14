using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductionItem
{
    public Building building { get; set; }

    public ResourcePairInfo[] input;
    public ResourcePairInfo output;

    public float productionTime;
    private float countdown;

    private bool isProducing;
    private bool isHolding;



    public ProductionItem(Building buildingVal, ResourcePairInfo[] inputVal, ResourcePairInfo outputVal, float productionTimeVal)
    {
        building = buildingVal;
        input = inputVal;
        output = outputVal;
        productionTime = productionTimeVal;
    }


    public void updateCall()
    {
        if (!building.IsActivated && !building.HasEnergy)
            return;

        if (isProducing)//production in progress
        {
            if (countdown > 0)//handle producing time
            {
                countdown -= Time.deltaTime;
            }
            else//when production is completed
            {
                //store new product when enough store capacity
                if(ResourceHandler.canStore(output.amount))
                {
                    ResourceHandler.addResource(output);
                    isHolding = false;
                    isProducing = false;
                }
                else
                {
                    isHolding = true;
                }
            }
        }
        else//when no production in progress
        {
            if (canProduce())//check product can produce
            {
                countdown = productionTime;
                isProducing = true;

                //subtract needed resources 
                for (int inputIndex = 0; inputIndex < input.Length; inputIndex++)
                    ResourceHandler.removeRessource(input[inputIndex]);
            }
        }
    }

    public bool canProduce()
    {
        if (!building.IsActivated && !building.HasEnergy)
            return false;

        for (int inputIndex = 0; inputIndex < input.Length; inputIndex++)
        {
            if (!ResourceHandler.resourceAvailable(input[inputIndex].resourceType, input[inputIndex].amount))
            {
                return false;
            }
        }

        return true;
    }

}
