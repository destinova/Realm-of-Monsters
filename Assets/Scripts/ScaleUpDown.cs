using UnityEngine;

public class ScaleUpDown : MonoBehaviour {
	public float min = 0.75f, max = 1.25f;

	private void Update() {
		transform.localScale = new Vector3(Mathf.Clamp(Mathf.PingPong(Time.time, max), min, max), Mathf.Clamp(Mathf.PingPong(Time.time, max), min, max), transform.localScale.z);
	}
}