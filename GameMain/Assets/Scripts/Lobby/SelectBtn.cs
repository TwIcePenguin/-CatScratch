using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectBtn : MonoBehaviour {

    [SerializeField]
    Button m_Btn = null;

    [SerializeField]
    Text btnText = null;

    [SerializeField]
    Image btnImage = null;
    [SerializeField]
    Image selectImage = null;

    public int btnIndex = 0;

    // Use this for initialization
    void Start ()
    {
        //m_Btn.onClick.AddListener(onBtnClick);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Init(int index)
    {
        gameObject.name = "btn_" + index;

        btnIndex = index;
        setImage(index);
        //setText(index);

        if (index == 1)
            SetSelectImg(true);
        else
            SetSelectImg(false);
    }

    public void setImage(int index)
    {
        Sprite btnImg = Resources.Load<Sprite>("Texture/select/00" + index);
        Sprite selectImg = Resources.Load<Sprite>("Texture/select/00" + index+ "_S");

        btnImage.sprite = btnImg;
        selectImage.sprite = selectImg;
    }

    public void setText(int index)
    {
        btnText.text = index.ToString();
    }

    public void SetSelectImg(bool value)
    {
        selectImage.enabled = value;
    }

    public void AddBtnEvent(UnityAction<int> fn)
    {
        m_Btn.onClick.AddListener(()=> { fn(btnIndex); });
    }

    void onBtnClick()
    {
        SelectPanel.Instance.SkinIndex = btnIndex;
    }


}
