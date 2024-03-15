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
        gridSize = 100;

        EntityCommandBuffer buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        InitCells(state, buffer);
        buffer.Playback(state.EntityManager);
    }
    void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach ((ButtonPressed reset, Entity e) in SystemAPI.Query<ButtonPressed>().WithEntityAccess())
        {
            RemoveCells(state, buffer);
            gridSize = GridDrawer.Instance.gridSize;
            InitCells(state, buffer);
        }
        buffer.Playback(state.EntityManager);
    }

    public void RemoveCells(SystemState state, EntityCommandBuffer ecb)
    {
        foreach((CellAliveComponent alive, Entity e) in SystemAPI.Query<CellAliveComponent>().WithEntityAccess())
        {
            ecb.DestroyEntity(e);
        }
    }

    public void InitCells(SystemState state, EntityCommandBuffer ecb)
    {
        for (int i = 0; i < (gridSize * gridSize); i++)
        {
            Entity newCell = ecb.CreateEntity();
            ecb.AddComponent(newCell, new CellPositionComponent
            {
                position = i
            });
            ecb.AddComponent(newCell, new CellAliveComponent
            {
                alive = (UnityEngine.Random.value < 0.5f),
            });
        }
    }
}
