using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private GameObject m_CatObj = null;
    public GameObject CatObj { get { return m_CatObj; } }

    [SerializeField]
    private GameObject m_GhostObj = null;
    public GameObject GhostObj { get { return m_GhostObj; } }

    private CatAnimatorController mCatAni = null;
    private GhostAnimatorController mGhostAni = null;

    private AniControllerBase mAnimatorController = null;

    void Awake()
    {
        //QA.Add<ClientSyncEvent>("1", ServerEvent);
        //Init();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (Input.GetKey(KeyCode.Alpha1))
        //{
        //    Move();
        //}

        //if (Input.GetKey(KeyCode.Alpha2))
        //{
        //    Turn();
        //}

        //if (Input.GetKey(KeyCode.Alpha3))
        //{
        //    Attack();
        //}

        //if(Input.GetKey(KeyCode.Alpha4))
        //{
        //    ChangeModel(PlayerControl.PlayerType.Ghost);
        //}
    }

    //void ServerEvent(ClientSyncEvent _event)
    //{
        
    //}

    public void Init()
    {
        m_CatObj.AddComponent<TurnCat>();
        m_GhostObj.AddComponent<TurnCat>();

        mCatAni = m_CatObj.GetComponent<CatAnimatorController>();
        mGhostAni = m_GhostObj.GetComponent<GhostAnimatorController>();

        mAnimatorController = mCatAni;
    }

    public void Move()
    {
        mAnimatorController.Trigger_Move();
    }

    public void Turn()
    {
        mAnimatorController.Trigger_Turn();
    }

    public void Attack()
    {
        mAnimatorController.Trigger_Attack();
    }

    public void Win()
    {
        mAnimatorController.Trigger_Win();
    }

    public void ChangeModel(PlayerControl.PlayerType _type)
    {
        Debug.Log("name=" + gameObject.name + "  type=" + _type.ToString());
        if(_type == PlayerControl.PlayerType.Cat)
        {
            m_CatObj.SetActive(true);
            m_GhostObj.SetActive(false);
            mAnimatorController = mCatAni;
        }
        else
        {
            m_CatObj.SetActive(false);
            m_GhostObj.SetActive(true);
            mAnimatorController = mGhostAni;
        }
    }
}
