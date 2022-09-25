using UnityEngine;

//After attack it loses 25% of its HP.
public class ReduceHP : MonoBehaviour, ISkillEffect {
	private float reduction = 0.25f;

	private void ReduceHealth() {
		float reductionFactor = 1 - reduction;
		float newHealth = GetComponent<Creature>().details.baseHP * reductionFactor;

		GetComponent<Creature>().details.baseHP = newHealth;
		GetComponent<Creature>().SetHPBar();
	}

    void ISkillEffect.Execute() {
		ReduceHealth();
	}

	void ISkillEffect.IncreaseTurns(int newTurn) { }

	public GameObject effect { get; set; }
}