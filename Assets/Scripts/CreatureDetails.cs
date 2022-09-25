using System;

[Serializable]
public class CreatureDetails {
	public string name;
	public int ID, level, rank, xp, maxLevel, ranksIncreased;
	public float baseHP, baseAtk, baseDef, baseSpd, baseCritChance, baseCritDamage, xpMultiplier;
	public int rankUpValue, evolutionValue;
	public bool canEvolve, canRankUp;
}