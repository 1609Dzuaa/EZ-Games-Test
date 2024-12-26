using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class BakeMesh : BaseSingleton<BakeMesh>
{
    [SerializeField] NavMeshSurface _navMeshSurface;

    public void Rebaking()
    {
        _navMeshSurface.BuildNavMesh();
    }
}
