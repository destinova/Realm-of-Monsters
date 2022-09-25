using UnityEngine;

public class PlaySound : MonoBehaviour {
    private GameManager manager;
    void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		if(manager.sfx == 1) {
			GetComponent<AudioSource>().Play();
		}
	}
}