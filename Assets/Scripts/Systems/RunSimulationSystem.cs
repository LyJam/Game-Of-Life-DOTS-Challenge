using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateBefore(typeof(DrawSystem))]
public partial struct RunSimulationSystem : ISystem
{
    int gridSize;
    void OnCreate(ref SystemState state)
    {
        gridSize = 100;
    }
    void OnUpdate(ref SystemState state)
    {
        gridSize = GridDrawer.Instance.gridSize;

        NativeArray<bool> aliveArray = new NativeArray<bool>(gridSize * gridSize, Allocator.TempJob);
        SetAliveArray setAliveArray = new SetAliveArray {aliveArray = aliveArray };
        JobHandle setAliveArrayJobHandle = setAliveArray.ScheduleParallel(state.Dependency);
        setAliveArrayJobHandle.Complete();

        CalclulateNextGeneration calclulateNextGeneration = new CalclulateNextGeneration() { gridSize = gridSize, aliveArray = aliveArray };
        JobHandle calclulateNextGenerationJobHandle = calclulateNextGeneration.ScheduleParallel(state.Dependency);
        calclulateNextGenerationJobHandle.Complete();

        aliveArray.Dispose();
    }
}

[BurstCompile]
public partial struct SetAliveArray : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public NativeArray<bool> aliveArray;
    public void Execute(in CellAliveComponent cell, in CellPositionComponent pos)
    {
        if (cell.alive)
        {
            aliveArray[pos.position] = true;
        }
        else
        {
            aliveArray[pos.position] = false;
        }
    }
}


[BurstCompile]
public partial struct CalclulateNextGeneration : IJobEntity
{
    [ReadOnly] public NativeArray<bool> aliveArray;
    public int gridSize;
    public void Execute(ref CellAliveComponent cell, in CellPositionComponent pos)
    {
        int neighbors = 0;
        // Top
        if ((pos.position + gridSize) < gridSize * gridSize) if(aliveArray[pos.position + gridSize]) neighbors++;
        // Bottom
        if ((pos.position - gridSize) >= 0) if(aliveArray[pos.position - gridSize]) neighbors++;
        // Left
        if ((pos.position - 1) >= 0) if (aliveArray[pos.position - 1]) neighbors++;
        // Right
        if ((pos.position + 1) < gridSize * gridSize) if (aliveArray[pos.position + 1]) neighbors++;
        // Top-Left
        if ((pos.position + gridSize - 1) >= 0 && (pos.position + gridSize - 1) < gridSize * gridSize) if (aliveArray[pos.position + gridSize - 1]) neighbors++;
        // Top-Right
        if ((pos.position + gridSize + 1) < gridSize * gridSize) if (aliveArray[pos.position + gridSize + 1]) neighbors++;
        // Bottom-Left
        if ((pos.position - gridSize - 1) >= 0) if (aliveArray[pos.position - gridSize - 1]) neighbors++;
        // Bottom-Right
        if ((pos.position - gridSize + 1) >= 0 && (pos.position - gridSize + 1) < gridSize * gridSize) if (aliveArray[pos.position - gridSize + 1]) neighbors++;

        if (neighbors <= 1) cell.alive = false;
        if (neighbors >= 4) cell.alive = false;
        if (neighbors == 3) cell.alive = true;
    }
}
