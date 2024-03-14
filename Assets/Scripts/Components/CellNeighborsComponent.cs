using Unity.Entities;

public struct CellNeighborsComponent : IComponentData
{
    public Entity topleft;
    public Entity top;
    public Entity topright;
    public Entity left;
    public Entity right;
    public Entity bottomleft;
    public Entity bottom;
    public Entity bottomRight;
}
