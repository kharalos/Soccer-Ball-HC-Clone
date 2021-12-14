using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragControl : MonoBehaviour
{
    private bool isDraging = false;
    private bool started = false;
    private bool launched;
    private Vector2 startTouch, swipeDelta;

    [SerializeField]
    private float dragAngle = 45f;

    //The values to smoothen the line angle. WIP
    [SerializeField]
    private float smoother = 10f;
    private float oldSmoother;

    [SerializeField]
    private LineRenderer line;

    [SerializeField]
    private Ball ballScript;

    private List<Vector3> hitPositions;

    private GameManager gm;

    private int maximumDeflect;

    private bool reachingGoal;

    public bool canDraw = true;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask interactMask;

    private Dummy targetDummy;
    private void Start()
    {
        canDraw = true;
        gm = FindObjectOfType<GameManager>();
        oldSmoother = smoother;
        hitPositions = new List<Vector3>();
        hitPositions.Clear();
        StartCoroutine(CreateLine());
    }
    public void Restart()
    {
        launched = false;
    }
    private void Update()
    {
        #region Standalone Inputs

        if (!launched)
        {
            if (Input.GetMouseButtonDown(0))
            {
                canDraw = true;
                isDraging = true;
                startTouch = Input.mousePosition;
                RayToInteractable();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDraging = false;
                if (Input.mousePosition.y < (Screen.height * 0.1f))
                {
                    Debug.Log("Mouse vertical position is : " + Input.mousePosition.y + " and the screen height is: " + Screen.height + " this means the no launch zone is: " + (Screen.height * 0.1f));
                    started = false;
                }
                Reset();
            }
            #endregion

            #region Mobile Input
            if (Input.touches.Length > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    canDraw = true;
                    isDraging = true;
                    startTouch = Input.touches[0].position;
                    RayToInteractable();
                }
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                {
                    if (Input.touches[0].position.y < (Screen.height * 0.1f))
                    {
                        started = false;
                    }

                    isDraging = false;
                    Reset();
                }
            }
            #endregion

        }
        //Calculate the distance
        swipeDelta = Vector2.zero;
        if (isDraging)
        {
            if (Input.touches.Length < 0)
                swipeDelta = Input.touches[0].position - startTouch;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
        }

        if (isDraging)
        {
            swipeDelta.x /= smoother;
            swipeDelta.x = Mathf.Clamp(swipeDelta.x, -dragAngle, dragAngle);
            transform.localRotation = Quaternion.Euler(0, swipeDelta.x, 0);

            if (targetDummy)
            {
                targetDummy.MoveDummy(swipeDelta.y);
            }

        }

        if (Input.mousePosition.y < (Screen.height * 0.1f))
        {
            line.startColor = Color.red;
            line.endColor = Color.red;
        }
        else
        {
            if (reachingGoal)
            {
                line.startColor = Color.green;
                line.endColor = Color.green;
            }
            else
            {
                line.startColor = Color.white;
                line.endColor = Color.white;
            }
        }
    }

    private void RayToInteractable()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, 100, interactMask))
        {
            canDraw = false;

            targetDummy = null;
            targetDummy = hit.transform.GetComponent<Dummy>();
        }
    }
    private IEnumerator CreateLine()
    {
        yield return new WaitForSeconds(0.02f);
        if (isDraging && canDraw)
        {
            hitPositions.Clear();
            line.positionCount = 1;
            DrawLine(transform.position, transform.forward);
            line.SetPosition(0, transform.position);
            maximumDeflect = 0;
        }
        StartCoroutine(CreateLine());
    }



    private void DrawLine(Vector3 vector, Vector3 forward)
    {
        reachingGoal = false;
        if (maximumDeflect > 15) return;
        maximumDeflect++;
        started = true;
        Debug.DrawRay(vector, forward * 10, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(vector, forward, out hit, 10f))
        {
            if (hit.transform.CompareTag("Dummy"))
            {
                Dummy dummy = hit.transform.GetComponent<Dummy>();
                Dummy.DummyType dt = dummy.type;

                AddLinePosition(hit.point);
                hitPositions.Add(hit.point);


                if (dt == Dummy.DummyType.Auto)
                {
                    DrawLine(hit.point, (FindNextDummyPosition(dummy.orderNumber) - hit.point).normalized);
                }
                else
                    DrawLine(hit.point, Vector3.Reflect(forward, hit.normal));

                //smoother = oldSmoother * line.positionCount;
            }
            if (hit.transform.CompareTag("Goal"))
            {
                Vector3 target = hit.transform.position;
                target.y = 0.3750002f;
                AddLinePosition(target + Vector3.forward);
                reachingGoal = true;
            }
        }
        else
        {
            AddLinePosition(vector + (forward * 10));
        }
    }
    private Vector3 FindNextDummyPosition(int currentOrder)
    {
        if (currentOrder > 0)
        {
            Vector3 target = gm.dummies[currentOrder - 1].gameObject.transform.position;
            target.y = 0.3750002f;
            return target;
        }
        else
            return GameObject.FindGameObjectWithTag("Goal").transform.position;
    }

    private void AddLinePosition(Vector3 hitPos)
    {
        line.positionCount++;
        line.SetPosition(line.positionCount - 1, hitPos);
    }

    private void Reset()
    {
        if (started)
        {
            hitPositions.Add(line.GetPosition(line.positionCount - 1));
            ballScript.GetPoints(hitPositions);
            hitPositions.Clear();
            started = false;
            gm.Launch();
            Debug.Log("Reset with " + (line.positionCount - 1) + " points.");
            launched = true;
        }
        targetDummy = null;
        //smoother = oldSmoother;
        line.positionCount = 0;
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }
}
