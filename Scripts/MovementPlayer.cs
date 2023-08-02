using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class MovementPlayer : MonoBehaviour
{
    public GameOver GameOver;
    public YouWin YouWin;
    public CharacterController characterController;

    // move, turn variables
    public float speed = 6;
    public float runSpeed = 12;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public bool isDead = false;
    bool insensitive = false;

    public Transform mainCam;

    public Animator playerAnim;

    // ground variables
    [SerializeField] private float gravity;
    private Vector3 velocity;
    [SerializeField] private float jumpHeight;

    private Vector3 givenDirectionRaw = Vector3.zero;
    private bool isOnGround;

    Rigidbody rb;
    Vector3 autoSavePosition;
    bool positionAdjusted;
    private int health;
    GameObject healthComponent;

    float currentBoost;
    public BoostBar boostBar;
    public float boostRechargeSpeed;

    public GameObject centerPoint;
    public float attackRange;
    bool attacking;

    public Renderer foxRenderer;

    IEnumerator Start()
    {
        rb = GetComponent<Rigidbody>();
        positionAdjusted = true;
        currentBoost = 100;
        autoSavePosition = transform.position;
        CountTimer.instance.HideTimer();
        GameOver.instance.Hide();
        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSeconds(0.3f);
        FindObjectOfType<AudioManager>().Stop("MenuMusic");
    }

    void Update()
    {
        ApplyDownForce();
        GetPlayerInput();
        PlayerMovement();
        PlayerJump();
        ApplyGravity();
        UpdateBoostBar();
        Attacking();
    }

    void FixedUpdate(){
        AdjustGetUpPosition();
    }


    void OnTriggerExit(Collider colliderInfo) {
        isOnGround = false;
        if(colliderInfo.tag == "Save"){
            CountTimer.instance.HideTimer();
        }
    }

    // check Ground and other colliders
    void OnTriggerStay(Collider colliderInfo){
        if(colliderInfo.tag != "Player"){
            switch(colliderInfo.tag){
                case "Ground":
                    isOnGround = true;
                    break;
                case "Save":
                    if(Input.GetButton("Crouch") && !insensitive){
                        if(colliderInfo.transform.position == autoSavePosition){
                            CountTimer.instance.AlreadySaved();
                        }
                        else{
                            CountTimer.instance.StartTimer();
                            if(CountTimer.instance.countTime <= 0.1f){
                                autoSavePosition = colliderInfo.transform.position;
                                CountTimer.instance.position = true;
                                FindObjectOfType<AudioManager>().Play("SavePosition");
                            }
                        }
                    } else if (!Input.GetButton("Crouch")){
                        CountTimer.instance.StopTimer();
                    }
                    break;
                case "Point":
                    //GameObject mySelf = colliderInfo.gameObject.transform.parent.gameObject;
                    colliderInfo.gameObject.transform.parent.gameObject.SetActive(false);
                    PointSystem.instance.AddPoint(1);
                    FindObjectOfType<AudioManager>().Play("CoinPickUp");
                    break;
                case "Enemy":
                    Die();
                    break;
                case "Finish":
                    if (Input.GetButton("Crouch"))
                    {
                        CountTimer.instance.StartTimer();
                        if (CountTimer.instance.countTime <= 0.1f)
                        {
                            YouWin.Setup();
                            isDead = true;
                        }
                    }
                    else if (!Input.GetButton("Crouch"))
                    {
                        CountTimer.instance.StopTimer();
                    }
                    break;
            }
        }
    }  

    void ApplyDownForce(){
        if (isOnGround && velocity.y < 0){
            velocity.y = -2f;
        }
    }

    void GetPlayerInput(){
        givenDirectionRaw = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
    }

    void PlayerMovement(){
        // Direction and orientation
        if(givenDirectionRaw.magnitude >= 0.1f && !isDead || givenDirectionRaw.magnitude >= 0.1f && insensitive)
        { // at speed grater than 0.1
        
            playerAnim.SetBool("Sit", false);
            // turn Player to certain direction
            float targetAngle = Mathf.Atan2(givenDirectionRaw.x, givenDirectionRaw.z) * Mathf.Rad2Deg + mainCam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // move Player in certain direction
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if(Input.GetButton("Crouch")){
                // slow movement
                characterController.Move(moveDir.normalized * 0.15f * speed * Time.deltaTime);

                playerAnim.SetBool("RunBool", isOnGround?false:true);
                FindObjectOfType<AudioManager>().Stop("PlayerWalk");
                FindObjectOfType<AudioManager>().Stop("PlayerRun");
            }else if(!Input.GetButton("Run") || !isOnGround || currentBoost <= 1){
                // Walk movement
                characterController.Move(moveDir.normalized * speed * Time.deltaTime);
                
                playerAnim.SetBool("RunBool", isOnGround?false:true);
                FindObjectOfType<AudioManager>().Play("PlayerWalk");
                FindObjectOfType<AudioManager>().Stop("PlayerRun");
            } else {
                // Run movement
                if(isOnGround) characterController.Move(moveDir.normalized * runSpeed * Time.deltaTime);
                
                playerAnim.SetBool("RunBool", isOnGround?true:false);
                FindObjectOfType<AudioManager>().Stop("PlayerWalk");
                FindObjectOfType<AudioManager>().Play("PlayerRun");
            } 

            // play Run animation
            if(!Input.GetButton("Crouch"))
                playerAnim.SetFloat("Forward", Mathf.Max(givenDirectionRaw.x, givenDirectionRaw.z, -givenDirectionRaw.x, -givenDirectionRaw.z, turnSmoothTime, Time.deltaTime));
            else 
                playerAnim.SetFloat("Forward", Mathf.Max(givenDirectionRaw.x, givenDirectionRaw.z, -givenDirectionRaw.x, -givenDirectionRaw.z)*0.4f, turnSmoothTime, Time.deltaTime);
        } else if (givenDirectionRaw.magnitude < 0.1f && !isDead && Input.GetButton("Crouch")){
            playerAnim.SetBool("Sit", true);
            playerAnim.SetFloat("Forward", 0, turnSmoothTime, Time.deltaTime);
            playerAnim.SetBool("RunBool", false);
            
            FindObjectOfType<AudioManager>().Stop("PlayerWalk");
            FindObjectOfType<AudioManager>().Stop("PlayerRun");
        } else if (givenDirectionRaw.magnitude < 0.1f && !isDead && !Input.GetButton("Crouch")){
            playerAnim.SetBool("Sit", false);
            playerAnim.SetFloat("Forward", 0, turnSmoothTime, Time.deltaTime);
            playerAnim.SetBool("RunBool", false);
            
            FindObjectOfType<AudioManager>().Stop("PlayerWalk");
            FindObjectOfType<AudioManager>().Stop("PlayerRun");
        } else{
            // stop Run animation
            playerAnim.SetFloat("Forward", 0, turnSmoothTime, Time.deltaTime);
            playerAnim.SetBool("RunBool", false);
            
            FindObjectOfType<AudioManager>().Stop("PlayerWalk");
            FindObjectOfType<AudioManager>().Stop("PlayerRun");
        }

        // Attack
        if (isOnGround && !isDead && !Input.GetButton("Run") && Input.GetButton("Fire1") && !Input.GetButton("Crouch"))
        {
            Attack();
        }
    }

    void PlayerJump(){
        if (isOnGround){
            // stop flying animation
            playerAnim.SetBool("Flying", false);

            // Jump
            if (Input.GetButtonDown("Jump"))
            {
                // get direction
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                playerAnim.SetBool("Flying", true);
                playerAnim.SetBool("RunBool", false);
                
                FindObjectOfType<AudioManager>().Play("Jump");
            }
        } else {
            playerAnim.SetBool("Flying", true);
            playerAnim.SetBool("RunBool", false);
        }
    }

    void ApplyGravity(){
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    async void Die() 
    {
        if (isDead == false && !insensitive)
        {
            isDead = true;
            // throw into air
            if (velocity.y > 0)
            {
                velocity.y = -2;
            }
            if (isOnGround)
            {
                velocity.y = 2;
            }
            
            FindObjectOfType<AudioManager>().Play("PlayerDie");
            playerAnim.SetTrigger("Die");
            await Task.Delay(2000);
            
            if (Health.instance.ReduceHealth() < 1)
            {
                GameOver.instance.Setup();
            }
            
            UseLastSave();
            playerAnim.SetTrigger("GetUp");
            velocity.y = 2;
        }
    }

    void UseLastSave(){
        transform.position = autoSavePosition;
        positionAdjusted = false;
    }

    void AdjustGetUpPosition(){
        // re-adjust player position (for some reason, player sometimes change position for 1 frame, then going back to place where he died)
        if (isDead && !positionAdjusted) {
            if(transform.position != autoSavePosition){
                transform.position = autoSavePosition;
            } else {
                positionAdjusted = true;
                isDead = false;
                InsensitivityAfterDie();
            }
        }
    }

    async void InsensitivityAfterDie()
    {
        insensitive = true;
        var initialColor = foxRenderer.material.color;
        foxRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);
        await Task.Delay(500);
        foxRenderer.material.color = initialColor;
        await Task.Delay(500);
        foxRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);
        await Task.Delay(500);
        foxRenderer.material.color = initialColor;
        await Task.Delay(500);
        foxRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);
        await Task.Delay(500);
        foxRenderer.material.color = initialColor;
        await Task.Delay(500);
        CountTimer.instance.StartTitle();
        insensitive = false;
        await Task.Delay(1000);
        CountTimer.instance.HideTimer();
    }

    void UpdateBoostBar(){
        // Add boost in time
        if(currentBoost >= 0 && currentBoost <= 100) currentBoost += Time.deltaTime * boostRechargeSpeed;
        
        // Drain boost on Run click
        if(Input.GetButton("Run") && givenDirectionRaw.magnitude >= 0.1 && !isDead) currentBoost -= Time.deltaTime * 30;
        
        // Calculate minium boost 
        if(currentBoost<=0) currentBoost = 0;
        
        // Set boostBar UI value
        boostBar.SetBoost(currentBoost);
    }

    void Attack()
    {
        playerAnim.SetTrigger("Attack");
        attacking = true;
        FindObjectOfType<AudioManager>().Play("PlayerHit");

    }
    void Attacking()
    {
        RaycastHit hit;
        var rightEyePoint = centerPoint.transform.position + centerPoint.transform.right;
        var leftEyePoint = centerPoint.transform.position - centerPoint.transform.right;

        // Raycast forward, to freeze blue enemy
        if (attacking)
        {
            if (Physics.Raycast(centerPoint.transform.position, transform.TransformDirection(Vector3.forward), out hit, attackRange))
            {
                AttackingFreeze(hit);
            }
            if (Physics.Raycast(rightEyePoint, transform.TransformDirection(Vector3.forward), out hit, attackRange))
            {
                AttackingFreeze(hit);
            }
            if (Physics.Raycast(leftEyePoint, transform.TransformDirection(Vector3.forward), out hit, attackRange))
            {
                AttackingFreeze(hit);
            }
            attacking = false;
        }
        Debug.DrawRay(centerPoint.transform.position, transform.TransformDirection(Vector3.forward) * attackRange, Color.magenta);
        Debug.DrawRay(leftEyePoint, transform.TransformDirection(Vector3.forward) * attackRange, Color.magenta);
        Debug.DrawRay(rightEyePoint, transform.TransformDirection(Vector3.forward) * attackRange, Color.magenta);
    }

    void AttackingFreeze(RaycastHit givenHit)
    {
        // Freeze enemy
        if (givenHit.transform.tag == "Enemy")
        {
            Enemy enemyComponent = givenHit.transform.GetComponent<Enemy>();
            enemyComponent.Freeze();
        }
    }
}