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
//[UpdateBefore(typeof(DrawSystem))]
public partial struct RunSimulationSystem : ISystem
{
    int gridSize;
    NativeArray<byte> aliveArray;
    void OnCreate(ref SystemState state)
    {
        gridSize = 100;
        aliveArray = new NativeArray<byte>(gridSize * gridSize, Allocator.TempJob);
        CreateAliveArray();
    }

    void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer buffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach ((ButtonPressed reset, Entity e) in SystemAPI.Query<ButtonPressed>().WithEntityAccess())
        {
            gridSize = GridDrawer.Instance.gridSize;
            CreateAliveArray();
            buffer.DestroyEntity(e);
        }
        buffer.Playback(state.EntityManager);

        SetAliveArray setAliveArray = new SetAliveArray {aliveArray = aliveArray , gridSize = gridSize};
        JobHandle setAliveArrayJobHandle = setAliveArray.ScheduleParallel(state.Dependency);
        setAliveArrayJobHandle.Complete();

        CalclulateNextGeneration calclulateNextGeneration = new CalclulateNextGeneration() { gridSize = gridSize, aliveArray = aliveArray };
        JobHandle calclulateNextGenerationJobHandle = calclulateNextGeneration.ScheduleParallel(state.Dependency);

        GridDrawer.Instance.ApplyTexture(aliveArray);
        
        calclulateNextGenerationJobHandle.Complete();
    }

    private void CreateAliveArray()
    {
        aliveArray.Dispose();

        //the '+ 2' will create a border around the actual grid. this is usefull because then we don't have to check if we are out of bounds of the array in the CalclulateNextGeneration job
        aliveArray = new NativeArray<byte>((gridSize + 2) * (gridSize + 2), Allocator.TempJob);
        for (int i = 0; i < (gridSize + 2) * (gridSize + 2); i++) 
        {
            aliveArray[i] = 0xFF;
        }
    }
}


[BurstCompile]
public partial struct SetAliveArray : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public NativeArray<byte> aliveArray;
    [ReadOnly] public int gridSize;
    public void Execute(in CellAliveComponent cell, in CellPositionComponent pos)
    {
        int position = pos.position + gridSize + 1;
        if (cell.alive)
        {
            aliveArray[position] = 0x00;
        }
        else
        {
            aliveArray[position] = 0xFF;
        }
    }
}


[BurstCompile]
public partial struct CalclulateNextGeneration : IJobEntity
{
    [ReadOnly] public NativeArray<byte> aliveArray;
    [ReadOnly] public int gridSize;
    public void Execute(ref CellAliveComponent cell, in CellPositionComponent pos)
    {
        int neighbors = 0;
        int adjustedPos = pos.position + gridSize + 1;
        int top = adjustedPos + gridSize;
        int bottom = adjustedPos - gridSize;
        // Top
        if(aliveArray[top] == 0x00) neighbors++;
        // Bottom
        if(aliveArray[bottom] == 0x00) neighbors++;
        // Left
        if (aliveArray[adjustedPos - 1] == 0x00) neighbors++;
        // Right
        if (aliveArray[adjustedPos + 1] == 0x00) neighbors++;
        // Top-Left
        if (aliveArray[top - 1] == 0x00) neighbors++;
        // Top-Right
        if (aliveArray[top + 1] == 0x00) neighbors++;
        // Bottom-Left
        if (aliveArray[bottom - 1] == 0x00) neighbors++;
        // Bottom-Right
        if (aliveArray[bottom + 1] == 0x00) neighbors++;

        if (neighbors <= 1) cell.alive = false;
        if (neighbors >= 4) cell.alive = false;
        if (neighbors == 3) cell.alive = true;
    }
}
