using UnityEngine;

public class TargetFramerate : MonoBehaviour {
	public int targetFrameRate = 30;

	void Start() {
#if !UNITY_EDITOR
		QualitySettings.SetQualityLevel(GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().qualitySetting);

		Application.targetFrameRate = targetFrameRate;
#else
		QualitySettings.SetQualityLevel(2);
#endif
	}
}