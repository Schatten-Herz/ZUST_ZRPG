using System;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructibleCreate.OnAnyDestroyed += DestructibleCreateOnAnyDestroyed;
    }

    private void DestructibleCreateOnAnyDestroyed(object sender, EventArgs e)
    {
        DestructibleCreate destructibleCreate = sender as DestructibleCreate;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructibleCreate.GetGridPosition(),true);
    }
}
