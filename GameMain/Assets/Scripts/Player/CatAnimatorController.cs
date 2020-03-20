using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimatorController : AniControllerBase
{

 //   private Animator mCatAnimator = null;


 //   void Awake()
 //   {
 //       mCatAnimator = GetComponentInChildren<Animator>();
 //   }

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update ()
 //   {
 //       //if (Input.GetKey(KeyCode.Alpha1))
 //       //{
 //       //    Trigger_Move();
 //       //}

 //       //if (Input.GetKey(KeyCode.Alpha2))
 //       //{
 //       //    Trigger_Turn();
 //       //}
 //   }

    public override void Trigger_Move()
    {
        mAnimator.SetBool("Bool_Move", true);
    }

    public override void Trigger_Turn()
    {
        mAnimator.SetBool("Bool_Move", false);
    }

    public override void Trigger_Win()
    {
        mAnimator.SetBool("Bool_Win", true);
    }
}

