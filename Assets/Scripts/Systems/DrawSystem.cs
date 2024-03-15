using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public partial struct DrawSystem : ISystem
{
    int gridSize;
    void OnCreate(ref SystemState state)
    {
        gridSize = 100;
    }
    void OnUpdate(ref SystemState state)
    {
        gridSize = GridDrawer.Instance.gridSize;

        NativeArray<byte> colors = new NativeArray<byte>(gridSize * gridSize, Allocator.TempJob);
        SetColors setColors = new SetColors { colors = colors};
        JobHandle handle = setColors.ScheduleParallel(state.Dependency);
        handle.Complete();

        GridDrawer.Instance.SetPixels(colors);
        
        colors.Dispose();
    }
}

[BurstCompile]
public partial struct SetColors : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public NativeArray<byte> colors;
    public void Execute(in CellAliveComponent cell, in CellPositionComponent pos)
    {
        if(cell.alive)
        {
            colors[pos.position] = 0x00;
        }
        else
        {
            colors[pos.position] = 0xFF;
        }
    }
}
