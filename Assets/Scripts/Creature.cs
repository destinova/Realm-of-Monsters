using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System;

public class Creature : MonoBehaviour {
	public Creature evolutionPrefab;
    public CreatureDetails details;
    public List<Skill> skills;
	public Type type;
	public Sprite sprite;
	public GameObject shadow, info, selector, damage, puff, bossPuff;
	public Material m_healing, m_damage;

	[HideInInspector]
	public BattleManager bm;
	[HideInInspector]
	public BossBattle bb;
	[HideInInspector]
	public FirstBattle fb;
	[HideInInspector]
	public int turns, positionInTeam = -1, positionInList;
	[HideInInspector]
	public float baseHP, baseAtk, baseDef, baseSpd, baseCritChance, baseCritDamage, guardingValue = 0f;
	[HideInInspector]
	public List<int> skillQueue = new List<int>();
	public bool attacked = false, active = true, boss = false;

	private int level;

	private void Awake() {
		if(SceneManager.GetActiveScene().name.Equals("Library")) {
			shadow.SetActive(false);
			info.SetActive(false);
		} else if(SceneManager.GetActiveScene().name.Equals("Team Selection")) {
			shadow.SetActive(false);
			info.SetActive(false);
			GetComponent<CreatureInteraction>().enabled = false;
		} else if(SceneManager.GetActiveScene().name.Equals("Upgrading") || SceneManager.GetActiveScene().name.Equals("Summoning")) {
			info.SetActive(false);
		} else if(SceneManager.GetActiveScene().name.Equals("Battle")){
			bm = GameObject.FindGameObjectWithTag("UI").GetComponent<BattleManager>();

			for(int i = 0; i < skills.Count; i++) {
				skillQueue.Add(0);
			}
		} else if(SceneManager.GetActiveScene().name.Equals("Boss Battle")) {
			bb = GameObject.FindGameObjectWithTag("UI").GetComponent<BossBattle>();

			for(int i = 0; i < skills.Count; i++) {
				skillQueue.Add(0);
			}
		} else if(SceneManager.GetActiveScene().name.Equals("First Battle")) {
			fb = GameObject.FindGameObjectWithTag("UI").GetComponent<FirstBattle>();

			for(int i = 0; i < skills.Count; i++) {
				skillQueue.Add(0);
			}
		}

		level = details.level;
		baseHP = details.baseHP;
		baseAtk = details.baseAtk;
		baseDef = details.baseDef;
		baseSpd = details.baseSpd;
		baseCritChance = details.baseCritChance;
		baseCritDamage = details.baseCritDamage;
	}

	public void SetDetails() {
		if(!details.canRankUp && !details.canEvolve) {
			RankUp(false);
		}

		LevelUp(false);
		info.GetComponentInChildren<Slider>().maxValue = details.baseHP;
		info.GetComponentInChildren<Slider>().value = details.baseHP;
		info.GetComponentInChildren<TextMeshProUGUI>().text = Extensions.KFormat((int)details.baseHP);
	}

	public void Bossify() {
		baseHP = details.baseHP = Mathf.Pow(details.baseHP, 1.32f);
		baseAtk = details.baseAtk = Mathf.Pow(details.baseAtk, 1.05f);
		baseDef = details.baseDef = Mathf.Clamp(Mathf.CeilToInt(details.baseDef * 1.25f), 0, 85);
		baseSpd = details.baseSpd = Mathf.Pow(details.baseSpd, 1.1f);
		baseCritChance = details.baseCritChance = Mathf.Clamp(details.baseCritChance * 1.1f, 0, 0.65f);
		baseCritDamage = details.baseCritDamage = Mathf.Clamp(details.baseCritDamage * 1.11f, 0, 3.5f);

		boss = true;
		info.SetActive(false);
	}

	public IEnumerator TakeDamage(float timer, float damage) {
		yield return new WaitForSeconds(timer);
		float dealDmg = Mathf.Clamp(damage - (damage * ((details.baseDef + guardingValue) / 100)), 0, int.MaxValue);

		this.damage.GetComponentInChildren<TextMeshProUGUI>().text = Extensions.KFormat((int)(dealDmg));
		this.damage.GetComponentInChildren<TextMeshProUGUI>().fontMaterial = m_damage;
		this.damage.GetComponent<Animation>().Play();
		this.damage.SetActive(true);
		Invoke("StopAnim", 1f);

		GetComponent<Animator>().SetTrigger("Damage");
		details.baseHP = Mathf.Clamp(details.baseHP - (dealDmg), 0, int.MaxValue);

		SetHPBar();

		if(details.baseHP < 1 && active) {
			active = false;
			GetComponent<Animator>().SetBool("Die", true);
			Invoke("Die", 1.25f);
			try {
				if(gameObject.tag.Equals("Enemy")) {
					++GetComponent<Creature>().bm.fallenEnemies;
				} else {
					++GetComponent<Creature>().bm.fallenCreatures;
				}
			} catch {
				if(gameObject.tag.Equals("Enemy")) {
					++GetComponent<Creature>().bb.fallenEnemies;
				} else {
					++GetComponent<Creature>().bb.fallenCreatures;
				}
			}
		}

		yield return false;
	}

