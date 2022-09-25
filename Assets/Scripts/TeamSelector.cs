using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSelector : MonoBehaviour {
	public Image creatureImage, typeFrame, typeIcon;
	public List<Sprite> icons;
	public TextMeshProUGUI level, rank;
	public int positionInTeam, ID, positionInList;

	[HideInInspector]
	public bool active = false;
	[HideInInspector]
	public TeamSelection scene;

	public void SetFrame() {
		creatureImage.sprite = Resources.Load("Icons/" + scene.manager.creaturesOwned[ID].name, typeof(Sprite)) as Sprite;
		level.text = scene.manager.creaturesOwned[ID].level.ToString();
		rank.text = scene.manager.creaturesOwned[ID].rank.ToString();

		switch(scene.manager.creaturesOwned[ID].type) {
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

		scene = GameObject.FindGameObjectWithTag("UI").GetComponent<TeamSelection>();

		GetComponent<Button>().onClick.AddListener(delegate {
			Selected();
		});
	}

	public void Selected() {
		scene.Click();
		if(scene.activeMembers < 3 && !active) {
			active = true;
			++scene.activeMembers;

			for(int i = 0; i < 3; i++) {
				if(!scene.positions[i]) {
					positionInTeam = i;
					scene.selectedTeam[positionInTeam] = Instantiate(Resources.Load("Prefabs/" + scene.manager.creaturesOwned[ID].name, typeof(GameObject)), scene.spawnLocations[i]) as GameObject;
					Instantiate(scene.dust, scene.spawnLocations[positionInTeam]);
					scene.selectedTeam[positionInTeam].GetComponent<Creature>().positionInTeam = positionInTeam;
					scene.selectedTeam[positionInTeam].GetComponent<Creature>().details.ID = scene.manager.creaturesOwned[ID].ID;
					scene.positions[i] = true;
					break;
				}
			}

			creatureImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		} else if(active) {
			Unselect();
		}

		if(scene.activeMembers == 3) {
			scene.start.interactable = true;
		}
	}

	public void Unselect() {
		creatureImage.color = Color.white;
		scene.TeamMemberPressed(this);
		active = false;
	}
}