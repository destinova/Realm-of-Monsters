using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowStats {
	public void Display(TextMeshProUGUI name, Slider rank, TextMeshProUGUI hp, TextMeshProUGUI atk, TextMeshProUGUI def, TextMeshProUGUI spd, TextMeshProUGUI crit, TextMeshProUGUI critDmg, Creature creature, 
		Image skill1, Image skill2, Image skill3, Image type, Sprite dmg, Sprite tank, Sprite healer) {
		name.text = creature.details.name;
		rank.value = creature.details.rank;

		hp.text = Extensions.KFormat((int)creature.details.baseHP);
		atk.text = Extensions.KFormat((int)creature.details.baseAtk);
		def.text = creature.details.baseDef.ToString();
		spd.text = Extensions.KFormat((int)creature.details.baseSpd);
		crit.text = Mathf.Round(creature.details.baseCritChance * 100f).ToString() + "%";
		critDmg.text = Mathf.Round(creature.details.baseCritDamage * 100f).ToString() + "%";

		switch(creature.type) {
			case Type.damage:
			type.sprite = dmg;
			break;
			case Type.healer:
			type.sprite = healer;
			break;
			case Type.tank:
			type.sprite = tank;
			break;
		}

		skill1.sprite = creature.skills[0].icon;
		if(creature.skills.Count == 3) {
			skill2.sprite = creature.skills[1].icon;
			skill3.sprite = creature.skills[2].icon;
			skill2.enabled = true;
			skill3.enabled = true;
		} else if(creature.skills.Count == 2) {
			skill2.enabled = true;
			skill3.enabled = false;
			skill2.sprite = creature.skills[1].icon;
		} else {
			skill2.enabled = false;
			skill3.enabled = false;
		}
	}

	public void Display(TextMeshProUGUI name, Slider rank, Creature creature, Image skill1, Image skill2, Image skill3, Image type, Sprite dmg, Sprite tank, Sprite healer) {
		name.text = creature.details.name;
		rank.value = creature.details.rank;

		switch(creature.type) {
			case Type.damage:
			type.sprite = dmg;
			break;
			case Type.healer:
			type.sprite = healer;
			break;
			case Type.tank:
			type.sprite = tank;
			break;
		}

		skill1.sprite = creature.skills[0].icon;
		if(creature.skills.Count == 3) {
			skill2.sprite = creature.skills[1].icon;
			skill3.sprite = creature.skills[2].icon;
			skill2.enabled = true;
			skill3.enabled = true;
		} else if(creature.skills.Count == 2) {
			skill2.enabled = true;
			skill3.enabled = false;
			skill2.sprite = creature.skills[1].icon;
		} else {
			skill2.enabled = false;
			skill3.enabled = false;
		}
	}

	public void Display(Image skill1, Image skill2, Image skill3, Creature creature) {
		skill1.sprite = creature.skills[0].icon;
		if(creature.skills.Count == 3) {
			skill2.sprite = creature.skills[1].icon;
			skill3.sprite = creature.skills[2].icon;
			skill2.enabled = true;
			skill3.enabled = true;
		} else if(creature.skills.Count == 2) {
			skill2.enabled = true;
			skill3.enabled = false;
			skill2.sprite = creature.skills[1].icon;
		} else {
			skill2.enabled = false;
			skill3.enabled = false;
		}
	}
}