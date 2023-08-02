using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class EnemyBlu : MonoBehaviour
{
    public float speed = 1;
    public float chaseSpeed = 3;
    bool chasing = false;
    bool waiting = false;
    public Animator enemyAnim;
    public GameObject eyePoint;
    public float raycastLength = 5f;
    private float desiredRot;
    public float damping = 10;
    RaycastHit hit;
    Quaternion lookRotation;
    public GameObject player;

    // waypoint variables
    private Vector3 NextWayPoint;
    private List<GameObject> WaypointList = new List<GameObject>();
    public Transform Path;
    private List<Transform> Waypoints;
    private int currentWaypoint = 0;
    public float CheckpointDistance = 50f;

    void Start()
    {
        WaypointList.AddRange(GameObject.FindGameObjectsWithTag("WayPoint"));

        Transform[] PathTransforms = Path.GetComponentsInChildren<Transform>();
        Waypoints = new List<Transform>();

        for (int i = 0; i < PathTransforms.Length; i++)
        {
            if (PathTransforms[i] != Path.transform)
            {
                Waypoints.Add(PathTransforms[i]);
            }
        }
    }

    void Update()
    {
        Walking();
        Chasing();
        FaceDirection();
        Rotate();
        SwitchWaypointOnTouch();
        RaycastForPlayer();
        CheckForObstacles();


        DebugNextWaypoint();
    }


    void RaycastForPlayer(){
        // eye variables
        var rightEyePoint = eyePoint.transform.position + eyePoint.transform.right;
        var leftEyePoint = eyePoint.transform.position - eyePoint.transform.right;
        var rightEyeAngle = transform.TransformDirection((Vector3.forward*0.66f) + (Vector3.right*0.33f));
        var leftEyeAngle = transform.TransformDirection((Vector3.forward*2/3) - (Vector3.right/3));

        // Center eye
        if (Physics.Raycast(eyePoint.transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastLength)){
            Debug.DrawRay(eyePoint.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            WhoIsHitted(hit);
        } else
        {
            Debug.DrawRay(eyePoint.transform.position, transform.TransformDirection(Vector3.forward) * raycastLength, Color.white);
        }
        // Right eye
        if (Physics.Raycast(rightEyePoint, rightEyeAngle, out hit, raycastLength))
        {
            Debug.DrawRay(rightEyePoint, rightEyeAngle * hit.distance, Color.yellow);
            WhoIsHitted(hit);
        } else
        {
            Debug.DrawRay(rightEyePoint, rightEyeAngle * raycastLength, Color.white);
        }
        // Left eye
        if (Physics.Raycast(leftEyePoint, leftEyeAngle, out hit, raycastLength))
        {
            Debug.DrawRay(leftEyePoint, leftEyeAngle * hit.distance, Color.yellow);
            WhoIsHitted(hit);
        } else
        {
            Debug.DrawRay(leftEyePoint, leftEyeAngle * raycastLength, Color.white);
        }
        
    }

    void Chase()
    {
        chasing = true;
        waiting = false;
        enemyAnim.SetBool("isChasing", true);
        enemyAnim.SetBool("isWaiting", false);
    }

    void Walk()
    {
        chasing = false;
        waiting = false;
        enemyAnim.SetBool("isChasing", false);
        enemyAnim.SetBool("isWaiting", false);
    }

    async void Wait()
    {
        chasing = false;
        waiting = true;
        enemyAnim.SetBool("isChasing", false);
        enemyAnim.SetBool("isWaiting", true);
        await Task.Delay(2000);
        Walk();
    }

    void Walking()
    {
        if(!chasing && !waiting)
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    void Chasing()
    {
        if (chasing)
        {
            transform.position += transform.forward * Time.deltaTime * chaseSpeed;
        }
    }

    void FaceDirection()
    {
        if (!chasing)
            lookRotation = Quaternion.LookRotation(Waypoints[currentWaypoint].position - transform.position);
        if (chasing)
        {
            lookRotation = Quaternion.LookRotation(player.transform.position - eyePoint.transform.position);

            if (Physics.Linecast(eyePoint.transform.position, player.transform.position, out RaycastHit hitInfo)){
                if (hitInfo.transform.tag == "Ground" || player.transform.parent.gameObject.GetComponent<MovementPlayer>().isDead)
                {
                    Walk();
                }
            }
            Debug.DrawLine(eyePoint.transform.position, player.transform.position, Color.red);
        }
    }

    void Rotate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * damping);
    }

    void SwitchWaypointOnTouch()
    {
        if (Vector3.Distance(transform.position, Waypoints[currentWaypoint].position) < CheckpointDistance)
        {
            if (currentWaypoint == Waypoints.Count - 1)
            {
                currentWaypoint = 0;
            }
            else
            {
                currentWaypoint++;
            }
        }
    }

    void WhoIsHitted(RaycastHit givenHitObject)
    {
        if(givenHitObject.transform.tag == "Player")
        {
            Chase();
        }
    }

    void DebugNextWaypoint()
    {
        Debug.DrawLine(eyePoint.transform.position, Waypoints[currentWaypoint].position, Color.green);
    }



    void CheckForObstacles()
    {
        var rightEyePoint = eyePoint.transform.position + eyePoint.transform.right;
        var leftEyePoint = eyePoint.transform.position - eyePoint.transform.right;

        if (Physics.Raycast(eyePoint.transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastLength/2, 9))
        {
            Debug.DrawRay(eyePoint.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.cyan);
            if (hit.transform.tag == "Enemy") Wait();
        } else
        {
            Debug.DrawRay(eyePoint.transform.position, transform.TransformDirection(Vector3.forward) * raycastLength/2, Color.blue);
        }
        if (Physics.Raycast(rightEyePoint, transform.TransformDirection(Vector3.forward), out hit, raycastLength/3, 9))
        {
            Debug.DrawRay(rightEyePoint, transform.TransformDirection(Vector3.forward) * hit.distance, Color.cyan);
            if (hit.transform.tag == "Enemy") Wait();
        } else
        {
            Debug.DrawRay(rightEyePoint, transform.TransformDirection(Vector3.forward) * raycastLength/3, Color.blue);
        }
        if (Physics.Raycast(leftEyePoint, transform.TransformDirection(Vector3.forward), out hit, raycastLength/3, 9))
        {
            Debug.DrawRay(leftEyePoint, transform.TransformDirection(Vector3.forward) * hit.distance, Color.cyan);
            if (hit.transform.tag == "Enemy") Wait();
        } else
        {
            Debug.DrawRay(leftEyePoint, transform.TransformDirection(Vector3.forward) * raycastLength/3, Color.blue);
        }
    }
}
