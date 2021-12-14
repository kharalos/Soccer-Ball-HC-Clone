using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dummy[] dummies;
    [SerializeField]
    private Ball ballScript;
    [SerializeField]
    private DragControl dc;
    [SerializeField]
    private Transform ballStartPos;
    [SerializeField]
    private CameraScript camScript;

    private bool win;

    public void Launch()
    {
        camScript.followBall = true;
    }
    public void Defeat()
    {
        if (win) return;
        ballScript.transform.position = ballStartPos.position;
        ballScript.target = ballStartPos.position;
        camScript.followBall = false;
        dc.Restart();
    }

    public void Victory()
    {
        if (win) return;
        Debug.Log("Goal");
        win = true;
    }

    public void GenerateLevel()
    {
        ballScript.EndCelebration();
    }
}
