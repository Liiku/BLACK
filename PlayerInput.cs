using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("===== Key settings =====")]
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    public string keyA;
    public string keyB;
    public string keyC;
    public string keyD;
    public string keyE;

    public string keyJRight;
    public string keyJLeft;
    public string keyJUp;
    public string keyJDown;

    [Header("===== Output signals =====")]
    public float Dup;
    public float Dright;
    public float Dmag;
    public Vector3 DVec;
    public float Jup;
    public float Jright;

    //1.pressing signal
    public bool run;
    public bool defense;
    
    //2.trigger once signal
    public bool jump;
    public bool Roll;
    public bool attack;
    
    //3.double trigger
    private Timer extTimer = new Timer();
    public bool IsExtending = false;



    [Header("===== Others =====")]
    public bool inputEnabled = true; //Flag

    private float targetDup;
    private float targetDright; 
    private float velocityDup;//ËÙÂÊ
    private float velocityDright;
                  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Jup = (Input.GetKey(keyJUp) ? 1.0f : 0) - (Input.GetKey(keyJDown) ? 1.0f : 0);
        //Jright = (Input.GetKey(keyJRight) ? 1.0f : 0) - (Input.GetKey(keyJLeft) ? 1.0f : 0);
        Jup = Input.GetAxis("Mouse Y") * 3.0f;
        Jright = Input.GetAxis("Mouse X") * 6.5f;

        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0) - (Input.GetKey(keyDown) ? 1.0f : 0);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0) - (Input.GetKey(keyLeft) ? 1.0f : 0);

        if (inputEnabled == false)
        {
            targetDup = 0;
            targetDright = 0;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2* Dright2));
        DVec = Dright * transform.right + Dup * transform.forward;

        //
        
        extTimer.Tick();
        //print(extTimer.state);
        if (extTimer.state == Timer.STATE.RUN)
        {
            IsExtending = true;
        }
        else
        {
            IsExtending = false;
        }
        
        
        run = Input.GetKey(keyA);

        defense = Input.GetKey(keyE);
        
        //jump
        if (Input.GetKeyDown(keyB))
        {
            jump = true;
        }
        else
        {
            jump = false;
        }

        //roll
        if (Input.GetKeyDown(keyC))
        {
            StartTimer(extTimer, 0.25f);
            Roll = true;
            print(IsExtending);
        }
        else
        {
            Roll = false;
        }
       
        //attack
        if (Input.GetKeyDown(keyD))
        {
            attack = true;
        }
        else
        {
            attack = false;
        }
    }


    private Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }

    private void StartTimer(Timer timer,float duration)
    {
        timer.duration = duration;
        timer.Go();
    }

}
