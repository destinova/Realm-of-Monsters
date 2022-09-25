using UnityEngine;

//Restores its own HP by half the ATK points.
public class HitDrain : MonoBehaviour, ISkillEffect {
	private void HealSelf() {
		float healing = GetComponent<Creature>().details.baseAtk * 0.5f;
		GetComponent<Creature>().Heal(healing);
	}

    void ISkillEffect.Execute() {
		HealSelf();
	}

	void ISkillEffect.IncreaseTurns(int newTurn) { }

	public GameObject effect { get; set; }
}