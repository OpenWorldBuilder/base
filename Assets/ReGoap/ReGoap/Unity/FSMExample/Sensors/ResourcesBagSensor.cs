﻿using UnityEngine;
using System.Collections;

public class ResourcesBagSensor : ReGoapSensor<string, object>
{
    private ResourcesBag resourcesBag;

    void Awake()
    {
        resourcesBag = GetComponent<ResourcesBag>();
    }

    public override void UpdateSensor()
    {
        var state = memory.GetWorldState();
        foreach (var pair in resourcesBag.GetResources())
        {
            state.Set("hasResource" + pair.Key, pair.Value > 0);
        }
    }
}
