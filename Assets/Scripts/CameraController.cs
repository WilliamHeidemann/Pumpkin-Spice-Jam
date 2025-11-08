using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private float _speed;


    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        MoveCamera(horizontal, vertical);
    }

    private void MoveCamera(float horizontal, float vertical)
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;
        
        Vector3 moveDir = forward * vertical * 1.3f + right * horizontal;
        transform.position += moveDir * _speed * Time.deltaTime;

    }
}