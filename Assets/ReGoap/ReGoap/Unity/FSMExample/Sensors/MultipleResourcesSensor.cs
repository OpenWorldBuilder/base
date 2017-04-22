﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// the agent in this example is a villager which knows the location of trees, so seeTree is always true if there is an available  tree
public class MultipleResourcesSensor : ResourceSensor
{
    public float MinResourceValue = 1f;
    public float MinPowDistanceToBeNear = 1f;

    public override void UpdateSensor()
    {
        var worldState = memory.GetWorldState();

        foreach (var pair in MultipleResourcesManager.Instance.Resources)
        {
            var resourceManager = pair.Value;

            worldState.Set("see" + resourceManager.GetResourceName(), resourceManager.GetResourcesCount() >= MinResourceValue);

            UpdateResources(resourceManager);

            var nearestResource = Utilities.GetNearest(transform.position, resourcesPosition);
            worldState.Set("nearest" + resourceManager.GetResourceName(), nearestResource);
            worldState.Set("nearest" + resourceManager.GetResourceName() + "Position",
                (Vector3?) (nearestResource != null ? nearestResource.GetTransform().position : Vector3.zero));
        }
    }
}
