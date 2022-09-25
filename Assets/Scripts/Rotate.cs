using UnityEngine;

public class Rotate : MonoBehaviour {
    public float speed;
    public bool blackHole = false;
    void Update() {
        if(!blackHole)
            transform.Rotate(0, Time.deltaTime * speed, 0, Space.Self);
        else {
			transform.Rotate(0, Time.deltaTime * speed, 0, Space.World);
		}
	}
}