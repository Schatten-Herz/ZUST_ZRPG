using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float gravity = 9.8f;
    private CharacterController _cc;

    private Vector3 _velocity;
    
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask MousePlane;
    public bool isGround;

    private void Start()
    {
        _cc = GetComponent<CharacterController>();
    }

    
    private void Update()
    {
        isGround = Physics.CheckSphere(groundCheck.position, checkRadius, MousePlane);
        if (isGround && _velocity.y < 0)
        {
            _velocity.y = -3f;
        }
        
        _velocity.y -= gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }
}
