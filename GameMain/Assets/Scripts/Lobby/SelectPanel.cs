using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectPanel : MonoSingleton<SelectPanel> {

    public Canvas canvas { get { return gameObject.GetComponent<Canvas>(); } }

    public int SelectCount = 6;

    public int SkinIndex = 0;

    [SerializeField]
    public GameObject m_BtnRoot = null;
    [SerializeField]
    public GameObject m_BtnSample = null;

    [SerializeField]
    public Button m_OkBtn = null;

    [SerializeField]
    public InputField m_InputName = null;

    [SerializeField]
    public GameObject m_3DModel = null;

    public List<SelectBtn> btnList = new List<SelectBtn>();
    
    // Use this for initialization
    void Start ()
    {
        m_OkBtn.onClick.AddListener(startGame);
        canvas.enabled = false;
        m_3DModel.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Show()
    {
        if (btnList.Count <= 0)
        {
            CreateBtn();
        }

        canvas.enabled = true;

        m_3DModel.SetActive(true);
    }

    void CreateBtn()
    {
        for(int i = 0; i < SelectCount; i++)
        {
            var btn = Instantiate(m_BtnSample, m_BtnRoot.transform);
            btn.SetActive(true);

            var selectBtn = btn.GetComponent<SelectBtn>();
            selectBtn.Init(i + 1);
            selectBtn.AddBtnEvent(onBtnClick);

            btnList.Add(selectBtn);
        }
    }

    void onBtnClick(int _skinIndex)
    {
        changeSkin(_skinIndex);

        AudioPlayer.Instance.playByName("cat_like" + _skinIndex);

        for (int i = 0; i < btnList.Count; i++)
        {
            var btn = btnList[i];
            btn.SetSelectImg(btn.btnIndex == _skinIndex);
        }
    }

    void changeSkin(int _skinIndex)
    {
        SkinIndex = _skinIndex;

        var mesh = m_3DModel.GetComponentInChildren<SkinnedMeshRenderer>();

        //Material mat = mesh.material;

        string path = "Material/Catg00" + SkinIndex + "_A";

        mesh.material = Resources.Load<Material>(path);

    }


    void startGame()
    {
        GM.configs["貓咪"] = SkinIndex;

        StartCoroutine(StartPlayScene());
    }

    IEnumerator StartPlayScene()
    {
        yield return SceneManager.LoadSceneAsync(3);

    }
}
