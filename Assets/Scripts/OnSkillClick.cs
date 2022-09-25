using UnityEngine;
using TMPro;

public class OnSkillClick{
    public void Display(Creature creature, GameObject popUp, TextMeshProUGUI description, int skillNumber) {
		string skillName = "<color=#ff9045><b>" + creature.skills[skillNumber].skillName + "</b></color>\r\n", desc = "\r\n<i>" + creature.skills[skillNumber].skillDescription + "</i>";
		string targets, type, power;
		if(creature.skills[skillNumber].multipleTargets)
			targets = "All";
		else
			targets = "1";

		switch(creature.skills[skillNumber].type) {
			case 0:
			type = "Attack";
			break;
			case 1:
			type = "Defense";
			break;
			case 2:
			type = "Healing";
			break;
			case 3:
			type = "Special";
			break;
			default:
			type = "Attack";
			break;
		}

		if(creature.skills[skillNumber].type == 0 || creature.skills[skillNumber].type == 2) {
			power = Extensions.KFormat(Mathf.RoundToInt(creature.skills[skillNumber].skillPowerMultiplier * creature.details.baseAtk));
		} else {
			power = "-";
		}

		description.text = skillName + "<color=#ff2525>Power:</color> " + power + "\r\n<color=#25ff25>Type: </color>" +
			type + "\r\n<color=#00aaff>Targets: </color>" + targets + "\r\n<color=#000000>Cooldown: </color>" + creature.skills[skillNumber].coolDown + " Turn(s)" + desc;

		popUp.SetActive(true);
	}

	public void BattleDisplay(GameObject popUp, TextMeshProUGUI description, Skill skill, Creature creature) {
		string skillName = "<color=#ff9045><b>" + skill.skillName + "</b></color>\n", desc = "\n<i>" + skill.skillDescription + "</i>";
		string targets, type, power;
		if(skill.multipleTargets)
			targets = "All";
		else
			targets = "1";

		switch(skill.type) {
			case 0:
			type = "Attack";
			break;
			case 1:
			type = "Defense";
			break;
			case 2:
			type = "Healing";
			break;
			case 3:
			type = "Special";
			break;
			default:
			type = "Attack";
			break;
		}

		if(skill.type == 0 || skill.type == 2) {
			power = Extensions.KFormat(Mathf.RoundToInt(skill.skillPowerMultiplier * creature.details.baseAtk));
		} else {
			power = "-";
		}

		description.text = skillName + "<color=#ff2525>Power:</color> " + power + "  |  <color=#25ff25>Type: </color>" +
			type + "  |  <color=#00aaff>Targets: </color>" + targets + "  |  <color=#C57CFF>Cooldown: </color>" + skill.coolDown + " Turn(s)" + desc;

		popUp.SetActive(true);
	}
}