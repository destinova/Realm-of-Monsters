using UnityEngine;

//Deal damage a 2nd time
public class DoubleHit : MonoBehaviour, ISkillEffect {
	private Creature target;

	private void AttackAgain(float damage) {
		Invoke("Animate", 1.1f);
		StartCoroutine(target.TakeDamage(1.25f, damage));
	}

	private void Animate() {
		GetComponent<Animator>().SetTrigger(GetComponent<Creature>().skills[0].skillName);
	}

    void ISkillEffect.Execute() {
		try {
			target = GetComponent<Creature>().bm.selectedTarget;
			AttackAgain(GetComponent<Creature>().bm.damageDealt);
		} catch {
			target = GetComponent<Creature>().bb.selectedTarget;
			AttackAgain(GetComponent<Creature>().bb.damageDealt);
		}
	}

	void ISkillEffect.IncreaseTurns(int newTurn) { }

	public GameObject effect { get; set; }
}