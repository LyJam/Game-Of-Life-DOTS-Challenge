using Unity.Entities;

public struct CellAliveComponent : IComponentData
{
    public bool alive;
    public bool aliveNextGeneration;
}
