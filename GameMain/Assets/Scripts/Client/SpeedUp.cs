using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : MonoBehaviour {

	public static SpeedUp Attach(GameObject gameObject) {
		SpeedUp speedUp = gameObject.GetComponent<SpeedUp>();
		if(speedUp != null) {
			if(speedUp.isActiveAndEnabled) {
				speedUp.value += 10f;
			} else {
				speedUp.enabled = true;
			}
			return speedUp;
		}
		return gameObject.AddComponent<SpeedUp>();;
	}

	public float lifeTime = 0.1f;
	public float value = 10f;

	private float lifeTimeEnd = 0f;

	private Transform transformCache = null;

	private void Awake() {
		transformCache = transform;
	}

	private void OnEnable() {
		lifeTimeEnd = Time.time + lifeTime;

        AudioPlayer.Instance.playByName("catrush");
	}

	private void Update() {
		if(Time.time < lifeTimeEnd) {
			transformCache.Translate(transformCache.forward * (value * Time.deltaTime), Space.World);
		} else {
			enabled = false;
		}
	}
}
