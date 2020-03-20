using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimatorController : AniControllerBase
{

    public override void Trigger_Move()
    {
        mAnimator.SetBool("Bool_Attack", false);
    }

    public override void Trigger_Turn()
    {
        mAnimator.SetBool("Bool_Attack", false);
        //mAnimator.SetTrigger("Trigger_Turn");
    }

    public override void Trigger_Attack()
    {
        mAnimator.SetBool("Bool_Attack", true);
    }
}
