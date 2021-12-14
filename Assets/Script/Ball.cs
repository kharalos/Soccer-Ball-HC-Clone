using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 target;
    private int targetID;
    private int targetLeft = 10;
    [SerializeField]
    //private float speed = 30f;
    private List<Vector3> movePositions;
    private Rigidbody rigidBody;

    [SerializeField]
    private ParticleSystem ps;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        target = transform.position;
    }


    public void GetPoints(List<Vector3> positions)
    {
        targetID = 0;
        movePositions = new List<Vector3>(positions);
        Debug.Log("Count is " + positions.Count);
        targetLeft = positions.Count;
        int i = 1;
        foreach (Vector3 pos in positions)
        {
            Debug.Log("Position " + i + " is " + pos);
            i++;
        }
        SetTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dummy"))
        {
            targetID++;
            SetTarget();
            targetLeft--;
            Debug.Log(targetLeft);
        }
        if (other.gameObject.CompareTag("Goal"))
        {
            Goal();
        }
    }
    public float force = 1000f;
    private void SetTarget()
    {
        if (movePositions.Count > targetID)
            target = movePositions[targetID];
        else
            target = transform.position + transform.forward * 10;

        rigidBody.velocity = ((Vector3.up * (0.8f - 0.2f/targetLeft)) + (target - transform.position).normalized) * force;
    }

    private void Goal()
    {
        FindObjectOfType<GameManager>().Victory();
        StartCelebration();
    }

    public void StartCelebration()
    {
        ps.Play();
    }

    public void EndCelebration()
    {
        ps.Stop();
    }

    private void Update()
    {
        //rigidBody.velocity = (target - transform.position) * speed * Time.deltaTime;
        if (targetLeft < 2 && GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            Debug.Log("Ball Stopped");
            targetLeft = 10;
            FindObjectOfType<GameManager>().Defeat();
        }
    }
}
