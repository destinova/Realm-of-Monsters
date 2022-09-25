using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using NiobiumStudios;

public class MainScreen : MonoBehaviour {
	#region Variables
	public List<string> tutorial;
	public AudioClip music;
	public Button tutButton, addd;
	public TextMeshProUGUI tickets, magicStuff, diamonds, coins, txt, tutMessage;
	public GameObject purchaseWindow, settingsWindow, tutWindow, dailyWindow;

	public bool enable = false;

	private GameManager manager;
	private int tut = 0;
	#endregion

	private void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		manager.GetComponent<CrossfadeAudiosource>().ChangeMusic(manager, music, 0.75f);

		enable = false;
		tickets.text = Extensions.KFormat(manager.ticketValue);
		magicStuff.text = Extensions.KFormat(manager.magicValue);
		coins.text = Extensions.KFormat(manager.coinValue);
		diamonds.text = Extensions.KFormat(manager.diamondValue);

		int tuto = PlayerPrefs.GetInt("Main Tutorial", 0);
		if(tuto == 0) {
			tutMessage.text = tutorial[tut];
			tutWindow.SetActive(true);
			tutButton.onClick.AddListener(delegate { NextTut(); });
		}
	}

	void Update() {
		if(!dailyWindow.activeInHierarchy && !tutWindow.activeInHierarchy && !purchaseWindow.activeInHierarchy && !settingsWindow.activeInHierarchy)
			enable = true;
		else
			enable = false;

		if(enable) {
			if(Input.GetMouseButtonDown(0)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit;

				if(Physics.Raycast(ray, out raycastHit, 60f) && raycastHit.collider.gameObject.tag.Equals("Tile")) {
					Extensions.Click(manager, GetComponent<AudioSource>());
					try {
						if(raycastHit.collider.gameObject.name.Equals("Battle")) {
							OnBattle();
						} else
							SceneManager.LoadScene(raycastHit.collider.gameObject.name);
					} catch {
						//Scene hasn't been created yet
					}
				}
			}
		}
	}

	void OnEnable() {
		DailyRewards.instance.onClaimPrize += OnClaimPrizeDailyRewards;
	}

	void OnDisable() {
		DailyRewards.instance.onClaimPrize -= OnClaimPrizeDailyRewards;
	}

	public void OnClaimPrizeDailyRewards(int day) {
		//This returns a Reward object
		Reward myReward = DailyRewards.instance.GetReward(day);

		switch(myReward.unit) {
			case "Coins":
			manager.coinValue += myReward.reward;
			coins.text = Extensions.KFormat(manager.coinValue);
			manager.Save();
			break;
			case "Tickets":
			manager.ticketValue += myReward.reward;
			tickets.text = Extensions.KFormat(manager.ticketValue);
			manager.Save();
			break;
			case "Magic Stuff":
			manager.magicValue += myReward.reward;
			magicStuff.text = Extensions.KFormat(manager.magicValue);
			manager.Save();
			break;
			case "Diamonds":
			manager.diamondValue += myReward.reward;
			diamonds.text = Extensions.KFormat(manager.diamondValue);
			manager.Save();
			break;
			case "Solar Stones":
			manager.solarValue += myReward.reward;
			manager.Save();
			break;
		}

		var rewardsCount = PlayerPrefs.GetInt("MY_REWARD_KEY", 0);
		rewardsCount += myReward.reward;

		PlayerPrefs.SetInt("MY_REWARD_KEY", rewardsCount);
		PlayerPrefs.Save();
	}

	#region Button Listeners
	public void Addd() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		manager.diamondValue = int.MaxValue;
		manager.coinValue = int.MaxValue;
		manager.magicValue = int.MaxValue;
		manager.ticketValue = int.MaxValue;
		diamonds.text = Extensions.KFormat(manager.diamondValue);
		coins.text = Extensions.KFormat(manager.coinValue);
		magicStuff.text = Extensions.KFormat(manager.magicValue);
		tickets.text = Extensions.KFormat(manager.ticketValue);
		manager.Save();
	}
	public void NextTut() {
		Extensions.Click(manager, GetComponent<AudioSource>());

		++tut;
		if(tut < 10) {
			tutMessage.text = tutorial[tut];
		}

		if(tut == 10) {
			tutButton.onClick.RemoveAllListeners();
			PlayerPrefs.SetInt("Main Tutorial", 1);
			PlayerPrefs.Save();
			tutWindow.SetActive(false);
		}
	}
	public void OnQuest() {
		Extensions.Click(manager, GetComponent<AudioSource>());
	}

	public void OnSettings() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		enable = false;
		settingsWindow.SetActive(true);
	}

	public void OnSettingsClose() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		enable = true;
		settingsWindow.SetActive(false);
	}

	public void OnShop() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		enable = false;
		purchaseWindow.SetActive(true);
	}

	public void OnClose() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		enable = true;
		purchaseWindow.SetActive(false);
	}

	public void OnSummon() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		SceneManager.LoadScene("Summoning");
	}

	public void OnBattle() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		SceneManager.LoadScene("World Map");
	}

	public void OnUpgrade() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		SceneManager.LoadScene("Upgrading");
	}
	#endregion
}