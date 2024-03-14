using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEditor.Experimental.GraphView;
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
        Stopwatch sw = Stopwatch.StartNew();

        gridSize = GridDrawer.Instance.gridSize;

        NativeArray<Color> colors = new NativeArray<Color>(gridSize * gridSize, Allocator.TempJob);
        SetColors setColors = new SetColors { colors = colors, gridSize = gridSize };
        JobHandle handle = setColors.ScheduleParallel(state.Dependency);
        handle.Complete();

        GridDrawer.Instance.SetPixel(colors.ToArray());

        colors.Dispose();

        sw.Stop();
        PerformanceData.Instance.setDrawTime(sw.ElapsedMilliseconds);
    }
}

[BurstCompile]
public partial struct SetColors : IJobEntity
{
    [NativeDisableParallelForRestriction]
    public NativeArray<Color> colors;
    public int gridSize;
    public void Execute(in CellAliveComponent cell, in CellPositionComponent pos)
    {
        if(cell.alive)
        {
            colors[pos.position.x + pos.position.y * gridSize] = Color.white;
        }
        else
        {
            colors[pos.position.x + pos.position.y * gridSize] = Color.black;
        }
    }
}