	public bool TakeDamage(float damage) {
		float dealDmg = Mathf.Clamp(damage - (damage * ((details.baseDef + guardingValue) / 100)), 0, int.MaxValue);

		this.damage.GetComponentInChildren<TextMeshProUGUI>().text = Extensions.KFormat((int)(dealDmg));
		this.damage.GetComponentInChildren<TextMeshProUGUI>().fontMaterial = m_damage;
		this.damage.GetComponent<Animation>().Play();
		this.damage.SetActive(true);
		Invoke("StopAnim", 1f);

		GetComponent<Animator>().SetTrigger("Damage");
		details.baseHP = Mathf.Clamp(details.baseHP - (dealDmg), 0, int.MaxValue);

		SetHPBar();

		if(details.baseHP < 1 && active) {
			active = false;
			GetComponent<Animator>().SetBool("Die", true);
			Invoke("Die", 1.25f);
			return true;
		}

		return false;
	}

	public void SetHPBar() {
		if(boss) {
			bb.bossBar.value = details.baseHP;
			bb.bossHealth.text = Extensions.KFormat((int)details.baseHP);
		} else {
			info.GetComponentInChildren<Slider>().value = details.baseHP;
			info.GetComponentInChildren<TextMeshProUGUI>().text = Extensions.KFormat((int)details.baseHP);
		}
	}

	public void Heal(float healing) {
		damage.GetComponentInChildren<TextMeshProUGUI>().text = Extensions.KFormat((int)healing);
		damage.GetComponentInChildren<TextMeshProUGUI>().fontMaterial = m_healing;
		damage.GetComponent<Animation>().Play();
		damage.SetActive(true);
		Invoke("StopAnim", 1f);
		details.baseHP = Mathf.Clamp(healing + details.baseHP, 0, baseHP);

		SetHPBar();
	}

	private void StopAnim() {
		damage.SetActive(false);
	}

