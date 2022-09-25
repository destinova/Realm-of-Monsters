using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FrameSelector : MonoBehaviour {
	public int ID, positionInList;
	public Image creatureImage, typeFrame, typeIcon;
	public List<Sprite> icons;
	public TextMeshProUGUI level, rank;

	[HideInInspector]
	public UpgradeScene scene;

	public void SetFrame() {
		creatureImage.sprite = Resources.Load("Icons/" + scene.manager.creaturesOwned[positionInList].name, typeof(Sprite)) as Sprite;
		level.text = scene.manager.creaturesOwned[positionInList].level.ToString();
		rank.text = scene.manager.creaturesOwned[positionInList].rank.ToString();
		name = scene.manager.creaturesOwned[positionInList].name;

		switch(scene.manager.creaturesOwned[positionInList].type) {
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
		//Debug.Log("Just selected " + name + " " + ID + " in team position: " + positionInList);

		scene.currentFrame.Unselect();

		creatureImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		scene.selected = true;

		Destroy(scene.selectedCreature.gameObject);
		Instantiate(scene.dust);
		scene.selectedCreature = (Instantiate(Resources.Load("Prefabs/" + scene.manager.creaturesOwned[positionInList].name, typeof(GameObject)), scene.spawnPoint) as GameObject).GetComponent<Creature>();
		NewDetails();
		scene.SetCreatureDetails();
		scene.currentFrame = this;
		scene.OnPopUpClose();

		GetComponent<Button>().interactable = false;
	}

	public void Unselect() {
		creatureImage.color = Color.white;

		GetComponent<Button>().interactable = true;
	}

	private void NewDetails() {
		scene.selectedCreature.details.ID = scene.manager.creaturesOwned[positionInList].ID;
		scene.selectedCreature.details.level = scene.manager.creaturesOwned[positionInList].level;
		scene.selectedCreature.details.rank = scene.manager.creaturesOwned[positionInList].rank;
		scene.selectedCreature.details.ranksIncreased = scene.manager.creaturesOwned[positionInList].ranksIncreased;
		scene.selectedCreature.details.xp = scene.manager.creaturesOwned[positionInList].xp;
		scene.selectedCreature.details.canRankUp = scene.manager.creaturesOwned[positionInList].canRankUp;

		scene.selectedCreature.SetDetails();
	}
}