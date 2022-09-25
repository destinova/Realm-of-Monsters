using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CreatureInteraction : MonoBehaviour {
	public float rotationSpeed = 100f;

	private AudioSource monsterCry;
	private GameManager manager;

	private void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		monsterCry = GetComponent<AudioSource>();
	}

	private void Update() {
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit;

			if(Physics.Raycast(ray, out raycastHit, 25f) && raycastHit.collider.gameObject.tag.Equals("Monster")) {
				raycastHit.collider.gameObject.GetComponent<Animator>().SetTrigger("Special");
				Cry();
			}
		}
	}

	private void Cry() {
		if(manager.sfx == 1) {
			monsterCry.Play();
		}
	}

	private void OnMouseDrag() {
		if(enabled) {
			float x = Input.GetAxis("Mouse X") * rotationSpeed * Mathf.Deg2Rad;

			transform.Rotate(Vector3.up, -x);
		}
    }
}