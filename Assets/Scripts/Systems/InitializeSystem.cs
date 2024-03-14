using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Entity newCell = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(newCell, new CellPositionComponent
                {
                    position = new int2(i, j)
                });
                state.EntityManager.AddComponentData(newCell, new CellAliveComponent
                {
                    alive = false,
                    aliveNextGeneration = false
                });
                state.EntityManager.AddComponent(newCell, typeof(CellNeighborsComponent));
            }
        }

        foreach ((CellPositionComponent pos, Entity entity) in SystemAPI.Query<CellPositionComponent>().WithEntityAccess())
        {
            int x = pos.position.x;
            int y = pos.position.y;
            foreach ((CellPositionComponent potentialNeighbor, Entity neighbor) in SystemAPI.Query<CellPositionComponent>().WithEntityAccess())
            {
                if (Math.Abs(potentialNeighbor.position.x - x) <= 1 && Math.Abs(potentialNeighbor.position.y - y) <= 1)
                {
                    int dx = potentialNeighbor.position.x - x;
                    int dy = potentialNeighbor.position.y - y;

                        if (dx == -1 && dy == 1)
                        {
                            CellNeighborsComponent cellData = state.EntityManager.GetComponentData<CellNeighborsComponent>(entity);
                            cellData.topleft = neighbor;
                            state.EntityManager.SetComponentData(entity, cellData);
                        }
                        // Top Neighbor
                        else if (dx == 0 && dy == 1)
                        {
                            CellNeighborsComponent cellData = state.EntityManager.GetComponentData<CellNeighborsComponent>(entity);
                            cellData.top = neighbor;
                            state.EntityManager.SetComponentData(entity, cellData);
                        }
                        // Top-Right Neighbor
                        else if (dx == 1 && dy == 1)
                        {
                            CellNeighborsComponent cellData = state.EntityManager.GetComponentData<CellNeighborsComponent>(entity);
                            cellData.topright = neighbor;
                            state.EntityManager.SetComponentData(entity, cellData);
                        }
                        // Right Neighbor
                        else if (dx == 1 && dy == 0)
                        {
                            CellNeighborsComponent cellData = state.EntityManager.GetComponentData<CellNeighborsComponent>(entity);
                            cellData.right = neighbor;
                            state.EntityManager.SetComponentData(entity, cellData);
                        }
                        // Bottom-Right Neighbor
                        else if (dx == 1 && dy == -1)
                        {
                            CellNeighborsComponent cellData = state.EntityManager.GetComponentData<CellNeighborsComponent>(entity);
                            cellData.bottomRight = neighbor;
                            state.EntityManager.SetComponentData(entity, cellData);
                        }
                        // Bottom Neighbor
                        else if (dx == 0 && dy == -1)
                        {
                            CellNeighborsComponent cellData = state.EntityManager.GetComponentData<CellNeighborsComponent>(entity);
                            cellData.bottom = neighbor;
                            state.EntityManager.SetComponentData(entity, cellData);
                        }
                        // Bottom-Left Neighbor
                        else if (dx == -1 && dy == -1)
                        {
                            CellNeighborsComponent cellData = state.EntityManager.GetComponentData<CellNeighborsComponent>(entity);
                            cellData.bottomleft = neighbor;
                            state.EntityManager.SetComponentData(entity, cellData);
                        }
                        // Left Neighbor
                        else if (dx == -1 && dy == 0)
                        {
                            CellNeighborsComponent cellData = state.EntityManager.GetComponentData<CellNeighborsComponent>(entity);
                            cellData.left = neighbor;
                            state.EntityManager.SetComponentData(entity, cellData);
                        }
                    }
            }
        }
    }
}
