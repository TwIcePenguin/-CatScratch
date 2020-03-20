using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoSingleton<LobbyPanel> {


    public Canvas canvas { get { return gameObject.GetComponent<Canvas>(); } }

    [SerializeField]
    public Button m_startBtn = null;


    void Awake()
    {
        m_startBtn.onClick.AddListener(StartGame);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void StartGame()
    {
        LobbyPanel.Instance.canvas.enabled = false;
        SelectPanel.Instance.Show();

    }

}
