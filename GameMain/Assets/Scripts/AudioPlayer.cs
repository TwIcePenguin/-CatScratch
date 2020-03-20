using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoSingleton<AudioPlayer> {

    public AudioSource m_Audio = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    AudioPlayer.Instance.playByName("attack");
        //    AudioPlayer.Instance.playByName("cry");
        //    AudioPlayer.Instance.playByName("cat_beCatch");
        //    AudioPlayer.Instance.playByName("attack");
        //    AudioPlayer.Instance.playByName("catrush");
        //    AudioPlayer.Instance.playByName("fail");
        //    AudioPlayer.Instance.playByName("fly");
        //    AudioPlayer.Instance.playByName("victory");

        //}

        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //   AudioPlayer.Instance.playByName("cry");
        //}
    }

    public void playByName(string name)
    {
        var path = "music/" + name;

        var clip = Resources.Load<AudioClip>(path);

        m_Audio.PlayOneShot(clip);
    }

}
