using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;

    private object _gridObject;

    public virtual void SetGridObject(object gridObject)
    {
        this._gridObject = gridObject;
    }

    protected virtual void Update()
    {
        textMeshPro.text = _gridObject.ToString();
    }
}
