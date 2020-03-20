using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour {

	public static Dictionary<string, int> configs = new Dictionary<string, int>();
	public static int currentLuckyCatId = -1;
	public static Vector2 guiScale = Vector3.one;
	public static int guiX = 0;
	public static int guiY = 0;
	public static ImServices services = null;
	public static float time = 0f;

	[System.Serializable]
	public struct Command {

		public string name;
		public int size;
		public int defaultValue;
	}

	public Command[] commands = null;

	private bool gameStart = false;
	private Texture2D guiBackdrop = null;
	private Texture2D guiBackdropTurn = null;

	private void Awake() {
		Application.targetFrameRate = 30;
		DontDestroyOnLoad(gameObject);
		guiBackdrop = MakeIcon(0, 0, 0, 63);
		guiBackdropTurn = MakeIcon(0, 0, 0, 127);
#if UNITY_EDITOR
		guiScale = new Vector2(2f, 2f);
		guiX = Screen.width >> 1;
		guiY = Screen.height >> 1;
#else
		guiScale = new Vector2(4f, 4f);
		guiX = Screen.width >> 2;
		guiY = Screen.height >> 2;
#endif
	}

	private bool Click(string text, bool turn, bool expand = false) {
		string textToFill = expand ? string.Concat(" ", text) : string.Concat(" ", text, " ");
		GUILayout.Label(textToFill, GUILayout.ExpandWidth(expand));
		Rect lastRect = GUILayoutUtility.GetLastRect();
		GUI.DrawTexture(lastRect, turn ? guiBackdropTurn : guiBackdrop, ScaleMode.StretchToFill);
		GUI.Label(lastRect, textToFill);
		return GUI.Button(lastRect, string.Empty, GUIStyle.none);
	}

	private Texture2D MakeIcon(byte r, byte g, byte b, byte a) {
		Color32[] colors = new Color32[16];
		Color32 color = new Color32(r, g, b, a);
		for(int i = 0; i < colors.Length; ++i) {
			colors[i] = color;
		}
		Texture2D texture = new Texture2D(4, 4, TextureFormat.RGBA32, false, false);
		texture.SetPixels32(colors);
		texture.Apply(false, true);
		return texture;
	}

	private void OnGUI() {
		GUIUtility.ScaleAroundPivot(GM.guiScale, Vector2.zero);
		GUILayout.BeginArea(new Rect(0f, 0f, GM.guiX, GM.guiY));
		for(int i = 0; i < commands.Length; ++i) {
			string name = commands[i].name;
			int size = commands[i].size;
			GUILayout.BeginHorizontal();
			for(int j = 0, k = GM.configs[name]; j < size; ++j) {
				bool expand = j != 0;
				if(Click(expand ? string.Empty : commands[i].name, j < k, expand || size == 1) && gameStart == false) {
					GM.configs[name] = j < k ? (j + 1 < k ? j + 1 : j) : j + 1;
					if(name.Equals("SERVER")) {
						if(GM.configs[name] == 0) {
							GM.configs["地圖"] = 0;
							GM.configs["地圖人數"] = 0;
							GM.configs["地圖時間"] = 0;
							GM.configs["貓咪"] = 0;
						} else {
							GM.configs["地圖"] = 0;
							GM.configs["地圖人數"] = 4;
							GM.configs["地圖時間"] = 1;
							GM.configs["貓咪"] = 1;
						}
					}
				}
			}
			GUILayout.EndHorizontal();
		}
		if(Click("遊戲開始", gameStart, true) && gameStart == false) {
			StartCoroutine(StartPlayScene());
		}
		GUILayout.EndArea();
	}

	private IEnumerator Start() {
		for(int i = 0; i < commands.Length; ++i) {
			GM.configs.Add(commands[i].name, commands[i].defaultValue);
		}
		if(Debug.isDebugBuild) {
			yield return SceneManager.LoadSceneAsync(1);
		} else {
			Debug.unityLogger.logEnabled = false;
			GM.services = gameObject.AddComponent<ImServices>();
			gameStart = true;
			enabled = false;
			yield return SceneManager.LoadSceneAsync(2);
		}
	}

	private IEnumerator StartPlayScene() {
		GM.services = gameObject.AddComponent<ImServices>();
		gameStart = true;
		yield return new WaitForSeconds(0.5f);
		enabled = false;
		if(GM.configs["貓咪"] == 0) {
			yield return SceneManager.LoadSceneAsync(2);
		} else {
			yield return SceneManager.LoadSceneAsync(3);
		}
	}
}
