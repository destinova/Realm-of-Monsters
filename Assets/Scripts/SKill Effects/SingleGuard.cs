using UnityEngine;

public class SingleGuard : MonoBehaviour, ISkillEffect {
	public float value;

	private int turns;
	private bool active = false;

	public GameObject effect { get; set; }

	void ISkillEffect.Execute() {
		active = true;
		turns = GetComponent<Creature>().turns;
		GetComponent<Creature>().guardingValue += value;
	}

	void ISkillEffect.IncreaseTurns(int newTurn) {
		if(newTurn > turns)
			EndScript();
	}

	public void EndScript() {
		GetComponent<Creature>().guardingValue -= value;

		if(active) {
			active = false;
			Destroy(effect);
		}
	}
}