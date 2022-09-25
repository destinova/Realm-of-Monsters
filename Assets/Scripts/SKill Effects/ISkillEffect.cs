using UnityEngine;

public interface ISkillEffect {
	public GameObject effect { get; set; }

	public void Execute();

	public void IncreaseTurns(int newTurn);
}