using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial struct GridSystem : ISystem
{
    private int gridSize;
    void OnCreate(ref SystemState state) 
    {
        gridSize = GridAuthoring.gridSize;

        InitCells(state);
        foreach (Cell cell in SystemAPI.Query<Cell>())
        {
            if (cell.top != Entity.Null)
            {
                Cell cellData = state.EntityManager.GetComponentData<Cell>(cell.top);
                Debug.Log(cellData.position);
            }
        }
    }
    void OnUpdate(ref SystemState state) 
    {
        
    }

    public void InitCells(SystemState state)
    {
        for(int i = 0;i < gridSize;i++)
        {
            for(int j = 0;j < gridSize;j++)
            {
                Entity newCell = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponent(newCell, typeof(Cell));
                state.EntityManager.AddComponentData(newCell, new Cell
                {
                    alive = false,
                    position = new int2(i, j)
                });
            }
        }

        foreach((Cell cell, Entity entity) in SystemAPI.Query<Cell>().WithEntityAccess())
        {
            int x = cell.position.x;
            int y = cell.position.y;
            foreach ((Cell potentialNeighbor, Entity neighbor) in SystemAPI.Query<Cell>().WithEntityAccess())
            {
                if(Math.Abs(potentialNeighbor.position.x - x) <= 1 && Math.Abs(potentialNeighbor.position.y - y) <= 1)
                {
                    int dx = potentialNeighbor.position.x - x;
                    int dy = potentialNeighbor.position.y - y;

                    // Top-Left Neighbor
                    if (dx == -1 && dy == 1)
                    {
                        Cell cellData = state.EntityManager.GetComponentData<Cell>(entity);
                        cellData.topleft = neighbor;
                        state.EntityManager.SetComponentData(entity, cellData);
                    }
                    // Top Neighbor
                    else if (dx == 0 && dy == 1)
                    {
                        Cell cellData = state.EntityManager.GetComponentData<Cell>(entity);
                        cellData.top = neighbor;
                        state.EntityManager.SetComponentData(entity, cellData);
                    }
                    // Top-Right Neighbor
                    else if (dx == 1 && dy == 1)
                    {
                        Cell cellData = state.EntityManager.GetComponentData<Cell>(entity);
                        cellData.topright = neighbor;
                        state.EntityManager.SetComponentData(entity, cellData);
                    }
                    // Right Neighbor
                    else if (dx == 1 && dy == 0)
                    {
                        Cell cellData = state.EntityManager.GetComponentData<Cell>(entity);
                        cellData.right = neighbor;
                        state.EntityManager.SetComponentData(entity, cellData);
                    }
                    // Bottom-Right Neighbor
                    else if (dx == 1 && dy == -1)
                    {
                        Cell cellData = state.EntityManager.GetComponentData<Cell>(entity);
                        cellData.bottomRight = neighbor;
                        state.EntityManager.SetComponentData(entity, cellData);
                    }
                    // Bottom Neighbor
                    else if (dx == 0 && dy == -1)
                    {
                        Cell cellData = state.EntityManager.GetComponentData<Cell>(entity);
                        cellData.bottom = neighbor;
                        state.EntityManager.SetComponentData(entity, cellData);
                    }
                    // Bottom-Left Neighbor
                    else if (dx == -1 && dy == -1)
                    {
                        Cell cellData = state.EntityManager.GetComponentData<Cell>(entity);
                        cellData.bottomleft = neighbor;
                        state.EntityManager.SetComponentData(entity, cellData);
                    }
                    // Left Neighbor
                    else if (dx == -1 && dy == 0)
                    {
                        Cell cellData = state.EntityManager.GetComponentData<Cell>(entity);
                        cellData.left = neighbor;
                        state.EntityManager.SetComponentData(entity, cellData);
                    }
                }
            }
        }
    }
}
