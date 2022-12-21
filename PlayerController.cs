using NTC.Global.Cache;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoCache
{
    public static PlayerController instance;

    [SerializeField]
    private Camera cam;

    [Header("Movement")]
    [SerializeField]
    private float playerSpeed = 5f;

    [SerializeField]
    private float sprintSpeed;

    [SerializeField]
    private float crouchSpeed;

    [SerializeField]
    private float gravity = -9.8f;

    [SerializeField]
    private float sensitivity = 1f;

    [Header("Crouch")]
    [SerializeField]
    private float normalHeight;

    [SerializeField]
    private float crouchHeight;

    [SerializeField]
    private float timeToBeCrouch;

    [SerializeField]
    private LayerMask crouchMask;
    

    [Header("Sounds")]

    [SerializeField]
    private float walkStepTime;

    [SerializeField]
    private float sprintStepTime;

    [SerializeField]
    private float crouchStepTime;

    private float sprintPress;
    private float rotationCam = 0f;
    private float expiredStepClipTime = 0f;
    private bool sprintIsAvailable;
    public bool isMove, isSprint;
    public bool isCrouch = false;

    private CharacterController controller;
    public PlayerInputActions playerInputActions;
    private FootStepParser footStepParser;
    private CameraShake cameraShake;
    private PlayerStats playerStats;
    private PlayerInteract playerInteract;

    private CompositeDisposable disposable = new CompositeDisposable();

    private Vector2 mouse;
    private Vector3 directionWithSpeed
    {
        get 
        {  
            return transform.TransformDirection(moveDirection) * 
                (sprintPress > 0.5 && sprintIsAvailable && !isCrouch ? sprintSpeed : isCrouch ? crouchSpeed : playerSpeed); 
        }
    }


    private Vector3 moveDirection;
    private Vector3 playerVelocity;


    private void Awake ()
    {
        playerInputActions = new PlayerInputActions();
        

        controller = GetCached<CharacterController>();
        footStepParser = GetCached<FootStepParser>();
        cameraShake = GetCached<CameraShake>();
        playerStats = GetCached<PlayerStats>();
        playerInteract = GetCached<PlayerInteract>();

        if (instance == null) instance = this;
        else if (instance == this) Destroy(gameObject);
    }

    protected override void Run()
    {
        Move(playerInputActions.onFoot.Movement.ReadValue<Vector2>());
        Look(playerInputActions.onFoot.Look.ReadValue<Vector2>());
        playerInteract.InteractChecker(playerInputActions.onFoot.Interact.triggered);
        Crouch();
        sprintPress = playerInputActions.onFoot.Sprint.ReadValue<float>();
        cameraShake.MoveShake(isMove, isSprint, isCrouch);
    }

    private void Move (Vector2 inputVector)
    {
        moveDirection = Vector3.zero; 
        moveDirection.x = inputVector.x;
        moveDirection.z = inputVector.y;

        if (moveDirection != Vector3.zero)
        {
            isMove = true;
            footStepParser.CheckGround();

            controller.Move(directionWithSpeed * Time.deltaTime);
            expiredStepClipTime += Time.deltaTime;
            if (sprintPress > 0.5f && sprintIsAvailable && !isCrouch) 
            {
                isSprint = true;
                if (expiredStepClipTime >= sprintStepTime)
                {
                    footStepParser.SprintStep();
                    expiredStepClipTime = 0;
                }
            }
            else 
            {
                isSprint = false;
                if (expiredStepClipTime >= walkStepTime && !isCrouch)
                {
                    footStepParser.WalkStep();
                    expiredStepClipTime = 0;
                } else if (expiredStepClipTime >= crouchStepTime && isCrouch)
                {
                    footStepParser.WalkStep();
                    expiredStepClipTime = 0;
                }
            }
        }
        else
        {
            expiredStepClipTime = ((sprintStepTime - walkStepTime) * sprintPress + walkStepTime) * 0.9f;
            isMove = false;
            isSprint = false;
        }

        playerStats.StaminaChanger(isSprint);


        playerVelocity.y += gravity * Time.deltaTime;
        if (controller.isGrounded && playerVelocity.y < 0f)
        {
            playerVelocity.y = -2f;
        } 
        

        controller.Move(playerVelocity * Time.deltaTime);
        
    }

    private void Look (Vector2 inputVector)
    {
        mouse.x = inputVector.x;

        rotationCam -= (inputVector.y * Time.deltaTime) * sensitivity * 0.9f;

        rotationCam = Mathf.Clamp(rotationCam, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(rotationCam, 0, 0);

        transform.Rotate(Vector3.up * (inputVector.x * Time.deltaTime) * sensitivity);
    }

    private void Crouch ()
    {

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + (normalHeight + crouchHeight) / 2f,
                transform.position.z), Vector3.up * (normalHeight - crouchHeight), Color.green);

        bool canStopCrouch;

        if (playerInputActions.onFoot.Crouch.triggered)
        {
            if (isCrouch)
            {
                canStopCrouch = !Physics.CheckCapsule(new Vector3(transform.position.x, transform.position.y + (normalHeight + crouchHeight) / 2f,
                transform.position.z), new Vector3(transform.position.x, transform.position.y + normalHeight + crouchHeight,
                transform.position.z), 0.55f, crouchMask);
                if (canStopCrouch) isCrouch = false;
            }
            else if (!isCrouch) isCrouch = true;
        }

        if (isCrouch) 
        { 
            if (controller.height > crouchHeight) controller.height -= Time.deltaTime / timeToBeCrouch;
            else controller.height = crouchHeight;
        }
        else if (!isCrouch)
        {
                if (controller.height < normalHeight) controller.height += Time.deltaTime / timeToBeCrouch;
                else controller.height = normalHeight;
        }
    }

    protected override void OnEnabled ()
    {
        playerStats.stamina.Subscribe(staminaValue =>
        {
            if (staminaValue > 1f)
                sprintIsAvailable = true;
            else
                sprintIsAvailable = false;
        }).AddTo(disposable);
        playerInputActions.onFoot.Enable();
    }
    protected override void OnDisabled ()
    {
        playerInputActions.onFoot.Disable();
        disposable.Clear();
    }
}
