using UnityEngine;

//Restores its own HP by 75% the ATK points.
public class LifeDrain : MonoBehaviour, ISkillEffect {
	private void HealSelf() {
		float healing = GetComponent<Creature>().details.baseAtk * 0.75f;
		GetComponent<Creature>().Heal(healing);
	}

    void ISkillEffect.Execute() {
		HealSelf();
	}

	void ISkillEffect.IncreaseTurns(int newTurn) { }

	public GameObject effect { get; set; }
}