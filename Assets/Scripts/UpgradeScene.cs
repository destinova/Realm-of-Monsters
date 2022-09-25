using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class UpgradeScene : MonoBehaviour {
	#region Variables
	public List<string> tutorial;
	public AudioClip music;
	public GameObject puffPrefab, tutWindow;
	public Button upgradeButton, levelUp, evolve, rankUp, magicStuff, coins, release, tutButton;
    public GameObject upgradeButtons, creatureSelector, skillPopUp, materialNeededObject, noMaterialPopUp, purchaseWindow;
    public TextMeshProUGUI creatureName, level, xp, hp, atk, def, spd, crit, critDmg, materialNeeded, skillDescription, magicStuffText, coinText, upgradeText, noMaterialPopUpText, tutMessage;
    public Slider xpBar, rank;
    public Image type, skill1, skill2, skill3;
    public Sprite damageClass, tankClass, healerClass;
    public Transform spawnPoint, listView;
    public GameObject dust, evolution, rankUpArrow, framePrefab;
	public ParticleSystem plus;

    [HideInInspector]
    public bool selected = false;
	[HideInInspector]
	public Creature selectedCreature;
	[HideInInspector]
	public FrameSelector currentFrame;
	[HideInInspector]
	public GameManager manager;

	private bool mainScreen = true, leveling = true, evolving = false, ranking = false, purchase = false, confirmRelease = false;
    private int stuff, coinsNeeded, tut;
	private List<FrameSelector> f = new List<FrameSelector>();
	#endregion

	void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		manager.GetComponent<CrossfadeAudiosource>().ChangeMusic(manager, music, 1f);

		magicStuffText.text = Extensions.KFormat(manager.magicValue);
        coinText.text = Extensions.KFormat(manager.coinValue);

		int tuto = PlayerPrefs.GetInt("Upgrade Tutorial", 0);
		if(tuto == 0) {
			tutMessage.text = tutorial[tut];
			tutWindow.SetActive(true);
			tutButton.onClick.AddListener(delegate { NextTut(); });
		}

		Initialize();
    }

	private void Initialize() {
		for(int i = 0; i < manager.creaturesOwned.Count; i++) {
			f.Add(Instantiate(framePrefab, listView).GetComponent<FrameSelector>());
			f[i].scene = this;
			f[i].ID = manager.creaturesOwned[i].ID;
			f[i].positionInList = i;
			f[i].SetFrame();
			if(i == 0) {
				currentFrame = f[i];
				f[i].Selected();
				//Debug.Log("Selected: " + f[i].name + " ID: "  + f[i].ID + " | Pos in List: " + f[i].positionInList);
			}
			//Debug.Log(f[i].name + " ID: "  + f[i].ID + " | Pos in List: " + f[i].positionInList);
		}

		listView.GetComponent<RectTransform>().sizeDelta = new Vector2(manager.creaturesOwned.Count * 256f, 150f);
	}

	private void ReInitialize() {
		selectedCreature.gameObject.SetActive(false);
		Instantiate(puffPrefab);

		for(int i = 0; i < f.Count; i++) {
			Destroy(f[i].gameObject);
		}

		f.Clear();

		//Debug.Log("After release list");
		for(int i = 0; i < manager.creaturesOwned.Count; i++) {
			f.Add(Instantiate(framePrefab, listView).GetComponent<FrameSelector>());
			f[i].scene = this;
			f[i].ID = manager.creaturesOwned[i].ID;
			f[i].positionInList = i;
			f[i].SetFrame();
			if(i == 0) {
				currentFrame = f[i];
				//Debug.Log("Selected: " + f[i].name + " ID: " + f[i].ID + " | Pos in List: " + f[i].positionInList);
				Invoke("Spawn", 1f);
			}
			//Debug.Log(f[i].name + " ID: " + f[i].ID + " | Pos in List: " + f[i].positionInList);
		}

		listView.GetComponent<RectTransform>().sizeDelta = new Vector2(manager.creaturesOwned.Count * 256f, 150f);
		
	}

	private void Spawn() {
		currentFrame.Selected();
		GetComponent<Canvas>().enabled = true;
		OnBack();
	}

	/// <summary>
	/// Displays the selected creature's details on the UI
	/// </summary>
	public void SetCreatureDetails() {
        new ShowStats().Display(creatureName, rank, hp, atk, def, spd, crit, critDmg, selectedCreature, skill1, skill2, skill3, type, damageClass, tankClass, healerClass);

		SetSameStats();
	}

	/// <summary>
	/// Calculate and display the material needed to upgrade to the next level/rank/evolution
	/// </summary>
	/// <param name="materialType">1 = magic stuff leveling, 2 = coins evolving, 3 = magic ranking</param>
	private void SetMaterialNeed(int materials) {
        if(materials == 1) {
            if(selectedCreature.details.level == selectedCreature.details.maxLevel) {
                materialNeeded.text = "max level\r\n<color=#0094ff><size=96>-</size></color>";
            } else {
                stuff = Mathf.CeilToInt((selectedCreature.HowMuchXpIsNeeded() - selectedCreature.details.xp) / 100f);
                materialNeeded.text = "magic stuff needed\r\n<color=#0094ff><size=96>" + Extensions.KFormat(stuff) + "</size></color>";
            }
        } else if(materials == 2) {
            if(selectedCreature.details.level == selectedCreature.details.maxLevel && selectedCreature.details.canEvolve) {
                SetEvoStats();
                coinsNeeded = selectedCreature.details.evolutionValue;
                materialNeeded.text = "Coins needed\r\n<color=#FFD100><size=96>" + Extensions.KFormat(coinsNeeded) + "</size></color>";
            } else {
                materialNeeded.text = "<color=#ff2525>Level up to\r\nlevel <size=96>" + selectedCreature.details.maxLevel + "</size> first</color>";
            }

            if(!selectedCreature.details.canEvolve) {
				materialNeeded.text = "This creature\r\ncan't evolve";
			}
        } else {
			if(selectedCreature.details.level == selectedCreature.details.maxLevel && selectedCreature.details.canRankUp) {
                SetRankStats();
                stuff = selectedCreature.details.rankUpValue;
				materialNeeded.text = "Magic stuff needed\r\n<color=#0094ff><size=96>" + Extensions.KFormat(stuff) + "</size></color>";
			} else {
				materialNeeded.text = "<color=#ff2525>Level up to\r\nlevel <size=96>" + selectedCreature.details.maxLevel + "</size> first</color>";
			}

            if(!selectedCreature.details.canRankUp) {
				materialNeeded.text = "This creature\r\ncan't rank up";
			}
		}
	}

    /// <summary>
    /// Check if there's enough magic stuff to level up then spend that amount
    /// </summary>
    private void LevelUp() {
        if(manager.magicValue - stuff < 0) {
            NoMaterial(0, "level up");
            return;
        }

		plus.Play();
		if(manager.sfx == 1) {
			plus.GetComponent<AudioSource>().Play();
		}
        selectedCreature.LevelUp(true);
		manager.magicValue -= stuff;
		manager.creaturesOwned[currentFrame.positionInList].level = selectedCreature.details.level;
		manager.magicUsed += stuff;
		magicStuffText.text = Extensions.KFormat(manager.magicValue);
		manager.Save();

		DisplayLevelDetails();
	}

    private void Evolve() {
		if(manager.coinValue - coinsNeeded < 0) {
			NoMaterial(1, "");
			return;
		}

		manager.coinValue -= coinsNeeded;
        coinText.text = Extensions.KFormat(manager.coinValue);

        Instantiate(evolution);
		ReplaceEvolution(selectedCreature.details.ID);
		GameObject temp = Instantiate(selectedCreature.evolutionPrefab.gameObject, spawnPoint);
        Destroy(selectedCreature.gameObject);
        selectedCreature = temp.GetComponent<Creature>();
        temp.GetComponent<Animator>().SetTrigger("Special");
		++manager.evos;
		manager.Save();

		OnBack();
	}

	/// <summary>
	/// find the owned creature, replace its details with its evolution's details. Keeps the ID the same
	/// </summary>
	private void ReplaceEvolution(int ID) {
		for(int i = 0; i < manager.creaturesOwned.Count; i++) {
			if(ID == manager.creaturesOwned[i].ID) {
				manager.creaturesOwned[i].name = selectedCreature.evolutionPrefab.details.name;
				manager.creaturesOwned[i].level = 1;
				manager.creaturesOwned[i].rank = selectedCreature.evolutionPrefab.details.rank;
				manager.creaturesOwned[i].ranksIncreased = selectedCreature.evolutionPrefab.details.ranksIncreased;
				manager.creaturesOwned[i].xp = 0;
				manager.creaturesOwned[i].type = selectedCreature.evolutionPrefab.type;
				manager.creaturesOwned[i].canRankUp = selectedCreature.evolutionPrefab.details.canRankUp;
				break;
			}
		}
	}

    private void RankUp() {
        if(manager.magicValue - stuff < 0) {
            NoMaterial(0, "rank up");
            return;
        }

		rankUpArrow.GetComponent<ArrowUp>().top = false;
		rankUpArrow.SetActive(true);
		if(manager.sfx == 1) {
			rankUpArrow.GetComponent<AudioSource>().Play();
		}
		
		selectedCreature.RankUp(true);
		manager.magicValue -= stuff;
		manager.creaturesOwned[currentFrame.positionInList].level = 1;
		manager.creaturesOwned[currentFrame.positionInList].canRankUp = false;
		manager.creaturesOwned[currentFrame.positionInList].rank = selectedCreature.details.rank;
		manager.creaturesOwned[currentFrame.positionInList].ranksIncreased = selectedCreature.details.ranksIncreased;
		magicStuffText.text = Extensions.KFormat(manager.magicValue);
		++manager.rankUps;
		manager.Save();

		upgradeButton.interactable = false;
		SetCreatureDetails();
        SetMaterialNeed(3);
	}

    /// <summary>
    /// Type: 0 = magic stuff, 1 = coins
    /// </summary>
    /// <param name="type"></param>
    private void NoMaterial(int type, string levelOrRank) {
        if(type == 0) {
            if(manager.magicValue != 0) {
                purchase = false;
                noMaterialPopUpText.text = "<color=#ff2525><b>Not Enough Material!</b></color>\r\n\r\nYou don't have enough <color=#0094ff>magic stuff</color> to <color=black><i>" + levelOrRank + "</i></color> this creature. The <color=#0094ff>" + manager.magicValue.ToString() + "</color> that you have will be used to add XP to this creature.";
            } else {
                purchase = true;
				noMaterialPopUpText.text = "<color=#ff2525><b>Not Enough Material!</b></color>\r\n\r\nYou don't have enough <color=#0094ff>magic stuff</color> to <color=black><i>" + levelOrRank + "</i></color> this creature. Would you like to purchase some <color=#0094ff>magic stuff</color>?";
			}
        } else {
            purchase = true;
			noMaterialPopUpText.text = "<color=#ff2525><b>Not Enough Material!</b></color>\r\n\r\nYou don't have enough <color=#FFD100>coins</color> to <color=black><i>evolve</i></color> this creature. Would you like to purchase some <color=#FFD100>coins</color>?";
		}

		noMaterialPopUp.SetActive(true);
    }

    private void DisplayLevelDetails() {
		if(selectedCreature.details.level == selectedCreature.details.maxLevel) {
			upgradeButton.interactable = false;
			SetCreatureDetails();
		} else {
            upgradeButton.interactable = true;
			SetNewStats();
		}

		SetMaterialNeed(1);
	}

    /// <summary>
    /// Show stats along with what they will be upon upgrading
    /// </summary>
    private void SetNewStats() {
        hp.text = Extensions.KFormat((int)selectedCreature.details.baseHP) + " <color=#25efab>" + Extensions.KFormat(Mathf.CeilToInt(selectedCreature.baseHP + (selectedCreature.baseHP * (selectedCreature.details.level + 1) * 0.25f))) + "\u2191</color>";
        atk.text = Extensions.KFormat((int)selectedCreature.details.baseAtk) + " <color=#25efab>" + Extensions.KFormat(Mathf.CeilToInt(selectedCreature.baseAtk + (selectedCreature.baseAtk * (selectedCreature.details.level + 1) * 0.225f))) + "\u2191</color>";
        def.text = selectedCreature.details.baseDef.ToString() + " <color=#25efab>" + Mathf.Clamp(Mathf.Ceil(selectedCreature.baseDef + (selectedCreature.baseDef * (selectedCreature.details.level + 1) * 0.0065f)), 0, 85).ToString() + "\u2191</color>";
        spd.text = Extensions.KFormat((int)selectedCreature.details.baseSpd) + " <color=#25efab>" + Extensions.KFormat(Mathf.CeilToInt(selectedCreature.baseSpd + (selectedCreature.baseSpd * (selectedCreature.details.level + 1) * 0.075f))) + "\u2191</color>";
		crit.text = Mathf.Round(selectedCreature.details.baseCritChance * 100f).ToString() + "% <color=#25efab>" + Mathf.Round(Mathf.Clamp(selectedCreature.baseCritChance + (selectedCreature.baseCritChance * (selectedCreature.details.level + 1) * 0.01f), 0, 0.65f) * 100f).ToString() + "\u2191</color>";
		critDmg.text = Mathf.Round(selectedCreature.details.baseCritDamage * 100f).ToString() + "% <color=#25efab>" + Mathf.Round(Mathf.Clamp(selectedCreature.baseCritDamage + (selectedCreature.baseCritDamage * (selectedCreature.details.level + 1) * 0.1f), 0, 3.5f) * 100f).ToString() + "\u2191</color>";

        SetSameStats();
	}

	private void SetRankStats() {
		hp.text = Extensions.KFormat((int)selectedCreature.details.baseHP) + " <color=#25efab>" + Extensions.KFormat(Mathf.CeilToInt(selectedCreature.baseHP + (selectedCreature.baseHP * (selectedCreature.details.level + 1) * 0.35f))) + "\u2191</color>";
		atk.text = Extensions.KFormat((int)selectedCreature.details.baseAtk) + " <color=#25efab>" + Extensions.KFormat(Mathf.CeilToInt(selectedCreature.baseAtk + (selectedCreature.baseAtk * (selectedCreature.details.level + 1) * 0.325f))) + "\u2191</color>";
		def.text = selectedCreature.details.baseDef.ToString() + " <color=#25efab>" + Mathf.Clamp(Mathf.Ceil(selectedCreature.baseDef + (selectedCreature.baseDef * (selectedCreature.details.level + 1) * 0.0075f)), 0, 85).ToString() + "\u2191</color>";
		spd.text = Extensions.KFormat((int)selectedCreature.details.baseSpd) + " <color=#25efab>" + Extensions.KFormat(Mathf.CeilToInt(selectedCreature.baseSpd + (selectedCreature.baseSpd * (selectedCreature.details.level + 1) * 0.085f))) + "\u2191</color>";
		crit.text = Mathf.Round(selectedCreature.details.baseCritChance * 100f).ToString() + "% <color=#25efab>" + Mathf.Round(Mathf.Clamp(selectedCreature.baseCritChance + (selectedCreature.baseCritChance * (selectedCreature.details.level + 1) * 0.012f), 0, 0.65f) * 100f).ToString() + "\u2191</color>";
		critDmg.text = Mathf.Round(selectedCreature.details.baseCritDamage * 100f).ToString() + "% <color=#25efab>" + Mathf.Round(Mathf.Clamp(selectedCreature.baseCritDamage + (selectedCreature.baseCritDamage * (selectedCreature.details.level + 1) * 0.105f), 0, 3.5f) * 100f).ToString() + "\u2191</color>";

		SetSameStats();
	}

    private void SetEvoStats() {
		string arrow, color;
		int newHP = (int)selectedCreature.evolutionPrefab.details.baseHP;
		int newAtk = (int)selectedCreature.evolutionPrefab.details.baseAtk;
		int newDef = (int)selectedCreature.evolutionPrefab.details.baseDef;
		int newSpd = (int)selectedCreature.evolutionPrefab.details.baseSpd;
		float newCrit = selectedCreature.evolutionPrefab.details.baseCritChance;
		float newDmg = selectedCreature.evolutionPrefab.details.baseCritDamage;

		if(newHP > selectedCreature.details.baseHP) {
			arrow = "\u2191";
			color = "#25efab";
			hp.text = Extensions.KFormat((int)selectedCreature.details.baseHP) + " <color=" + color + ">" + Extensions.KFormat(newHP) + arrow + "</color>";
		} else {
			arrow = "\u2193";
			color = "red";
			hp.text = Extensions.KFormat((int)selectedCreature.details.baseHP) + " <color=" + color + ">" + Extensions.KFormat(newHP) + arrow + "</color>";
		}

		if(newAtk > selectedCreature.details.baseAtk) {
			arrow = "\u2191";
			color = "#25efab";
			atk.text = Extensions.KFormat((int)selectedCreature.details.baseAtk) + " <color=" + color + ">" + Extensions.KFormat(newAtk) + arrow + "</color>";
		} else {
			arrow = "\u2193";
			color = "red";
			atk.text = Extensions.KFormat((int)selectedCreature.details.baseAtk) + " <color=" + color + ">" + Extensions.KFormat(newAtk) + arrow + "</color>";
		}

		if(newDef > selectedCreature.details.baseDef) {
			arrow = "\u2191";
			color = "#25efab";
			def.text = selectedCreature.details.baseDef.ToString() + " <color=" + color + ">" + newDef.ToString() + arrow + "</color>";
		} else {
			arrow = "\u2193";
			color = "red";
			def.text = selectedCreature.details.baseDef.ToString() + " <color=" + color + ">" + newDef.ToString() + arrow + "</color>";
		}

		if(newSpd > selectedCreature.details.baseSpd) {
			arrow = "\u2191";
			color = "#25efab";
			spd.text = Extensions.KFormat((int)selectedCreature.details.baseSpd) + " <color=" + color + ">" + Extensions.KFormat(newSpd) + arrow + "</color>";
		} else {
			arrow = "\u2193";
			color = "red";
			spd.text = Extensions.KFormat((int)selectedCreature.details.baseSpd) + " <color=" + color + ">" + Extensions.KFormat(newSpd) + arrow + "</color>";
		}

		if(newCrit > selectedCreature.details.baseCritChance) {
			arrow = "\u2191";
			color = "#25efab";
			crit.text = Mathf.Round(selectedCreature.details.baseCritChance * 100f).ToString() + "% <color=" + color + ">" + Mathf.Round(newCrit * 100f).ToString() + arrow + "</color>";
		} else {
			arrow = "\u2193";
			color = "red";
			crit.text = Mathf.Round(selectedCreature.details.baseCritChance * 100f).ToString() + "% <color=" + color + ">" + Mathf.Round(newCrit * 100f).ToString() + arrow + "</color>";
		}

		if(newDmg > selectedCreature.details.baseCritDamage) {
			arrow = "\u2191";
			color = "#25efab";
			critDmg.text = Mathf.Round(selectedCreature.details.baseCritDamage * 100f).ToString() + "% <color=" + color + ">" + Mathf.Round(newDmg * 100f).ToString() + arrow + "</color>";
		} else {
			arrow = "\u2193";
			color = "red";
			critDmg.text = Mathf.Round(selectedCreature.details.baseCritDamage * 100f).ToString() + "% <color=" + color + ">" + Mathf.Round(newDmg * 100f).ToString() + arrow + "</color>";
		}
	}

	private void SetSameStats() {
		level.text = selectedCreature.details.level.ToString();

		if(selectedCreature.details.level == selectedCreature.details.maxLevel) {
			xp.text = "MAX LEVEL";
			xpBar.maxValue = 1;
			xpBar.value = 1;
		} else {
			xp.text = selectedCreature.details.xp.ToString() + "/" + selectedCreature.HowMuchXpIsNeeded().ToString();
			xpBar.maxValue = selectedCreature.HowMuchXpIsNeeded();
			xpBar.value = selectedCreature.details.xp;
		}
	}

	private void ReleaseCreature() {
		int multiplier = 1;

		if(selectedCreature.details.rank == 2)
			multiplier = 3;
		else if(selectedCreature.details.rank == 3)
			multiplier = 5;
		else if(selectedCreature.details.rank == 4)
			multiplier = 7;
		else if(selectedCreature.details.rank == 5)
			multiplier = 10;

		int value = selectedCreature.details.level * selectedCreature.details.rank * multiplier * 5;
		string name = "";

		for(int i = 0; i < manager.creaturesOwned.Count; i++) {
			if(selectedCreature.details.ID == manager.creaturesOwned[i].ID) {
				name = manager.creaturesOwned[i].name;
				manager.creaturesOwned.RemoveAt(i);
				break;
			}
		}

#if !UNITY_EDITOR
		Extensions.ShowAndroidToastMessage(name + " was released. You received " + value + " diamonds.");
#endif

		manager.diamondValue += value;
		manager.Save();

		confirmRelease = false;
		noMaterialPopUp.SetActive(false);
		GetComponent<Canvas>().enabled = false;
		ReInitialize();
	}

	#region Button Listeners
	public void NextTut() {
		Extensions.Click(manager, GetComponent<AudioSource>());

		++tut;
		if(tut < 19) {
			tutMessage.text = tutorial[tut];
		}

		if(tut == 19) {
			tutButton.onClick.RemoveAllListeners();
			PlayerPrefs.SetInt("Upgrade Tutorial", 1);
			PlayerPrefs.Save();
			tutWindow.SetActive(false);
		}
	}

	/// <summary>
	/// What to do when 'Upgrade' button is pressed
	/// </summary>
	public void OnUpgrade() {
		if(mainScreen) {
			if(selected) {
				mainScreen = false;
				purchase = false;

				creatureSelector.SetActive(false);
				upgradeButtons.SetActive(true);
				materialNeededObject.SetActive(true);
				release.gameObject.SetActive(true);

				OnLevelUp();
			}
        } else {
            if(leveling) {
                LevelUp();
			}
            if(evolving) {
                Evolve();
            }
            if(ranking) {
                RankUp();
            }
		}
		OnPopUpClose();
	}

    public void OnLevelUp() {
		OnPopUpClose();
		leveling = true;
		evolving = false;
		ranking = false;
		upgradeText.text = "LEVEL UP";

        levelUp.targetGraphic.color = new Color(0f, 0.55f, 1f);
        evolve.targetGraphic.color = Color.black;
        rankUp.targetGraphic.color = Color.black;

        DisplayLevelDetails();
	}

    public void OnEvolve() {
		OnPopUpClose();
		leveling = false;
		evolving = true;
		ranking = false;
		upgradeText.text = "Evolve";

		levelUp.targetGraphic.color = Color.black;
		evolve.targetGraphic.color = new Color(0f, 0.55f, 1f);
		rankUp.targetGraphic.color = Color.black;

        if(selectedCreature.details.level == selectedCreature.details.maxLevel && selectedCreature.details.canEvolve) {
            upgradeButton.interactable = true;
        } else {
            upgradeButton.interactable = false;
        }

		SetCreatureDetails();
		SetMaterialNeed(2);
	}

    public void OnRankUp() {
		OnPopUpClose();
		leveling = false;
		evolving = false;
		ranking = true;
		upgradeText.text = "rank up";

		levelUp.targetGraphic.color = Color.black;
		evolve.targetGraphic.color = Color.black;
		rankUp.targetGraphic.color = new Color(0f, 0.55f, 1f);

		if(selectedCreature.details.level == selectedCreature.details.maxLevel && selectedCreature.details.canRankUp) {
			upgradeButton.interactable = true;
		} else {
			upgradeButton.interactable = false;
		}

		SetCreatureDetails();
		SetMaterialNeed(3);
	}

    public void OnCancel() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		noMaterialPopUp.SetActive(false);
    }

    public void OnContinue() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		if(purchase) {
			OnPurchase(0);
		} else if(confirmRelease) {
			ReleaseCreature();
		} else {
			selectedCreature.GainXp(manager.magicValue, manager);
			manager.magicValue = 0;
			magicStuffText.text = manager.magicValue.ToString();
			SetMaterialNeed(1);
			SetSameStats();
		}

		noMaterialPopUp.SetActive(false);
    }

    public void OnPurchase(int window) {
		Extensions.Click(manager, GetComponent<AudioSource>());
		selectedCreature.GetComponent<CreatureInteraction>().enabled = false;
        purchaseWindow.GetComponent<Shop>().window = window;
        purchaseWindow.SetActive(true);
    }

    public void OnPurchaseClose() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		selectedCreature.GetComponent<CreatureInteraction>().enabled = true;
		purchaseWindow.SetActive(false);
    }

	public void OnRelease() {
		OnPopUpClose();

		if(manager.creaturesOwned.Count <= 3) {
			Extensions.ShowAndroidToastMessage("You can't have less than 3 creatures on your team.");
			return;
		}

		int multiplier = 1;

		if(selectedCreature.details.rank == 2)
			multiplier = 3;
		else if(selectedCreature.details.rank == 3)
			multiplier = 5;
		else if(selectedCreature.details.rank == 4)
			multiplier = 7;
		else if(selectedCreature.details.rank == 5)
			multiplier = 10;

		int value = selectedCreature.details.level * selectedCreature.details.rank * multiplier * 5;

		noMaterialPopUpText.text = "You will no longer be able to use this creature after releasing it. You will earn <color=#AACBE7><b>" + value.ToString() + " diamonds</b></color> for realsing this creature.\nAre you sure you want to release this creature?";
		confirmRelease = true;
		noMaterialPopUp.SetActive(true);
	}

    /// <summary>
    /// What to do when 'Back' button is pressed
    /// </summary>
    public void OnBack() {
		if(mainScreen) {
			Extensions.Click(manager, GetComponent<AudioSource>());
			SceneManager.LoadScene("Main Screen");
        } else {
            OnPopUpClose();
            mainScreen = true;

			release.gameObject.SetActive(false);
			currentFrame.SetFrame();
			creatureSelector.SetActive(true);
			upgradeButtons.SetActive(false);
			materialNeededObject.SetActive(false);

            upgradeText.text = "UPGRADE";
			upgradeButton.interactable = true;
			SetCreatureDetails();
		}
    }

    /// <summary>
    /// When a skill is selected, display a pop-up that describes what that skill is
    /// </summary>
    /// <param name="skillNumber">The number of the skill that got selected</param>
    public void OnSkillClick(int skillNumber) {
		Extensions.Click(manager, GetComponent<AudioSource>());
		new OnSkillClick().Display(selectedCreature, skillPopUp, skillDescription, skillNumber);
	}

    /// <summary>
    /// Close the pop-up when the close button is clicked
    /// </summary>
    public void OnPopUpClose() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		skillPopUp.SetActive(false);
    }
	#endregion
}