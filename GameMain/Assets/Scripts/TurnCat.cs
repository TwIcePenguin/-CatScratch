using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCat : MonoBehaviour {

	public float angle = 45f;

	private Transform transformAnchor = null;
	private Transform transformAnchorSub = null;
	private Transform transformCache = null;
	private Transform transformParentCache = null;

	private void Awake() {
		transformAnchor = new GameObject("Anchor").transform;
		transformAnchorSub = new GameObject("AnchorSub").transform;
		transformAnchorSub.SetParent(transformAnchor, false);
		transformCache = transform;
		transformParentCache = transformCache.parent;
	}

	private void OnUpdate() {
		transformAnchor.localRotation = Quaternion.identity;
		transformCache.localPosition = Vector3.zero;
		transformCache.localRotation = transformParentCache.rotation;
		transformCache.SetParent(transformAnchor, false);
		transformAnchor.localRotation = Quaternion.Euler(angle, 0f, 0f);
		transformCache.SetParent(transformParentCache, true);
		transformCache.localPosition = Vector3.zero;
		transformCache.localScale = Vector3.one;
	}

	private void LateUpdate() {
		if(transformCache.hasChanged) {
			OnUpdate();
			transformCache.hasChanged = false;
		}
	}
}
