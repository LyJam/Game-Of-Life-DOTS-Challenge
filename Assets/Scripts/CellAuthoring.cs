using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CellAuthoring : MonoBehaviour
{
    public class Baker : Baker<CellAuthoring>
    {
        public override void Bake(CellAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.None);
            AddComponent(e, new Cell { alive = false}) ;
        }
    }
}


public struct Cell : IComponentData
{
    public bool alive;
    public int2 position;

    public Entity topleft;
    public Entity top;
    public Entity topright;
    public Entity left;
    public Entity right;
    public Entity bottomleft;
    public Entity bottom;
    public Entity bottomRight;
}

