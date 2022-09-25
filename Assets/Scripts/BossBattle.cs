using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BossBattle : MonoBehaviour {	
	public Transform CM;
	public AudioClip music;
	public Vector3 cm_Position1, cm_Position2;
	public Quaternion cm_rotation1, cm_rotation2;
	public BossSpawner spawner;
    public float speed, offset;
	public List<Button> skillButtons;
	public Button attack;
	public Image picture, frame;
	public List<Image> timerImages;
	public TextMeshProUGUI speedScale, description, bossHealth;
    public GameObject skillDesc, victoryPanel, pauseMenu;
    public Sprite four, three, two, one;
	public Slider bossBar;

	[HideInInspector]
    public bool executingEffect = false;
	[HideInInspector]
	public GameManager manager;	
	[HideInInspector]
	public List<GameObject> allCreatures = new List<GameObject>();
	[HideInInspector]
	public Creature selectedTarget;
	[HideInInspector]
	public float damageDealt;
	[HideInInspector]
	public int fallenEnemies = 0, fallenCreatures = 0;

	private float x = 5f, y = 17f, plus = 15f;
	private bool selectATarget = false, firstCam = true;
    private bool singleAttack = false, atLocation = false, multi = false;
	private int skillNum, index;
    private Vector3 ogPos;
    private Creature activeCreature;
    private GameObject[] enemies = new GameObject[3];
	private GameObject[] creatures = new GameObject[3];
	private GameObject effect;
	
    void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		manager.GetComponent<CrossfadeAudiosource>().ChangeMusic(manager, music, 0.5f);

		spawner.Spawn();
	}

    void Update() {
        if(selectATarget) {
			if(Input.GetMouseButtonDown(0)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit;

				if(Physics.Raycast(ray, out raycastHit, 60f) && raycastHit.collider.gameObject.tag.Equals("Monster")) {
					Extensions.Click(manager, GetComponent<AudioSource>());
					if(selectedTarget != null)
						selectedTarget.selector.SetActive(false);

					selectedTarget = raycastHit.collider.gameObject.GetComponent<Creature>();
                    if(activeCreature.skills[skillNum].type == 1)
						selectedTarget.selector.GetComponent<Image>().color = new Color(0, 0.5f, 1);
                    else
						selectedTarget.selector.GetComponent<Image>().color = new Color(0.25f, 1f, 0.5f);
					selectedTarget.selector.SetActive(true);

                    attack.interactable = true;
				}
			}
		}

        if(singleAttack && !atLocation) {
            MoveToEnemy();
        } else if(singleAttack && atLocation) {
            MoveEffect();
        }
    }

    private void MoveToEnemy() {
		atLocation = true;

        if(atLocation) {
			activeCreature.GetComponent<Animator>().SetTrigger(activeCreature.skills[skillNum].skillName);
			DealSingleDamage();
		}
    }

	private void MoveEffect() {
		x = 5f;
		y = 17f;
		plus = 15f;
		if(allCreatures[index].tag.Equals("Enemy")) {
			x = -10f;
			y = 21f;
			plus = 3f;
		}

		if(multi) {
			if(Vector3.Distance(effect.transform.position, new Vector3(x, y, 0f)) > 6f) {
				effect.transform.position = Vector3.MoveTowards(effect.transform.position, new Vector3(x, y, 0f), Time.deltaTime * speed * 1.5f);
			} else {
				atLocation = false;
			}
		} else {
			if(Vector3.Distance(effect.transform.position, selectedTarget.transform.position) > 15f) {
				effect.transform.position = Vector3.MoveTowards(effect.transform.position, new Vector3(selectedTarget.transform.position.x, selectedTarget.transform.position.y + plus, selectedTarget.transform.position.z), Time.deltaTime * speed * 1.5f);
			} else {
				atLocation = false;
			}
		}

        if(!atLocation && !multi) {
            DealSingleDamage();
        } else if(!atLocation && multi) {
            singleAttack = false;
            multi = false;
            DealMultiDamage();
        }
	}

	private void DealSingleDamage() {
		singleAttack = false;
		bool dead = false;
		float damage = activeCreature.details.baseAtk * activeCreature.skills[skillNum].skillPowerMultiplier;
		float critDamage = damage * (activeCreature.details.baseCritDamage + 1f);

		if(CritChance()) {
			dead = selectedTarget.TakeDamage(critDamage);
			damageDealt = critDamage;
		} else {
			dead = selectedTarget.TakeDamage(damage);
			damageDealt = damage;
		}

		if(dead && selectedTarget.tag.Equals("Enemy")) {
			++fallenEnemies;
		} else if(dead && selectedTarget.tag.Equals("Monster")) {
			++fallenCreatures;
		}

        if(activeCreature.skills[skillNum].specialEffect) {
			try {
				activeCreature.GetComponent<ISkillEffect>().Execute();
			} catch { }
		}

		Invoke("EndTurn", activeCreature.skills[skillNum].animationTimer);
	}

    private void DealMultiDamage() {
		bool friend = true;
		bool dead;
		float damage = activeCreature.details.baseAtk * activeCreature.skills[skillNum].skillPowerMultiplier;
		float critDamage = damage * (activeCreature.details.baseCritDamage + 1f);

		if(activeCreature.tag.Equals("Enemy"))
			friend = false;

		if(CritChance()) {
			if(friend) {
				dead = allCreatures[3].GetComponent<Creature>().TakeDamage(critDamage);
				if(dead) {
					++fallenEnemies;
				}
			} else {
				for(int i = 0; i < 3; i++) {
					dead = allCreatures[i].GetComponent<Creature>().TakeDamage(critDamage);
					if(dead && !friend) {
						++fallenCreatures;
					}
				}
			}
			damageDealt = critDamage;
		} else {
			if(friend) {
				dead = allCreatures[3].GetComponent<Creature>().TakeDamage(damage);
				if(dead) {
					++fallenEnemies;
				}
			} else {
				for(int i = 0; i < 3; i++) {
					dead = allCreatures[i].GetComponent<Creature>().TakeDamage(damage);
					if(dead ) {
						++fallenCreatures;
					}
				}
			}
			damageDealt = damage;
		}

		if(activeCreature.skills[skillNum].specialEffect) {
			try {
				activeCreature.GetComponent<ISkillEffect>().Execute();
			} catch { }
		}

		Invoke("EndTurn", activeCreature.skills[skillNum].animationTimer);
	}

    private void MultiHeal() {
		bool friend = true;

		if(allCreatures[index].tag.Equals("Enemy"))
			friend = false;

		if(CritChance()) {
			if(friend) {
				for(int i = 0; i < 3; i++) {
					allCreatures[i].GetComponent<Creature>().Heal(activeCreature.details.baseAtk * activeCreature.skills[skillNum].skillPowerMultiplier * (activeCreature.details.baseCritDamage + 1f));
				}
			} else {
					allCreatures[3].GetComponent<Creature>().Heal(activeCreature.details.baseAtk * activeCreature.skills[skillNum].skillPowerMultiplier * (activeCreature.details.baseCritDamage + 1f));
			}
		} else {
			if(friend) {
				for(int i = 0; i < 3; i++) {
					allCreatures[i].GetComponent<Creature>().Heal(activeCreature.details.baseAtk * activeCreature.skills[skillNum].skillPowerMultiplier);
				}
			} else { 
					allCreatures[3].GetComponent<Creature>().Heal(activeCreature.details.baseAtk * activeCreature.skills[skillNum].skillPowerMultiplier);
			}
		}

		if(activeCreature.skills[skillNum].specialEffect) {
			activeCreature.GetComponent<ISkillEffect>().Execute();
		}

		Invoke("EndTurn", activeCreature.skills[skillNum].animationTimer);
	}

	private void SingleHeal() {
		if(CritChance()) {
			selectedTarget.GetComponent<Creature>().Heal(activeCreature.details.baseAtk * activeCreature.skills[skillNum].skillPowerMultiplier * (activeCreature.details.baseCritDamage + 1f));
		} else {
			selectedTarget.GetComponent<Creature>().Heal(activeCreature.details.baseAtk * activeCreature.skills[skillNum].skillPowerMultiplier);
		}

		if(activeCreature.skills[skillNum].specialEffect) {
			activeCreature.GetComponent<ISkillEffect>().Execute();
		}

		Invoke("EndTurn", activeCreature.skills[skillNum].animationTimer);
	}

	private void EndTurn() {
        atLocation = false;
        selectATarget = false;

		activeCreature.transform.position = ogPos;
        activeCreature.transform.rotation = new Quaternion(0, 0, 0, 0);
		activeCreature.selector.SetActive(false);
		if(activeCreature.tag.Equals("Monster"))
			activeCreature.info.SetActive(true);
		activeCreature.attacked = true;

		if(!activeCreature.boss) {
			activeCreature.GetComponent<CapsuleCollider>().radius = 0.75f;
		}

		if(fallenEnemies == 1 || fallenCreatures == 3) {
			Invoke("EndGame", 3f);
		} else
			SelectNextAttacker(false);
	}

	private void EndGame() {
		bool won;
		int fallCounter = 0;
		Time.timeScale = 1f;

		for(int i = 0; i < 4; i++) {
			allCreatures[i].SetActive(false);
			if(i > 2 && !allCreatures[i].GetComponent<Creature>().active) {
				++fallCounter;
			}
		}
		
		if(fallCounter == 1)
			won = true;
		else
			won = false;

		GetComponent<Canvas>().enabled = true;
		victoryPanel.GetComponent<VictoryPanel>().creatures.AddRange(creatures);
		victoryPanel.GetComponent<VictoryPanel>().won = won;
		victoryPanel.GetComponent<VictoryPanel>().Set();
		victoryPanel.SetActive(true);
	}

	private void SelectNextAttacker(bool nextWave) {
		int prevSpd = 0, index = 0, round = 0;
		activeCreature = null;

		for(int i = 0; i < 4; i++) {
			if(!allCreatures[i].GetComponent<Creature>().active || allCreatures[i].GetComponent<Creature>().attacked) {
				round++;
			}
		}

		if(round == 4 || nextWave) {
			for(int i = 0; i < 4; i++) {
				allCreatures[i].GetComponent<Creature>().attacked = false;
				allCreatures[i].GetComponent<Creature>().IncreaseTurn();
				//for(int j = 0; j < allCreatures[i].GetComponent<Creature>().skills.Count; j++) {
				//	allCreatures[i].GetComponent<Creature>().skills[j].turnsPassed++;
				//}
			}
		}

		for(int i = 0; i < 4; i++) {
			if(!allCreatures[i].GetComponent<Creature>().attacked && allCreatures[i].GetComponent<Creature>().active && allCreatures[i].GetComponent<Creature>().details.baseSpd > prevSpd) {
				prevSpd = (int)allCreatures[i].GetComponent<Creature>().details.baseSpd;
				index = i;
			}
		}

		this.index = index;

		if(allCreatures[index].tag.Equals("Monster")) {
			activeCreature = allCreatures[index].GetComponent<Creature>();
			setUI(false);
		} else {
			activeCreature = allCreatures[index].GetComponent<Creature>();
			//frame.gameObject.SetActive(false);
			GetComponent<Canvas>().enabled = false;
			Invoke("SelectRandomAttack", 1.5f);
		}
	}

    private void ResetSelectors() {
		selectATarget = false;
        attack.interactable = false;
        activeCreature.selector.SetActive(true);

		for(int i = 0; i < 3; i++) {
			if(creatures[i] != activeCreature.gameObject) {
				creatures[i].GetComponent<Creature>().selector.SetActive(false);
			} else {
				activeCreature.selector.GetComponent<Image>().color = new Color(1, 0.8f, 0);
			}
		}
	}

    public void Initialize() {
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		creatures = GameObject.FindGameObjectsWithTag("Monster");
		allCreatures.AddRange(creatures);
		allCreatures.AddRange(enemies);

		int index = 0, prevSpd = 0;
        for(int i = 0; i < 4; i++) {
            if(allCreatures[i].GetComponent<Creature>().details.baseSpd > prevSpd) {
                prevSpd = (int)allCreatures[i].GetComponent<Creature>().details.baseSpd;
                index = i;
            }
		}

		this.index = index;

		if(allCreatures[index].tag.Equals("Monster")) {
			activeCreature = allCreatures[index].GetComponent<Creature>();
			setUI(true);
		} else {
			activeCreature = allCreatures[index].GetComponent<Creature>();
			//frame.gameObject.SetActive(false);
			GetComponent<Canvas>().enabled = false;
			Invoke("SelectRandomAttack", 1.5f);
		}
    }

	private void SelectRandomTarget(int prev) {
		int choice = Random.Range(0, 3);
		if(choice == prev) {
			SelectRandomTarget(choice);
			return;
		}	
		if(!creatures[choice].GetComponent<Creature>().active) {
			SelectRandomTarget(choice);
			return;
		}

		selectedTarget = creatures[choice].GetComponent<Creature>();
		Attack();
	}

	private void SelectRandomEnemy() {
		int choice = 0;

		selectedTarget = enemies[choice].GetComponent<Creature>();
		Attack();
	}

	private void SelectRandomAttack() {
		skillNum = Random.Range(0, activeCreature.skills.Count);

		if(activeCreature.skills[skillNum].type == 0) {
			SelectRandomTarget(-1);
		} else {
			SelectRandomEnemy();
		}
	}

	private void Attack() {
		Skill skill = activeCreature.skills[skillNum];
		ogPos = activeCreature.transform.position;
		if(skill.type == 0)
			activeCreature.info.SetActive(false);

		if(skill.type == 0 && !skill.multipleTargets) {
			if(skill.longRange) {
				activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
				
				if(skill.atTarget) {
					effect = Instantiate(skill.skillEffect, selectedTarget.transform);
					Invoke("DealSingleDamage", skill.delay);

				} else {
					effect = Instantiate(skill.skillEffect, new Vector3(activeCreature.transform.position.x - skill.offset - 4f, activeCreature.transform.position.y + 15f, activeCreature.transform.position.z), Quaternion.identity);
					singleAttack = atLocation = true;
				}

			} else {
				singleAttack = true;
			}

		} else if(skill.type == 0 && skill.multipleTargets) {
			activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
			
			if(skill.atTarget) {
				effect = Instantiate(skill.skillEffect, new Vector3(-10, 20, 0), new Quaternion(0, 180, 0, 0));
				Invoke("DealMultiDamage", skill.delay);

			} else {
				effect = Instantiate(skill.skillEffect, new Vector3(activeCreature.transform.position.x - skill.offset - 4f, activeCreature.transform.position.y + 15f, activeCreature.transform.position.z), Quaternion.identity);
				singleAttack = atLocation = multi = true;
			}


		} else if(skill.type == 2 && skill.multipleTargets) {
			activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
			effect = Instantiate(skill.skillEffect, new Vector3(10, 20, 0), new Quaternion(-90, 0, 0, 90));
			MultiHeal();

		} else if(skill.type == 2 && !skill.multipleTargets) {
			activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
			effect = Instantiate(skill.skillEffect, selectedTarget.transform);
			SingleHeal();
		
		} else if(skill.type == 1 && !skill.multipleTargets && !skill.longRange) {
			activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
			activeCreature.GetComponent<ISkillEffect>().effect = Instantiate(skill.skillEffect, activeCreature.transform);
			activeCreature.GetComponent<ISkillEffect>().Execute();
			Invoke("EndTurn", skill.animationTimer);

		} else if(skill.type == 1 && skill.multipleTargets) {
			activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
			activeCreature.GetComponent<ISkillEffect>().effect = Instantiate(skill.skillEffect, new Vector3(5, 20, 0), new Quaternion(0, 0, 0, 0));
			activeCreature.GetComponent<ISkillEffect>().Execute();
			Invoke("EndTurn", skill.animationTimer);
		}
	}

    private void setUI(bool first) {
		//frame.gameObject.SetActive(true);
		GetComponent<Canvas>().enabled = true;
		activeCreature.selector.GetComponent<Image>().color = new Color(1, 0.8f, 0);
		activeCreature.selector.SetActive(true);
		picture.sprite = activeCreature.sprite;

		switch(activeCreature.type) {
            case Type.damage:
			frame.color = new Color(1, 0.1f, 0);
            break;
            case Type.tank:
			frame.color = new Color(0, 0.5f, 1);
            break;
			case Type.healer:
			frame.color = new Color(0.25f, 1f, 0.5f);
			break;
		}

		frame.GetComponent<Outline>().effectColor = frame.color;
        new ShowStats().Display(skillButtons[0].GetComponent<Image>(), skillButtons[1].GetComponent<Image>(), skillButtons[2].GetComponent<Image>(), activeCreature);

		if(first) {
			for(int i = 0; i < activeCreature.skills.Count; i++)
				skillButtons[i].interactable = true;
		} else 
			GetButtonTimers();		
	}

    private bool CritChance() {
        float ran = Random.Range(0f, 1f);

        if(ran <= activeCreature.details.baseCritChance)
            return true;

        return false;
    }

	/// <summary>
	/// Call after attacker selection
	/// </summary>
	private void GetButtonTimers() {
		int[] timers = new int[3];
		for(int i = 0; i < activeCreature.skills.Count; i++) {
			timers[i] = activeCreature.CheckQueue(i);
		}

		SetTimerOnButton(timers);
	}

    private void SetTimerOnButton(int[] timers) {
		for(int i = 0; i < activeCreature.skills.Count; i++) {
			if(timers[i] == 0) {
				skillButtons[i].interactable = true;
				timerImages[i].enabled = false;
			} else {
				skillButtons[i].interactable = false;
				switch(timers[i]) {
					case 1:
					timerImages[i].sprite = one;
					break;
					case 2:
					timerImages[i].sprite = two;
					break;
					case 3:
					timerImages[i].sprite = three;
					break;
					case 4:
					timerImages[i].sprite = four;
					break;
				}
				timerImages[i].enabled = true;
			}
		}

		switch(activeCreature.skills.Count) {
			case 1:
			timerImages[1].enabled = false;
			timerImages[2].enabled = false;
			break;
			case 2:
			timerImages[2].enabled = false;
			break;
		}
	}

	#region Button Listeners
	public void OnCamChange() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		if(firstCam) {
			CM.position = cm_Position2;
			CM.rotation = cm_rotation2;
			firstCam = false;
		} else {
			CM.position = cm_Position1;
			CM.rotation = cm_rotation1;
			firstCam = true;
		}
	}

    public void OnSkill(int skillNum) {
		Extensions.Click(manager, GetComponent<AudioSource>());
		this.skillNum = skillNum;
        Skill skill = activeCreature.skills[skillNum];

        ResetSelectors();

        new OnSkillClick().BattleDisplay(skillDesc, description, skill, activeCreature);

		switch(skillNum) {
            case 0:
			skillButtons[0].interactable = false;
			skillButtons[1].interactable = true;
			skillButtons[2].interactable = true;
            break;
            case 1:
			skillButtons[0].interactable = true;
			skillButtons[1].interactable = false;
			skillButtons[2].interactable = true;
			break;
            case 2:
			skillButtons[0].interactable = true;
			skillButtons[1].interactable = true;
			skillButtons[2].interactable = false;
			break;
        }

        if(skill.multipleTargets) {
            for(int i = 0; i < 3; i++) {
                if(skill.type == 0) {
                    enemies[0].GetComponent<Creature>().selector.GetComponent<Image>().color = new Color(1, 0.1f, 0);
                    enemies[0].GetComponent<Creature>().selector.SetActive(true);
                } else if(skill.type == 1) {
                    creatures[i].GetComponent<Creature>().selector.GetComponent<Image>().color = new Color(0, 0.5f, 1);
                    creatures[i].GetComponent<Creature>().selector.SetActive(true);
                } else if(skill.type == 2) {
					creatures[i].GetComponent<Creature>().selector.GetComponent<Image>().color = new Color(0.25f, 1f, 0.5f);
					creatures[i].GetComponent<Creature>().selector.SetActive(true);
				}
			}
			attack.interactable = true;
		} else {
            if(skill.type == 0) {
				selectedTarget = allCreatures[3].GetComponent<Creature>();
				attack.interactable = true;
			} else if(skill.type == 1 && skill.longRange) {
				selectATarget = true;
            } else if(skill.type == 1 && !skill.longRange) {
                activeCreature.selector.GetComponent<Image>().color = new Color(0, 0.5f, 1);
				attack.interactable = true;
			} else if(skill.type == 2 && skill.longRange) {
				selectATarget = true;
            } else if(skill.type == 2 && !skill.longRange) {
                activeCreature.selector.GetComponent<Image>().color = new Color(0.25f, 1f, 0.5f);
				attack.interactable = true;
			}
		}
	}

    public void OnActivate() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		Skill skill = activeCreature.skills[skillNum];
		activeCreature.AddToQueue(skillNum);
        attack.interactable = false;

		for(int i = 0; i < 3; i++) {
			skillButtons[i].interactable = false;
		}

        skillDesc.SetActive(false);
		ogPos = activeCreature.transform.position;
		if(skill.type == 0)
			activeCreature.info.SetActive(false);

		if(skill.type == 0) {
			AttackSelected(skill);
		} else if(skill.type == 2) {
			HealSelected(skill);
		} else {
			GuardSelected(skill);
		}
	}

	public void OnPause() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		Time.timeScale = 0f;
		pauseMenu.SetActive(true);
	}

	public void OnResume() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		Time.timeScale = 1f;
		speedScale.text = "x1";
		pauseMenu.SetActive(false);
	}

	public void OnQuit() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		Time.timeScale = 1f;
		SceneManager.LoadScene("World Map");
	}

	public void OnScale() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		if(Time.timeScale == 1) {
            Time.timeScale = 2f;
            speedScale.text = "x2";
        } else {
			Time.timeScale = 1f;
			speedScale.text = "x1";
		}
    }
	#endregion

	private void AttackSelected(Skill skill) {
		//single short
		if(!skill.multipleTargets) {
			selectedTarget.selector.SetActive(false);

			//Single target long
			if(skill.longRange) {
				activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);

				//Single target long at target
				if(skill.atTarget) {
					effect = Instantiate(skill.skillEffect, selectedTarget.transform);
					Invoke("DealSingleDamage", skill.delay);

					//Single target long move to target
				} else {
					effect = Instantiate(skill.skillEffect, new Vector3(activeCreature.transform.position.x + skill.offset, activeCreature.transform.position.y + 4f, activeCreature.transform.position.z), Quaternion.identity);
					singleAttack = atLocation = true;
				}

			//Single target move to target
			} else {
				singleAttack = true;
			}

		//Multi target
		} else if(skill.multipleTargets) {
			activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
			enemies[0].GetComponent<Creature>().selector.SetActive(false);

			//Multi target at target
			if(skill.atTarget) {
				if(skill.moveToCast) {
					ogPos = activeCreature.transform.position;
					activeCreature.GetComponent<CapsuleCollider>().radius = 0.1f;
					activeCreature.transform.position = new Vector3(-5, 20, 0);
				}
				effect = Instantiate(skill.skillEffect);
				Invoke("DealMultiDamage", skill.delay);

				//Multi target move to target
			} else {
				effect = Instantiate(skill.skillEffect, new Vector3(activeCreature.transform.position.x + skill.offset, activeCreature.transform.position.y + 4f, activeCreature.transform.position.z), Quaternion.identity);
				singleAttack = atLocation = multi = true;
			}
		}

	}

	private void HealSelected(Skill skill) {
		if(skill.multipleTargets) {
			activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
			for(int i = 0; i < 3; i++) {
				creatures[i].GetComponent<Creature>().selector.SetActive(false);
			}
			effect = Instantiate(skill.skillEffect, new Vector3(-10, 20, 0), new Quaternion(-90, 0, 0, 90));
			MultiHeal();
			
		//Single heal
		} else if(!skill.multipleTargets) {
			activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
			selectedTarget.GetComponent<Creature>().selector.SetActive(false);
			effect = Instantiate(skill.skillEffect, selectedTarget.transform);
			SingleHeal();
		}
	}

	private void GuardSelected(Skill skill) {
		if(!skill.multipleTargets && !skill.longRange) {
			activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
			activeCreature.GetComponent<ISkillEffect>().effect = Instantiate(skill.skillEffect, activeCreature.transform);
			activeCreature.GetComponent<ISkillEffect>().Execute();
			Invoke("EndTurn", skill.animationTimer);
		} else if(skill.multipleTargets) {
			for(int i = 0; i < 3; i++) {
				creatures[i].GetComponent<Creature>().selector.SetActive(false);
			}
			activeCreature.GetComponent<Animator>().SetTrigger(skill.skillName);
			activeCreature.GetComponent<ISkillEffect>().effect = Instantiate(skill.skillEffect);
			activeCreature.GetComponent<ISkillEffect>().Execute();
			Invoke("EndTurn", skill.animationTimer);
		}
	}
}