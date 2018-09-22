﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInputController : MonoBehaviour {
    public Camera mainCamera;
    public LeftJoystick leftJoystick; // the game object containing the LeftJoystick script
    public RightJoystick rightJoystick; // the game object containing the RightJoystick script
    public float moveSpeed = 6.0f; // movement speed of the player character
    public float crossHairSpeed = 2f;

    public float distanceRaycast = 10f;
    [SerializeField]
    private Vector2 characterRangeMin;
    [SerializeField]
    private Vector2 characterRangeMax;
  

    private Vector3 leftJoystickInput; // holds the input of the Left Joystick
    private Vector3 rightJoystickInput; // hold the input of the Right Joystick
    private Animator characterAnimator; // the animator controller of the player character
    
    //[Header("Animaciones")]
    static int s_DeadHash = Animator.StringToHash("Dead");
    static int s_ShotHash = Animator.StringToHash("Shot");
    //static int s_RunStartHash = Animator.StringToHash("RunStart");
    static int s_MovingHash = Animator.StringToHash("MovingXY");
    //static int s_JumpingHash = Animator.StringToHash("Jumping");
    //static int s_JumpingSpeedHash = Animator.StringToHash("JumpSpeed");
    //static int s_SlidingHash = Animator.StringToHash("Sliding");

    public Character character;
    public CharacterCollider characterCollider;


    //public float SpeedForward;
    //public float Speed;

    public int maxLife = 3;
    public float laneChangeSpeed = 1.0f;

    public int currentLife { get { return m_CurrentLife; } set { m_CurrentLife = value; } }
    protected bool m_IsInvincible;

    protected int m_CurrentLife;

    // Cheating functions, use for testing
    public void CheatInvincible(bool invincible)
    {
        m_IsInvincible = invincible;
    }

    public bool IsCheatInvincible()
    {
        return m_IsInvincible;
    }


    protected void Awake()
    {
 
        m_CurrentLife = 0;
  
    }

    public void Init()
    {
        //transform.position = k_StartingPosition;
        //m_TargetPosition = Vector3.zero;

        //m_CurrentLane = k_StartingLane;
        //characterCollider.transform.localPosition = Vector3.zero;

        currentLife = maxLife;

        //m_Audio = GetComponent<AudioSource>();

        //m_ObstacleLayer = 1 << LayerMask.NameToLayer("Obstacle");
    }

    // Called at the beginning of a run or rerun
    public void Begin()
    {
        character.animator.SetBool(s_DeadHash, false);

        //characterCollider.Init();

        //m_ActiveConsumables.Clear();
    }

    // Use this for initialization
    void Start () {
        characterAnimator = character.GetComponent<Animator>();
	}

    // Update is called once per frame
    protected void Update() {

        // get input from both joysticks
        leftJoystickInput = leftJoystick.GetInputDirection();
        rightJoystickInput = rightJoystick.GetInputDirection();

        MovingCharacterXY();

        MovingCrosshair();
       

       
    }

    private void MovingCharacterXY() {
        float xMovementLeftJoystick = leftJoystickInput.x; // The horizontal movement from joystick 01
        float yMovementLeftJoystick = leftJoystickInput.y; // The vertical movement from joystick 01	

        // if there is no input on the left joystick
        if (leftJoystickInput == Vector3.zero)
        {
            characterAnimator.SetFloat(s_MovingHash, 0f);
        }

        // if there is only input from the left joystick
        if (leftJoystickInput != Vector3.zero)
        {
            Vector3 accelXY = new Vector3(xMovementLeftJoystick, yMovementLeftJoystick, 0f) * moveSpeed;
            Vector3 velocity = character.transform.localPosition + accelXY * Time.deltaTime;
            Vector3 velFixed = new Vector3(
                Mathf.Clamp(velocity.x, -characterRangeMin.x, characterRangeMax.x),
                 Mathf.Clamp(velocity.y, -characterRangeMin.y, characterRangeMax.y),
                 0f
                );
            character.transform.localPosition = velFixed;
            characterAnimator.SetFloat(s_MovingHash, xMovementLeftJoystick);
            //aniPlayer.SetFloat(horizontalHash, xAxis, 0.1f, animationSpeed * Time.deltaTime);
        }
    }

    private void MovingCrosshair() {
        float xMovementRightJoystick = rightJoystickInput.x; // The horizontal movement from joystick 02
        float yMovementRightJoystick = rightJoystickInput.y; // The vertical movement from joystick 02
        Vector3 jsPosition;
        if (rightJoystickInput != Vector3.zero)
        {
            jsPosition = new Vector3(
                Screen.width / 2 + (xMovementRightJoystick * Screen.width * 0.5f),
                Screen.height / 2 + (yMovementRightJoystick * Screen.height * 0.5f),
                0f);

            characterAnimator.SetBool(s_ShotHash, true);
        }
        else
        {
            jsPosition = new Vector3(
                Screen.width / 2 ,
                Screen.height / 2,
                0f);
            characterAnimator.SetBool(s_ShotHash, false);
        }

        //transformo el vector jposition en screen position
        Vector2 viewPortPosA = mainCamera.ScreenToViewportPoint(jsPosition);
        Vector2 screenPosA = new Vector2(
                ((viewPortPosA.x * UILevels.uiLevelsInstance.rtCrossHairParent.sizeDelta.x) - (UILevels.uiLevelsInstance.rtCrossHairParent.sizeDelta.x * 0.5f)),
                ((viewPortPosA.y * UILevels.uiLevelsInstance.rtCrossHairParent.sizeDelta.y) - (UILevels.uiLevelsInstance.rtCrossHairParent.sizeDelta.y * 0.5f)));

        UILevels.uiLevelsInstance.rtCrossHair.anchoredPosition = Vector2.Lerp(
            UILevels.uiLevelsInstance.rtCrossHair.anchoredPosition, screenPosA,
            Time.deltaTime * crossHairSpeed
            );

        //funciona con hit
        Ray _ray = mainCamera.ScreenPointToRay(jsPosition);
        RaycastHit _rayHit;
        if (Physics.Raycast(_ray, out _rayHit, distanceRaycast, LayerMask.NameToLayer("Player")))
        {
            print("hot");
            if (_rayHit.collider.gameObject.CompareTag("Asteroid") || _rayHit.collider.gameObject.CompareTag("Enemy"))
            {
                //target = _rayHit.collider.transform.position;
                UILevels.uiLevelsInstance.SetCrossHairColor01();
            }
            else
            {
                UILevels.uiLevelsInstance.SetCrossHairColor02();
            }

        }
        else
        {
             UILevels.uiLevelsInstance.SetCrossHairColor00();
        }

    }

    protected virtual void LateUpdate()
    {

        //transform.position = transform.position + (transform.forward * SpeedForward * Time.deltaTime);
        //// This will move the current transform based on a finger drag gesture
        //TouchSystem.MoveObjectInX(transform, TouchSystem.DragDelta, Speed);
    }
}
