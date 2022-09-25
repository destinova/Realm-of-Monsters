using UnityEngine;

public class ArrowUp : MonoBehaviour {
	public float originY, height, speed;
	public bool top = false;
	
	private float x, z;

	private void Start() {
		x = transform.position.x;
		z = transform.position.z;
	}

	void Update() {
		if(!top) {
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(x, height, z), Time.deltaTime * speed);
			if(transform.position.y == height) {
				top = true;
				Invoke("Stop", 1f);
			}
		}
	}

	private void Stop() {
		transform.position = new Vector3(x, originY, z);
		gameObject.SetActive(false);
	}
}