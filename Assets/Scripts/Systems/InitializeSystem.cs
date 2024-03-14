using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[UpdateBefore(typeof(RunSimulationSystem))]
public partial struct InitializeSystem : ISystem
{
    private int gridSize;

    void OnCreate(ref SystemState state)
    {
        gridSize = GridDrawer.gridSize;

        InitCells(state);
    }
    void OnUpdate(ref SystemState state)
    {

    }

    public void InitCells(SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Entity newCell = ecb.CreateEntity();
                ecb.AddComponent(newCell, new CellPositionComponent
                {
                    position = new int2(i, j)
                });
                ecb.AddComponent(newCell, new CellAliveComponent
                {
                    alive = (UnityEngine.Random.value < 0.5f),
                });
            }
        }
        ecb.Playback(state.EntityManager);
    }
}
