using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductionItem
{
    public Building building { get; set; }

    public float productionTime;

    private float countdown;
    public float Countdown { get => countdown; }

    private bool isProducing;
    public bool IsProducing { get => isProducing; }

    private bool isHolding;
    public bool IsHolding { get => isHolding; }


    public ResourcePairInfo[] input = null;
    public ResourcePairInfo output = null;




    public ProductionItem(Building buildingVal, ResourcePairInfo[] inputVal, ResourcePairInfo outputVal, float productionTimeVal)
    {
        output = null;

        building = buildingVal;
        input = inputVal;
        output = outputVal;
        productionTime = productionTimeVal;
    }


    public void updateCall()
    {
        if (!building.IsActivated && !building.HasEnergy)
            return;

        if (IsProducing)//production in progress
        {
            if (countdown > 0)//handle producing time
            {
                countdown -= Time.deltaTime;
            }
            else//when production is completed
            {
                //store new product when enough store capacity
                if (ResourceHandler.canStore(output.amount))
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
