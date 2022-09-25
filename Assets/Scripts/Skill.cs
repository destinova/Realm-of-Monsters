using System;
using UnityEngine;

[Serializable]
public class Skill {
	public string skillName, skillDescription;
	[Header ("Types: 0=Attack, 1=Defend, 2=Heal, 3=Special")]
	public int type;
	public int coolDown;
	public float animationTimer;
	public float delay;
	public float skillPowerMultiplier;
	public float offset;
	public bool longRange, multipleTargets, specialEffect, atTarget, moveToCast;
	public GameObject skillEffect;
	public Sprite icon;
}