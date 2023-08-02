using UnityEngine;
using System.Collections;
 
// Makes objects float up & down while gently spinning.
public class FloatingPoint : MonoBehaviour {
    // User Inputs
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public GameObject pointElement;
    public float speedTowardsPlayer;
    float speedUpValue;

    // Position Storage Variables
    Vector3 posOffset = new Vector3 ();
    Vector3 tempPos = new Vector3 ();
    Vector3 offsetPointElement = new Vector3 ();

    // Use this for initialization
    void Start () {
        offsetPointElement = pointElement.transform.position - transform.position;
        speedUpValue = 0;
    }
     
    // Update is called once per frame
    void Update () {
        // Spin object around Y-Axis
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
 
        // Float up/down with a Sin()
        posOffset = transform.position;
        tempPos = posOffset;
        tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;
 
        pointElement.transform.position = tempPos + offsetPointElement;
    }

    //Follow player if close enough
    void OnTriggerStay(Collider colliderInfo){
        if(colliderInfo.tag == "Player"){
            speedUpValue += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, colliderInfo.transform.position, speedUpValue * speedTowardsPlayer);
        }
    }
}