	private void Die() {
		if(GetComponent<ISkillEffect>() != null) {
			GetComponent<ISkillEffect>().IncreaseTurns(int.MaxValue);
		}

		if(boss) {
			Instantiate(bossPuff, transform.position + new Vector3(0, 2f, 0), new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f));
		}
		else
			Instantiate(puff, transform.position, new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f));
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Calculate how much XP is needed based on current level and the number of ranks the creature has moved up
	/// </summary>
	/// <returns></returns>
	public int HowMuchXpIsNeeded() {
		float num = details.level + ((details.maxLevel + 5) * details.ranksIncreased);
		float baseNum = num + 1f;

		return Mathf.FloorToInt(baseNum / 2f * baseNum * details.rank * 25f / details.xpMultiplier);
	}

	/// <summary>
	/// Increase creature stats based on their current level
	/// </summary>
	/// <param name="upgrading">If the creature is getting leveled up through an upgrade</param>
	public void LevelUp(bool upgrading) {
		if(upgrading) {
			++details.level;
			details.xp = 0;
		} else {
			if(!details.canRankUp && !details.canEvolve) {
				level += details.maxLevel;
			}
		}

		details.baseHP = Mathf.Ceil(details.level == 1 ? baseHP : baseHP + (baseHP * details.level * 0.25f));
		details.baseAtk = Mathf.Ceil(details.level == 1 ? baseAtk : baseAtk + (baseAtk * details.level * 0.225f));
		details.baseDef = Mathf.Clamp(Mathf.Ceil(details.level == 1 ? baseDef : baseDef + (baseDef * details.level * 0.0065f)), 0, 85);
		details.baseSpd = Mathf.Ceil(details.level == 1 ? baseSpd : baseSpd + (baseSpd * details.level * 0.075f));
		details.baseCritChance = Mathf.Clamp(details.level == 1 ? baseCritChance : baseCritChance + (baseCritChance * details.level * 0.01f), 0, 0.65f);
		details.baseCritDamage = Mathf.Clamp(details.level == 1 ? baseCritDamage : baseCritDamage + (baseCritDamage * details.level * 0.1f), 0, 3.5f);

		if(!upgrading && !SceneManager.GetActiveScene().name.Equals("Upgrading")) {
			baseHP = details.baseHP;
		}
	}

	public void SetLevel(int level) {
		details.level = level;

		baseHP = details.baseHP = Mathf.Ceil(details.level == 1 ? baseHP : baseHP + (baseHP * details.level * 0.25f));
		details.baseAtk = Mathf.Ceil(details.level == 1 ? baseAtk : baseAtk + (baseAtk * details.level * 0.225f));
		details.baseDef = Mathf.Clamp(Mathf.Ceil(details.level == 1 ? baseDef : baseDef + (baseDef * details.level * 0.0065f)), 0, 85);
		details.baseSpd = Mathf.Ceil(details.level == 1 ? baseSpd : baseSpd + (baseSpd * details.level * 0.075f));
		details.baseCritChance = Mathf.Clamp(details.level == 1 ? baseCritChance : baseCritChance + (baseCritChance * details.level * 0.01f), 0, 0.65f);
		details.baseCritDamage = Mathf.Clamp(details.level == 1 ? baseCritDamage : baseCritDamage + (baseCritDamage * details.level * 0.1f), 0, 3.5f);

		info.GetComponentInChildren<Slider>().maxValue = details.baseHP;
		info.GetComponentInChildren<Slider>().value = details.baseHP;
		info.GetComponentInChildren<TextMeshProUGUI>().text = Extensions.KFormat((int)details.baseHP);
	}

	public void RankUp(bool upgrading) {
		int level = this.level;
		if(upgrading) {
			++details.rank;
			++details.ranksIncreased;
			details.xp = 0;
			details.level = 1;
			level += details.maxLevel;
			details.canRankUp = false;
		} else {
			level = details.maxLevel + 1;
		}

		baseHP = details.baseHP = Mathf.Ceil(baseHP + (baseHP * level * 0.35f));
		baseAtk = details.baseAtk = Mathf.Ceil(baseAtk + (baseAtk * level * 0.325f));
		baseDef = details.baseDef = Mathf.Clamp(Mathf.Ceil(baseDef + (baseDef * level * 0.0075f)), 0, 85);
		baseSpd = details.baseSpd = Mathf.Ceil(baseSpd + (baseSpd * level * 0.085f));
		baseCritChance = details.baseCritChance = Mathf.Clamp(baseCritChance + (baseCritChance * level * 0.012f), 0, 0.65f);
		baseCritDamage = details.baseCritDamage = Mathf.Clamp(baseCritDamage + (baseCritDamage * level * 0.105f), 0, 3.5f);
	}

	public bool GainBattleXp(int xp, GameManager manager) {
		if((xp * details.xpMultiplier + details.xp) >= HowMuchXpIsNeeded()) {
			if(details.level != details.maxLevel) {
				xp = Mathf.FloorToInt(xp * details.xpMultiplier + details.xp - HowMuchXpIsNeeded());
				LevelUp(false);
				++details.level;
				if(xp >= HowMuchXpIsNeeded()) {
					GainBattleXp(xp, manager);
				} else {
					details.xp = xp;
					manager.creaturesOwned[positionInList].xp = xp;
					manager.creaturesOwned[positionInList].level = details.level;
					manager.Save();
				}
			}
			return true;
		} else {
			if(details.level != details.maxLevel) {
				details.xp += Mathf.FloorToInt(xp * details.xpMultiplier);
				manager.creaturesOwned[positionInList].xp = details.xp;
				manager.Save();
			}
			return false;
		}
	}

	public void GainXp(int magicStuff, GameManager manager) {
		details.xp += magicStuff * 100;
		manager.creaturesOwned[positionInList].xp = details.xp;
		manager.Save();
	}

	/// <summary>
	/// Add cooldown to the used effect
	/// </summary>
	/// <param name="skillNum"></param>
	public void AddToQueue(int skillNum) {
		skillQueue[skillNum] = skills[skillNum].coolDown;
	}

	/// <summary>
	/// Check cooldown on an effect and reduce that cooldown by 1. Check through all skill with a loop
	/// </summary>
	/// <param name="skillNum"></param>
	/// <returns></returns>
	public int CheckQueue(int skillNum) {
		int cd = skillQueue[skillNum];
		skillQueue[skillNum] = Mathf.Clamp(--skillQueue[skillNum], 0, int.MaxValue);

		return cd;
	}

	public void IncreaseTurn() {
		++turns;
		if(GetComponent<ISkillEffect>() != null) {
			GetComponent<ISkillEffect>().IncreaseTurns(turns);
		}
	}
}