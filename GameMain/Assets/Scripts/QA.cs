using System;
using System.Collections.Generic;

public static class QA {

	public delegate void Message();
	public delegate void Message<T>(T param);

	private static Dictionary<string, Delegate>[] msgs = new Dictionary<string, Delegate>[] {
		new Dictionary<string, Delegate>(),
		new Dictionary<string, Delegate>()
	};
#if UNITY_EDITOR

	private static Dictionary<string, Type> recTypes = new Dictionary<string, Type>();
#endif

	public static void Add(string key, Message msg) {
		Delegate val;
		if(msgs[0].TryGetValue(key, out val)) {
			msgs[0][key] = (val as Message) + msg;
		} else {
			msgs[0].Add(key, msg);
		}
	}

	public static void Add<T>(string key, Message<T> msg) {
#if UNITY_EDITOR
		Type type;
		if(recTypes.TryGetValue(key, out type)) {
			if( ! type.Equals(typeof(T))) {
				UnityEngine.Debug.LogError(string.Format("{0}<{1}> failed to add type of {2}", key, type, typeof(T)));
				return;
			}
		} else {
			recTypes.Add(key, typeof(T));
		}
#endif
		Delegate val;
		if(msgs[1].TryGetValue(key, out val)) {
			msgs[1][key] = (val as Message<T>) + msg;
		} else {
			msgs[1].Add(key, msg);
		}
	}

	public static bool Invoke(string key) {
		Delegate val;
		if(msgs[0].TryGetValue(key, out val)) {
			(val as Message)();
			return true;
		}
#if UNITY_EDITOR
		else {
			UnityEngine.Debug.LogWarning(string.Format("{0} (Miss)", key));
		}
#endif
		return false;
	}

	public static bool Invoke<T>(string key, T param) {
#if UNITY_EDITOR
		Type type;
		if(recTypes.TryGetValue(key, out type)) {
			if( ! type.Equals(typeof(T))) {
				UnityEngine.Debug.LogError(string.Format("{0}<{1}> failed to invoke type of {2}", key, type, typeof(T)));
				return false;
			}
		}
#endif
		Delegate val;
		if(msgs[1].TryGetValue(key, out val)) {
			(val as Message<T>)(param);
			return true;
		}
#if UNITY_EDITOR
		else {
			UnityEngine.Debug.LogWarning(string.Format("{0}<{1}> (Miss)", key, typeof(T)));
		}
#endif
		return false;
	}

	public static void Remove(string key, Message msg) {
		Delegate val;
		if(msgs[0].TryGetValue(key, out val)) {
			val = (val as Message) - msg;
			if(val != null) {
				msgs[0][key] = val;
			} else {
				msgs[0].Remove(key);
			}
		}
	}

	public static void Remove<T>(string key, Message<T> msg) {
#if UNITY_EDITOR
		Type type;
		if(recTypes.TryGetValue(key, out type)) {
			if( ! type.Equals(typeof(T))) {
				UnityEngine.Debug.LogError(string.Format("{0}<{1}> failed to remove type of {2}", key, type, typeof(T)));
				return;
			}
		} else {
			recTypes.Remove(key);
		}
#endif
		Delegate val;
		if(msgs[1].TryGetValue(key, out val)) {
			val = (val as Message<T>) - msg;
			if(val != null) {
				msgs[1][key] = val;
			} else {
				msgs[1].Remove(key);
			}
		}
	}
}
