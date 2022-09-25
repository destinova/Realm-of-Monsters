using UnityEngine;

public class BopUpDown : MonoBehaviour {
    public float height, speed;

    private float x, z, w;
    private void Start() {
        x = transform.position.x;
        z = transform.position.z;
        w = transform.position.y;
    }

    void Update() {
        float y = w + height * Mathf.Sin(Time.time * speed);
		transform.position = new Vector3(x, y, z);
	}
}