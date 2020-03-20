using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniControllerBase : MonoBehaviour {

    protected Animator mAnimator = null;

    void Awake()
    {
        mAnimator = GetComponentInChildren<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Trigger_Move()
    {
    }

    public virtual void Trigger_Turn()
    {
    }

    public virtual void Trigger_Attack()
    {
    }

    public virtual void Trigger_Win()
    {
    }
}
