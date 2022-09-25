using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SummonScene : MonoBehaviour {
	#region Variables
	public List<string> tutorial;

	public AudioClip music;
	public Button back, cont, lesser, king, god, tutButton;
    public TextMeshProUGUI silverCount, solarCount, cosmicCount, coinsText, diamondsText, noMaterialPopUpText, tutMessage;
	public TextMeshProUGUI creatureName, hp, atk, def, spd, crit, critDmg, skillDescription;
	public Slider rankSlider;
	public Image typeIcon, skill1, skill2, skill3;
	public Image stone;
    public Sprite lesserSprite, kingSprite, godSprite;
	public Sprite damageClass, tankClass, healerClass;
	public GameObject spaceEcho, cosmicRays, godRay;
    public GameObject buttonPanel, detailsPanel, statPanel, skillPopUp, noMaterialPopUp, purchaseWindow, tutWindow;

	[Header("")]
	public Transform spawnPoint;

	public List<GameObject> rank1Prefabs, rank2Prefabs, rank3Prefabs, rank4Prefabs;

	private int type = 1, rank = 0, prevoiusPull = -1, tut;
	private GameObject summonedPrefab;
	private bool firstPull = true;
	private GameManager manager;
	#endregion

	void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		manager.GetComponent<CrossfadeAudiosource>().ChangeMusic(manager, music, 1f);

		silverCount.text = Extensions.KFormat(manager.silverValue);
		solarCount.text = Extensions.KFormat(manager.solarValue);
		cosmicCount.text = Extensions.KFormat(manager.cosmicValue);
		coinsText.text = Extensions.KFormat(manager.coinValue);
		diamondsText.text = Extensions.KFormat(manager.diamondValue);

		int tuto = PlayerPrefs.GetInt("Summon Tutorial", 0);
		if(tuto == 0) {
			tutMessage.text = tutorial[tut];
			tutWindow.SetActive(true);
			tutButton.onClick.AddListener(delegate { NextTut(); });
		}
	}

	private void ShowStats() {
		Creature creature = summonedPrefab.GetComponent<Creature>();

		detailsPanel.SetActive(true);
		statPanel.SetActive(true);
		cosmicRays.SetActive(true);
		cont.gameObject.SetActive(true);

		summonedPrefab.GetComponent<Animator>().SetTrigger("Special");

		new ShowStats().Display(creatureName, rankSlider, hp, atk, def, spd, crit, critDmg, creature, skill1, skill2, skill3, typeIcon, damageClass, tankClass, healerClass);
	}

    private void SelectRank() {
		int id = UnityEngine.Random.Range(1, 101);

		switch(type) {
			case 1:
			if(id <= 67) {
				rank = 1;
			} else if(id > 68 && id <= 95) {
				rank = 2;
			} else if(id > 95) {
				rank = 3;
			}
			break;
			case 2:
			if(id <= 65) {
				rank = 2;
			} else if(id > 66 && id <= 97) {
				rank = 3;
			} else if(id > 97) {
				rank = 4;
			}
			break;
			case 3:
			if(id <= 58) {
				rank = 3;
			} else if(id > 58) {
				rank = 4;
			}
			break;
		}

		Summon();
    }

	private void Summon() {
		int id = 0;
		switch(rank) {
			case 1:
			if(firstPull) {
				id = Random.Range(0, rank1Prefabs.Count);
				firstPull = false;
			} else {
				id = Random.Range(0, rank1Prefabs.Count);
				if(id == prevoiusPull) {
					id = GetRandom(rank1Prefabs.Count, id);
				}
			}
			prevoiusPull = id;
			summonedPrefab = Instantiate(rank1Prefabs[id], spawnPoint);
			break;

			case 2:
			if(firstPull) {
				id = Random.Range(0, rank2Prefabs.Count);
				firstPull = false;
			} else {
				id = Random.Range(0, rank2Prefabs.Count);
				if(id == prevoiusPull) {
					id = GetRandom(rank2Prefabs.Count, id);
				}
			}
			prevoiusPull = id;
			summonedPrefab = Instantiate(rank2Prefabs[id], spawnPoint);
			break;

			case 3:
			if(firstPull) {
				id = Random.Range(0, rank3Prefabs.Count);
				firstPull = false;
			} else {
				id = Random.Range(0, rank3Prefabs.Count);
				if(id == prevoiusPull) {
					id = GetRandom(rank3Prefabs.Count, id);
				}
			}
			prevoiusPull = id;
			summonedPrefab = Instantiate(rank3Prefabs[id], spawnPoint);
			break;

			case 4:
			if(firstPull) {
				id = Random.Range(0, rank4Prefabs.Count);
				firstPull = false;
			} else {
				id = Random.Range(0, rank4Prefabs.Count);
				if(id == prevoiusPull) {
					id = GetRandom(rank4Prefabs.Count, id);
				}
			}
			prevoiusPull = id;
			summonedPrefab = Instantiate(rank4Prefabs[id], spawnPoint);
			break;
		}

		SaveToOwned();
		Invoke("ShowStats", 2f);
	}

	private void SaveToOwned() {
		Creature creature = summonedPrefab.GetComponent<Creature>();
		manager.creaturesOwned.Add(new CreatureOwned(creature.details.name, manager.nextId, 1, creature.details.rank, 0, creature.details.ranksIncreased, creature.details.canRankUp, creature.type));
		++manager.nextId;
		++manager.summons;
		manager.Save();
	}

	/// <summary>
	/// Keep selecting a random number till the selected number isn't the same as the previous number
	/// </summary>
	/// <param name="maxRange"></param>
	/// <param name="currentValue"></param>
	/// <returns></returns>
	private int GetRandom(int maxRange, int currentValue) {
		int num = Random.Range(0, maxRange);
		if(num == currentValue) {
			num = GetRandom(maxRange, num);
		}
		return num;
	}

	private bool CheckAmount() {
		switch(type) {
			case 1:
			if(manager.silverValue - 1 < 0) {
				NoMaterial("Silver");
				return false;
			}

			silverCount.text = Extensions.KFormat(--manager.silverValue);

			break;
			case 2:
			if(manager.solarValue - 1 < 0) {
				NoMaterial("Solar");
				return false;
			}

			solarCount.text = Extensions.KFormat(--manager.solarValue);

			break;
			case 3:
			if(manager.cosmicValue - 1 < 0) {
				NoMaterial("Cosmic");
				return false;
			}

			cosmicCount.text = Extensions.KFormat(--manager.cosmicValue);

			break;
		}

		return true;
	}

	/// <summary>
	/// Type: 0 = magic stuff, 1 = coins
	/// </summary>
	/// <param name="type"></param>
	private void NoMaterial(string stoneType) {
		noMaterialPopUpText.text = "<color=#ff2525><b>Not Enough Stones!</b></color>\r\n\r\nYou don't have enough <color=black><i>" + stoneType + " Stones</i></color>. Would you like to purchase some?";
		noMaterialPopUp.SetActive(true);
	}

	#region Button Listeners
	public void NextTut() {
		Extensions.Click(manager, GetComponent<AudioSource>());

		++tut;
		if(tut < 10) {
			tutMessage.text = tutorial[tut];
		}

		if(tut == 10) {
			tutButton.onClick.RemoveAllListeners();
			PlayerPrefs.SetInt("Summon Tutorial", 1);
			PlayerPrefs.Save();
			tutWindow.SetActive(false);
		}
	}

	public void OnBack() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		SceneManager.LoadScene("Main Screen");
	}

	public void OnCancel() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		noMaterialPopUp.SetActive(false);
	}

	public void OnPurchase(int window) {
		Extensions.Click(manager, GetComponent<AudioSource>());
		purchaseWindow.GetComponent<Shop>().window = window;
		purchaseWindow.SetActive(true);
		noMaterialPopUp.SetActive(false);
	}

	public void OnPurchaseClose() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		purchaseWindow.SetActive(false);
	}

	public void OnSummon() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		bool enough = CheckAmount();

        if(enough) {
			buttonPanel.SetActive(false);
			stone.gameObject.SetActive(false);
			spaceEcho.SetActive(false);
			back.gameObject.SetActive(false);

			Instantiate(godRay);
			SelectRank();
		}
    }

    public void OnContinue() {
		buttonPanel.SetActive(true);
		stone.gameObject.SetActive(true);
		spaceEcho.SetActive(true);
        back.gameObject.SetActive(true);
        detailsPanel.SetActive(false);
        statPanel.SetActive(false);
        cont.gameObject.SetActive(false);
		cosmicRays.SetActive(false);

		Destroy(summonedPrefab);
		OnPopUpClose();
	}

    public void OnSelectStone(int type) {
		Extensions.Click(manager, GetComponent<AudioSource>());
		switch(type) {
            case 1: stone.sprite = lesserSprite;
			this.type = type;
			break;
            case 2: stone.sprite = kingSprite;
			this.type = type;
			break;
            case 3: stone.sprite = godSprite;
			this.type = type;
			break;
        }
    }

	/// <summary>
	/// When a skill is selected, display a pop-up that describes what that skill is
	/// </summary>
	/// <param name="skillNumber">The number of the skill that got selected</param>
	public void OnSkillClick(int skillNumber) {
		Extensions.Click(manager, GetComponent<AudioSource>());
		new OnSkillClick().Display(summonedPrefab.GetComponent<Creature>(), skillPopUp, skillDescription, skillNumber);
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