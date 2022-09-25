using System.Collections;
using UnityEngine;
using TMPro;

public static class Extensions {
	public static string KFormat(this int num) {
		if(num >= 100000000)
			return (num / 1000000).ToString("#,0M");

		if(num >= 1000000)
			return (num / 1000000D).ToString("0.#") + "M";

		if(num >= 100000)
			return (num / 1000).ToString("#,0K");

		if(num >= 10000)
			return (num / 1000D).ToString("0.#") + "K";

		return num.ToString("#,0");
	}

	/// <param name="message">Message string to show in the toast.</param>
	public static void ShowAndroidToastMessage(string message) {
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

		if(unityActivity != null) {
			AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
			unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
				AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
				toastObject.Call("show");
			}));
		}
	}

	/// <summary>
	/// Plays a click sound if SFX is enabled in settings
	/// </summary>
	/// <param name="manager">The Game Manager in the scene</param>
	/// <param name="source">The source of the click sound</param>
	public static void Click(GameManager manager, AudioSource source) {
		if(manager.sfx == 1) {
			source.Play();
		}
	}
}

public enum Type {
	damage,
	tank,
	healer
}

[System.Serializable]
public class Mob {
	public int floor;
	public int wave;
	public int level;
	public string name;
}

[System.Serializable]
public class Mobs {
	public Mob[] mob;
}

[System.Serializable]
public class Boss {
	public int floor;
	public int level;
	public int scale;
	public float position;
	public float fontsize;
	public float y;
	public float z;
	public string name;
}

[System.Serializable]
public class Bosses {
	public Boss[] boss;
}

[System.Serializable]
public class Rewards {
	public int floor;
	public int coins;
	public float magic;
	public int xp;
}

[System.Serializable]
public class RewardsList {
	public Rewards[] rewards;
}