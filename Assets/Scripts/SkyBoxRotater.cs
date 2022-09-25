using UnityEngine;

public class SkyBoxRotater : MonoBehaviour {
	public float rotationSpeed;

    void Update() {
		RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
	}
}