using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Enemy : MonoBehaviour
{
    public float speed = 1;
    bool turning;
    public Animator enemyAnim;
    public GameObject eyePoint;
    public float raycastLength = 0.7f;
    private float desiredRot;
    public float rotSpeed = 250;
    public float damping = 10;
    float randomTurn;
    float turnUntil;
    float myGivenTime;
    bool canWalk;
    RaycastHit hit;
    bool freeze;
    public GameObject childCanvas;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        bool turning = false;
        freeze = false;
        desiredRot = transform.eulerAngles.y;
        WaitTime(0.5f);
    }

    void Update()
    {
        if (!CheckForObstacles() && WaitTime(0))
        {
            MoveForward();
        }
        else
        {
            StayStedy();
        }
        LookAroundSelf();
        Freezing();
    }

    void StayStedy(){

        enemyAnim.SetBool("Walk", false);
    }

    void MoveForward() {
        enemyAnim.SetBool("Walk", true);
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    void TurnInOtherDir(){
        // turn into random direction
        if(!turning && WaitTime(0)){
            turning = true;
            randomTurn = Random.Range(-180f, 180f);
            turnUntil = randomTurn - transform.eulerAngles.x;
            turnUntil = Mathf.Abs(turnUntil) / rotSpeed;
            enemyAnim.SetTrigger("LookAround");
            WaitTime(4);
        }
    }

    void LookAroundSelf(){
        if (turning) {
            if(turnUntil > 0){
                //turn left or right
                if (randomTurn < 0) desiredRot -= rotSpeed * Time.deltaTime;
                else desiredRot += rotSpeed * Time.deltaTime;

                turnUntil -= Time.deltaTime;
            } else {
                turning = false;
            }
        }
        // apply rotation
        var desiredRotation = Quaternion.Euler(transform.eulerAngles.x, desiredRot, transform.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * damping);
    }

    bool CheckForObstacles(){
        var rightEyePoint = eyePoint.transform.position + eyePoint.transform.right;
        var leftEyePoint = eyePoint.transform.position - eyePoint.transform.right;

        if (Physics.Raycast(eyePoint.transform.position, transform.TransformDirection(Vector3.forward), out hit, raycastLength, 9)){
            TurnInOtherDir();
            Debug.DrawRay(eyePoint.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return true;
        }
        if (Physics.Raycast(rightEyePoint, transform.TransformDirection(Vector3.forward), out hit, raycastLength, 9))
        {
            TurnInOtherDir();
            Debug.DrawRay(rightEyePoint, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return true;
        }
        if (Physics.Raycast(leftEyePoint, transform.TransformDirection(Vector3.forward), out hit, raycastLength, 9))
        {
            TurnInOtherDir();
            Debug.DrawRay(leftEyePoint, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return true;
        }
        Debug.DrawRay(leftEyePoint, transform.TransformDirection(Vector3.forward) * raycastLength, Color.white);
        Debug.DrawRay(rightEyePoint, transform.TransformDirection(Vector3.forward) * raycastLength, Color.white);
        Debug.DrawRay(eyePoint.transform.position, transform.TransformDirection(Vector3.forward) * raycastLength, Color.white);
        return false;
    }

    bool WaitTime(float givenTime) {
        // count down time
        myGivenTime += givenTime;
        if(myGivenTime > 0){
            myGivenTime -= Time.deltaTime;
            return false;
        } else {
            return true;
        }
    }

    public void Freeze()
    {
        freeze = true;
        FindObjectOfType<AudioManager>().Play("BlueEnemyFreeze");
    }

    async void Freezing()
    {
        if (freeze && WaitTime(0))
        {
            childCanvas.GetComponent<CountTimerEnemy>().StartTimer();
            freeze = false;
            StayStedy();
            enemyAnim.SetBool("Freeze", true);
            WaitTime(4);
            await Task.Delay(4000);
            enemyAnim.SetBool("Freeze", false);
            TurnInOtherDir();
        }
    }
}