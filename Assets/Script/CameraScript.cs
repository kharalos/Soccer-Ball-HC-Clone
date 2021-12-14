using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public bool followBall;

    private bool positionedCamera;

    public Transform ball;

    private Vector3 startPos;
    private Quaternion startRot;
    private void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }
    public int xax = 4, yax = 5;
    private void Update()
    {
        if (followBall)
        {
            transform.position = Vector3.Lerp(transform.position, ball.transform.position + Vector3.up * xax + Vector3.forward * -yax, 0.8f);
            transform.LookAt(ball.position);

            if (positionedCamera)
                positionedCamera = false;
        }
        else
        {
            if (!positionedCamera)
            {
                transform.position = startPos;
                transform.rotation = startRot;
            }
        }
    }
}
