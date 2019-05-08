/* 
 * written by Ninoslav Kjireski 05/2019
 * parts of this project were provided by Joseph Hocking 2017
 * and the Unity-Chan Asset Package 05/2019 from the Unity Asset Store.
 * Written for DTT as an application test
 * released under MIT license (https://opensource.org/licenses/MIT)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class PlayerMovement : MonoBehaviour
{
    //Camera (switch)
    [SerializeField] private Camera headCam;
    [SerializeField] private Camera mazeCam;
    [SerializeField] Toggle cameraSwitch;

    //Movement
    public float forwardSpeed = 6.0f;
    public float backwardSpeed = 2.0f;
    public float rotateSpeed = 2.0f;
    public float jumpPower = 3.0f;
    public float gravity = -9.8f;

    public float sensitivityHor = 4.0f;
    public float sensitivityVert = 4.0f;

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    private float rotationVert = 0;

    //private CharacterController charController;

    public float animSpeed = 1.5f;              
    public float lookSmoother = 3.0f;           // a smoothing setting for camera motion
    public bool useCurves = true;              
                                                
    public float useCurvesHeight = 0.5f;        

    private CapsuleCollider col;
    private Rigidbody rb;

    private Vector3 velocity;

    private float orgColHight;
    private Vector3 orgVectColCenter;
    private Animator anim;                          
    private AnimatorStateInfo currentBaseState;     

    //private GameObject cameraObject; 
    // Animation
    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int locoState = Animator.StringToHash("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    static int restState = Animator.StringToHash("Base Layer.Rest");

    //GUI - Elements
    public bool boolToogleButton;

    void Start()
    {
        //charController = GetComponent<CharacterController>();
     
        anim = GetComponent<Animator>();
    
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
       
        //cameraObject = GameObject.FindWithTag("MainCamera");
        
        orgColHight = col.height;
        orgVectColCenter = col.center;

        headCam.enabled = true;
        mazeCam.enabled = false;
    }

    void FixedUpdate()
    {
        //Add listener for when the state of the Toggle changes, to switch cam
        cameraSwitch.onValueChanged.AddListener(delegate {
            ToggleValueChanged(cameraSwitch);
        });

        //movement
        float h = Input.GetAxis("Horizontal");              
        float v = Input.GetAxis("Vertical");                
        anim.SetFloat("Speed", v);                          
        anim.SetFloat("Direction", h);                      
        anim.speed = animSpeed;                             
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0); 
        rb.useGravity = true;

        MoveCharacter(h, v);
        RotateCharacter();
        RotateCamera();

        if (currentBaseState.fullPathHash == locoState)
        {
            if (useCurves)
            {
                resetCollider();
            }
        }

        //jump animation
        else if (currentBaseState.fullPathHash == jumpState)
        {
            //cameraObject.SendMessage("setCameraPositionJumpView");  

            if (!anim.IsInTransition(0))
            {

                if (useCurves)
                {
                    float jumpHeight = anim.GetFloat("JumpHeight");
                    float gravityControl = anim.GetFloat("GravityControl");
                    if (gravityControl > 0)
                        rb.useGravity = false;
                   
                    Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.distance > useCurvesHeight)
                        {
                            col.height = orgColHight - jumpHeight;         
                            float adjCenterY = orgVectColCenter.y + jumpHeight;
                            col.center = new Vector3(0, adjCenterY, 0); 
                        }
                        else
                        {				
                            resetCollider();
                        }
                    }
                }			
                anim.SetBool("Jump", false);
            }
        }
        
        //idle animation
        else if (currentBaseState.fullPathHash == idleState)
        {
            if (useCurves)
            {
                resetCollider();
            }
            if (Input.GetButtonDown("Jump"))
            {
                anim.SetBool("Rest", true);
            }
        }

        //rest animation
        else if (currentBaseState.fullPathHash == restState)
        {
            if (!anim.IsInTransition(0))
            {
                anim.SetBool("Rest", false);
            }
        }
    }

    private void ToggleValueChanged(Toggle cameraSwitch)
    {
        headCam.enabled = !headCam.enabled;
        mazeCam.enabled = !mazeCam.enabled;
    }

    void resetCollider()
    {
        col.height = orgColHight;
        col.center = orgVectColCenter;
    }

    private void MoveCharacter(float h, float v)
    {
        velocity = new Vector3(0, 0, v);

        velocity = transform.TransformDirection(velocity);

        if (v > 0.1)
        {
            velocity *= forwardSpeed;
        }
        else if (v < -0.1)
        {
            velocity *= backwardSpeed;
        }

        if (Input.GetButtonDown("Jump"))
        {

            if (currentBaseState.fullPathHash == locoState)
            {

                if (!anim.IsInTransition(0))
                {
                    rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                    anim.SetBool("Jump", true);
                }
            }
        }


        transform.localPosition += velocity * Time.fixedDeltaTime;

        transform.Rotate(0, h * rotateSpeed, 0);
    }

    private void RotateCharacter()
    {
        if(headCam.isActiveAndEnabled)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
        }
    }

    private void RotateCamera()
    {
        if(headCam.isActiveAndEnabled)
        {
            rotationVert -= Input.GetAxis("Mouse Y") * sensitivityVert;
            rotationVert = Mathf.Clamp(rotationVert, minimumVert, maximumVert);

            headCam.transform.localEulerAngles = new Vector3(
                rotationVert, headCam.transform.localEulerAngles.y, 0
            );
        }
    }
}
