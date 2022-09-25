using System.Collections.Generic;
using UnityEngine;

public class TeamGuard : MonoBehaviour, ISkillEffect {
	public float value;

	private int turns;
	private bool active = false;
	private List<GameObject> creatures = new List<GameObject>();

	public GameObject effect { get; set; }

	void ISkillEffect.Execute() {
		active = true;
		turns = GetComponent<Creature>().turns;

		try {
			creatures.AddRange(GetComponent<Creature>().bm.allCreatures);
		} catch {
			creatures.AddRange(GetComponent<Creature>().bb.allCreatures);
		}

		for(int i = 0; i < 3; i++) {
			if(gameObject.tag.Equals("Monster")) {
				creatures[i].GetComponent<Creature>().guardingValue += value;
			} else {
				try {
					creatures[i + 3].GetComponent<Creature>().guardingValue += value;
				} catch {
					creatures[3].GetComponent<Creature>().guardingValue += value;
				}
			}
		}
	}

	void ISkillEffect.IncreaseTurns(int newTurn) {
		if(newTurn > turns && active)
			EndScript();
	}

	public void EndScript() {
		if(active) {
			active = false;

			for(int i = 0; i < 3; i++) {
				if(gameObject.tag.Equals("Monster")) {
					creatures[i].GetComponent<Creature>().guardingValue -= value;
				} else {
					try {
						creatures[i + 3].GetComponent<Creature>().guardingValue -= value;
					} catch {
						creatures[3].GetComponent<Creature>().guardingValue -= value;
					}
				}
			}

			Destroy(effect);
		}
	}
}