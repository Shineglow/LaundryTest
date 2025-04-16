using Assets.LaundryTest.Buildings.Blocks;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BuildObject : MonoBehaviour
{
    private Vector3 _size = Vector3.one;

    public Vector3 Size
    {
        get => _size;
        set
        {
            _size = value;
            _halfSize = value / 2f;
        }
    }
    private Vector3 _halfSize;
    public Vector3 HalfSize => _halfSize;
    private Collider _collider;

    public List<SidePhysics> AllSides;
    public SidePhysics Bottom; 

    public void SetCollisionsEnabled(bool isEnabled)
    {
        _collider.enabled = isEnabled;
        AllSides.ForEach(i => {
            i.Collider.enabled = isEnabled;
            i.Rigidbody.detectCollisions = isEnabled;
        });
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
}
