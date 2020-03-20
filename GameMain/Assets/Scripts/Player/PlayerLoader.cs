using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoSingleton<PlayerLoader> {

    [SerializeField]
    private GameObject m_PlayerSample = null;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    CreatePlayer(1);
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    CreatePlayer(2);
        //}

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    ClearAllPlayer();
        //}
    }

    public GameObject CreatePlayer(int _index)
    {
        GameObject playerGO = Instantiate(m_PlayerSample);
        playerGO.name = "player_" + _index;
        Player player = playerGO.GetComponent<Player>();
        player.Init();

        SetMaterial(PlayerControl.PlayerType.Cat, _index, player.CatObj);
        SetMaterial(PlayerControl.PlayerType.Ghost, _index, player.GhostObj);

        playerGO.SetActive(true);

        return playerGO;
    }

    public void SetMaterial(PlayerControl.PlayerType type, int _index, GameObject obj)
    {
        var mesh = obj.GetComponentInChildren<SkinnedMeshRenderer>();

        if (mesh == null)
            return;

        string path = "";
        if (type == PlayerControl.PlayerType.Ghost)
        {
            path = "Material/Catg00" + _index + "_A";
        }
        else
        {
            path = "Material/Cat00" + _index + "_A";
        }

        mesh.material = Resources.Load<Material>(path);

    }

    
}
