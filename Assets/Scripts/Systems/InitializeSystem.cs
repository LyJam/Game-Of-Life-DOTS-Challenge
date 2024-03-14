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
                    aliveNextGeneration = false
                });
                ecb.AddComponent(newCell, typeof(CellNeighborsComponent));
            }
        }
        ecb.Playback(state.EntityManager);

        NativeArray<Entity> gridEntities = new NativeArray<Entity>(gridSize * gridSize, Allocator.Persistent);
        foreach((CellPositionComponent pos, Entity e) in SystemAPI.Query<CellPositionComponent>().WithEntityAccess())
        {
            int index = pos.position.x + pos.position.y * gridSize; // Convert 2D position to 1D index
            if (index >= 0 && index < gridSize * gridSize)
            {
                gridEntities[index] = e; // Store the entity reference
            }
        }

        foreach ((CellPositionComponent pos, Entity entity) in SystemAPI.Query<CellPositionComponent>().WithEntityAccess())
        {
            int x = pos.position.x;
            int y = pos.position.y;
            CellNeighborsComponent cellData = state.EntityManager.GetComponentData<CellNeighborsComponent>(entity);
            // Top
            if (y + 1 < gridSize) cellData.top = gridEntities[x + (y + 1) * gridSize];
            // Bottom
            if (y - 1 >= 0) cellData.bottom = gridEntities[x + (y - 1) * gridSize];
            // Left
            if (x - 1 >= 0) cellData.left = gridEntities[(x - 1) + y * gridSize];
            // Right
            if (x + 1 < gridSize) cellData.right = gridEntities[(x + 1) + y * gridSize];
            // Top-Left
            if (x - 1 >= 0 && y + 1 < gridSize) cellData.topleft = gridEntities[(x - 1) + (y + 1) * gridSize];
            // Top-Right
            if (x + 1 < gridSize && y + 1 < gridSize) cellData.topright = gridEntities[(x + 1) + (y + 1) * gridSize];
            // Bottom-Left
            if (x - 1 >= 0 && y - 1 >= 0) cellData.bottomleft = gridEntities[(x - 1) + (y - 1) * gridSize];
            // Bottom-Right
            if (x + 1 < gridSize && y - 1 >= 0) cellData.bottomRight = gridEntities[(x + 1) + (y - 1) * gridSize];
            state.EntityManager.SetComponentData(entity, cellData);
        }

        gridEntities.Dispose();

        /*
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
        */
    }
}
