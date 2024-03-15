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
       
        SetAliveArray setAliveArray = new SetAliveArray { gridSize = gridSize, aliveArray = aliveArray };
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
    public int gridSize;
    public void Execute(in CellAliveComponent cell, in CellPositionComponent pos)
    {
        if (cell.alive)
        {
            aliveArray[pos.position.x + pos.position.y * gridSize] = true;
        }
        else
        {
            aliveArray[pos.position.x + pos.position.y * gridSize] = false;
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
        int x = pos.position.x;
        int y = pos.position.y;
        // Top
        if (y + 1 < gridSize) if(aliveArray[x + (y + 1) * gridSize]) neighbors++;
        // Bottom
        if (y - 1 >= 0) if(aliveArray[x + (y - 1) * gridSize]) neighbors++;
        // Left
        if (x - 1 >= 0) if (aliveArray[(x - 1) + y * gridSize]) neighbors++;
        // Right
        if (x + 1 < gridSize) if (aliveArray[(x + 1) + y * gridSize]) neighbors++;
        // Top-Left
        if (x - 1 >= 0 && y + 1 < gridSize) if (aliveArray[(x - 1) + (y + 1) * gridSize]) neighbors++;
        // Top-Right
        if (x + 1 < gridSize && y + 1 < gridSize) if (aliveArray[(x + 1) + (y + 1) * gridSize]) neighbors++;
        // Bottom-Left
        if (x - 1 >= 0 && y - 1 >= 0) if (aliveArray[(x - 1) + (y - 1) * gridSize]) neighbors++;
        // Bottom-Right
        if (x + 1 < gridSize && y - 1 >= 0) if (aliveArray[(x + 1) + (y - 1) * gridSize]) neighbors++;

        if (neighbors <= 1) cell.alive = false;
        if (neighbors >= 4) cell.alive = false;
        if (neighbors == 3) cell.alive = true;
    }
}
