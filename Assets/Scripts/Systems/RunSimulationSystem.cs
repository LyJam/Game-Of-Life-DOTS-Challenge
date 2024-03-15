using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(DrawSystem))]
public partial struct RunSimulationSystem : ISystem
{
    int gridSize;
    NativeArray<bool> aliveArrayPrefab;
    void OnCreate(ref SystemState state)
    {
        gridSize = 100;
        aliveArrayPrefab = new NativeArray<bool>(gridSize * gridSize, Allocator.TempJob);
        SetAliveArrayPrefab();
    }
    void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach ((ButtonPressed reset, Entity e) in SystemAPI.Query<ButtonPressed>().WithEntityAccess())
        {
            gridSize = GridDrawer.Instance.gridSize;
            SetAliveArrayPrefab();
            buffer.DestroyEntity(e);
        }
        buffer.Playback(state.EntityManager);

        NativeArray<bool> aliveArray = new NativeArray<bool>((gridSize + 2) * (gridSize + 2), Allocator.TempJob);
        NativeArray<bool>.Copy(aliveArrayPrefab, aliveArray);

        SetAliveArray setAliveArray = new SetAliveArray {aliveArray = aliveArray , gridSize = gridSize};
        JobHandle setAliveArrayJobHandle = setAliveArray.ScheduleParallel(state.Dependency);
        setAliveArrayJobHandle.Complete();

        CalclulateNextGeneration calclulateNextGeneration = new CalclulateNextGeneration() { gridSize = gridSize, aliveArray = aliveArray };
        JobHandle calclulateNextGenerationJobHandle = calclulateNextGeneration.ScheduleParallel(state.Dependency);
        calclulateNextGenerationJobHandle.Complete();

        aliveArray.Dispose();
    }

    private void SetAliveArrayPrefab()
    {
        aliveArrayPrefab.Dispose();

        //the '+ 2' will create a border around the actual grid. this is usefull because then we don't have to check if we are out of bounds of the array in the CalclulateNextGeneration job
        aliveArrayPrefab = new NativeArray<bool>((gridSize + 2) * (gridSize + 2), Allocator.TempJob);
        for (int i = 0; i < (gridSize + 2) * (gridSize + 2); i++) 
        {
            aliveArrayPrefab[i] = false;
        }
    }
}


[BurstCompile]
public partial struct SetAliveArray : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public NativeArray<bool> aliveArray;
    [ReadOnly] public int gridSize;
    public void Execute(in CellAliveComponent cell, in CellPositionComponent pos)
    {
        if (cell.alive)
        {
            aliveArray[pos.position + gridSize + 1] = true;
        }
    }
}


[BurstCompile]
public partial struct CalclulateNextGeneration : IJobEntity
{
    [ReadOnly] public NativeArray<bool> aliveArray;
    [ReadOnly] public int gridSize;
    public void Execute(ref CellAliveComponent cell, in CellPositionComponent pos)
    {
        int neighbors = 0;
        int adjustedPos = pos.position + gridSize + 1;
        int top = adjustedPos + gridSize;
        int bottom = adjustedPos - gridSize;
        // Top
        if(aliveArray[top]) neighbors++;
        // Bottom
        if(aliveArray[bottom]) neighbors++;
        // Left
        if (aliveArray[adjustedPos - 1]) neighbors++;
        // Right
        if (aliveArray[adjustedPos + 1]) neighbors++;
        // Top-Left
        if (aliveArray[top - 1]) neighbors++;
        // Top-Right
        if (aliveArray[top + 1]) neighbors++;
        // Bottom-Left
        if (aliveArray[bottom - 1]) neighbors++;
        // Bottom-Right
        if (aliveArray[bottom + 1]) neighbors++;

        if (neighbors <= 1) cell.alive = false;
        if (neighbors >= 4) cell.alive = false;
        if (neighbors == 3) cell.alive = true;
    }
}
