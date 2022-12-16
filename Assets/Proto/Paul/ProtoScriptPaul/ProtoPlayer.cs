using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProtoPlayer : MonoBehaviour
{
    Rigidbody2D rb;
    ProtoGameManager protoGameManager;
    ScrollingBackGround scrollingBackGround;
    AudioManager _audioManager;

    #region Jump

    //------JUMP-----//
    /*[Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpTimeCounter;
    [SerializeField] bool isJumping;
    [SerializeField] float jumpTime;*/



    // A Gaetan


    [SerializeField] float jumpCut;
    [SerializeField] float jumpForce;

    bool IsJumping = false;

    float LastPressedJumpTime;
    [SerializeField] float bufferTime;


    [SerializeField] Transform groundCheck;
    [SerializeField] Vector2 groundCheckRadius;
    [SerializeField] LayerMask groundLayer;
    float LastOnGroundTime;
    bool isGrounded;
    [SerializeField] float coyoteTime;

    [SerializeField] float gravityScale;
    [SerializeField] float fallGravityMult;



    #endregion

    #region GROUND

    //------GROUND-----//
   /* [Header("Ground")]
    [SerializeField] Transform groundCheckCollider;
    float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool isGrounded;*/
    #endregion

    #region Etat

    //------isEvil-----//
    [Header("Etat")]
    [SerializeField] int life;

    //Tester de retirer le public.
    public bool isEvil = false;
    #endregion

    #region Switch
    ScrollingBackGround[] scroll;
    [SerializeField] proto_UI proto_ui;
    #endregion




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();

        protoGameManager = FindObjectOfType<ProtoGameManager>();

        scroll = FindObjectsOfType<ScrollingBackGround>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void FixedUpdate()
    {
        //GroundCheck();
    }

    // Update is called once per frame
    void Update()
    {
        //Jump
        /*if (isJumping == true)
        {
            if (jumpTime > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                //rb.AddForce(Vector2.up * jumpForce);
                jumpTime -= Time.deltaTime;
            }
            else
            {
                isJumping = false;

                Debug.Log("Fin de saut");
            }
        }*/





        CanJump();

        if (rb.velocity.y >= 0)
            SetGravityScale(gravityScale);
        else
            SetGravityScale(gravityScale * fallGravityMult);



        if (Physics2D.OverlapBox(groundCheck.position, groundCheckRadius, 0, groundLayer))//checks if set box overlaps with ground
        {
            isGrounded = true;
            LastOnGroundTime = coyoteTime;
        }
        else
            isGrounded = false;



        if (IsJumping && isGrounded)
        {
            IsJumping = false;
            GetComponent<Animator>().SetBool("GentilleJump", false);
        }

        if (CanJump() && LastPressedJumpTime > 0)
        {

            IsJumping = true;
            GetComponent<Animator>().SetBool("GentilleJump", true);
            JumpAcction();
        }





        //Interaction
    }
    #region JUMP
    //Jump Input
    public void Jump(InputAction.CallbackContext ctx)
    {
        /* if (!ctx.performed)
         {
             isJumping = false; return;
         }

         if (ctx.performed && isGrounded && !isEvil && isJumping == false)
         {
             isJumping = true;

             jumpTime = jumpTimeCounter;

             Debug.Log("Jump Perform !");
         }*/






        if (ctx.started && !IsJumping && isGrounded)
        {
            LastPressedJumpTime = bufferTime;

        }

        if (ctx.canceled)
        {
            JupCut();
        }



    }





  





    public void JumpAcction()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        float force = jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }


    public void JupCut()
    {
        rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCut), ForceMode2D.Impulse);
    }


    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;


    }

    private bool CanJumpCut()
    {
        return IsJumping && rb.velocity.y > 0;
    }


    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }


    //Pour v�rifier si il touche le sol.
    /* void GroundCheck()
     {
         isGrounded = false;

         Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);

         if (colliders.Length > 0)
         {
             isGrounded = true;

             Debug.Log("isGrounded");
         }
     }*/
    #endregion









    public void Switch()
    {
        isEvil = !isEvil;

        if (isEvil)
        {
            GetComponentInChildren<Animator>().SetTrigger("GoodVersEvil");
            _audioManager.Play("SwitchEvil");
        }
        else
        {
            GetComponentInChildren<Animator>().SetTrigger("EvilVersGood");
            _audioManager.Play("SwitchGood");
        }

        proto_ui.ChangeUI(isEvil);

        foreach (ScrollingBackGround listscroll in scroll)
        {
            listscroll.ChangeBackGround(isEvil);
        }



        //Changement d'animation
        GetComponentInParent<Animator>().SetBool("isEvil", isEvil);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            //protoGameManager.GetInfo(collision.transform, 1f /*Changer la valeur � la main*/);

            collision.GetComponent<proto_CheckPoint>().SendData();

            Debug.Log("New Checkpoint");
        }

        if (collision.CompareTag("Enemy"))
        {
            life -= 1;

            if (life == 0)
            {
                protoGameManager.OnDeath();

                life = 3;
            }

            Debug.Log("Enemy contact");
        }

        if (collision.CompareTag("Trou"))
        {
            protoGameManager.Respawn();

            Debug.Log("Perso tomb�");
        }

        if (collision.CompareTag("Pi�ce"))
        {
            //
            collision.GetComponent<Pi�ce>().activeObject();

            Debug.Log("Pi�ce ramass�");
        }

        if (collision.CompareTag("Switch"))
        {
            Switch();

            //Debug.Log("Switch");
        }
    }


   
}