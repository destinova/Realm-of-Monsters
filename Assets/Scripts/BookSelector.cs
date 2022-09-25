using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookSelector : MonoBehaviour {
	public Creature creaturePrefab;
	public string creatureName;
	public Image creatureImage, typeFrame, typeIcon;
	public List<Sprite> icons;
	public TextMeshProUGUI _name;

	private LibraryScene scene;

	private void Start() {
		scene = GameObject.FindGameObjectWithTag("UI").GetComponent<LibraryScene>();

		SetFrame();
	}

	public void SetFrame() {
		creatureImage.sprite = creaturePrefab.sprite;
		_name.text = creaturePrefab.details.name.ToString();

		switch(creaturePrefab.type) {
			case Type.damage:
			typeFrame.color = new Color(1, 0.1f, 0);
			typeIcon.sprite = icons[0];
			break;
			case Type.tank:
			typeFrame.color = new Color(0, 0.5f, 1);
			typeIcon.sprite = icons[1];
			break;
			case Type.healer:
			typeFrame.color = new Color(0.25f, 1f, 0.5f);
			typeIcon.sprite = icons[2];
			break;
		}
	}

	public void Selected() {
		scene.currentFrame.Unselect();

		creatureImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);


		Destroy(scene.selectedCreature.gameObject);
		//Instantiate(scene.dust);
		scene.selectedCreature = (Instantiate(Resources.Load("Prefabs/" + creatureName, typeof(GameObject)), scene.spawnPoint) as GameObject).GetComponent<Creature>();
		scene.SetCreatureDetails();
		scene.currentFrame = this;
		scene.OnPopUpClose();

		GetComponent<Button>().interactable = false;
	}

	public void Unselect() {
		creatureImage.color = Color.white;

		GetComponent<Button>().interactable = true;
	}
}