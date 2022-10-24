using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput pi;
    public float walkSpeed = 2.4f;
    public float runMultiplier = 2.0f;
    public float jumpVelocity = 5.0f;
    public float rollVelocity = 1.0f;

    [Space (10)]
    [Header("===== Friction Settings =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    private bool canAttack;
    private bool lockPlaner = false;
    private CapsuleCollider col;
   

    void Awake()
    {
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update() // Time.deltaTime 1/60
    {
        //print (pi.Dup);
        anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), ((pi.run) ? 2.0f : 1.0f), 0.5f));

        anim.SetBool("defense", pi.defense);

        if (rigid.velocity.magnitude > 1.0f && pi.Roll)
        {
            anim.SetTrigger("roll");
        }

        if (pi.jump)
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }
        
        if (pi.Roll)
        {
            anim.SetTrigger("roll");
            canAttack = false;
        }

        if (pi.attack && CheckState("ground") && canAttack)
        {
            anim.SetTrigger("attack");
        }

        if (pi.Dmag > 0.1f)
        {
            model.transform.forward = Vector3.Slerp(model.transform.forward, pi.DVec, 0.3f); // !!£¡
        }
        
        if (lockPlaner == false)
        {
            planarVec = pi.Dmag * model.transform.forward * walkSpeed * ((pi.run)? runMultiplier : 1.0f);
        }

        //print(CheckState("idle","attack"));
    }

    void FixedUpdate() // Time.fixedDeltaTime 1/50
    {
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        col.center += anim.deltaPosition;
        thrustVec = Vector3.zero;
    }
    
    private bool CheckState(string stateName,string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }


    ///
    ///Message processing  block
    ///
    public void OnJumpEnter()
    {
        pi.inputEnabled = false;
        lockPlaner = true;
        thrustVec = new Vector3(0,jumpVelocity, 0);
    }

    public void IsGround()
    {
        anim.SetBool("isGround", true);
    } 

    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }

    public void OnGroundEnter()
    {
        pi.inputEnabled = true;
        lockPlaner = false;
        canAttack = true;
        col.material = frictionOne;
    }

    public void OnGroundExit()
    {
        col.material = frictionZero;
    }

    public void OnFallEnter()
    {
        pi.inputEnabled = false;
        lockPlaner = true;
    }

    public void OnRollEnter()
    {
        pi.inputEnabled = false;
        lockPlaner = true;
        thrustVec = new Vector3 (0, rollVelocity, 0);
    }

    public void OnAttack1hAEnter()
    {
        pi.inputEnabled = false;
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), 1.0f);
        anim.applyRootMotion = true;
    }

    public void OnAttack1hAUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAveloci");
    }

    public void OnAttackIdle()
    {
        pi.inputEnabled = true;
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), 0);
        anim.applyRootMotion = false;
    }
}
