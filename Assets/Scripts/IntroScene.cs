using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour {
	#region Variables
	public List<string> tutorial;
	[Header("")]
	public AudioClip music;
	public Button tutButton;
	public TextMeshProUGUI tutText;
	public TextMeshProUGUI creatureName, hp, atk, def, spd, crit, critDmg, skillDescription;
	public Slider rankSlider;
	public Image typeIcon, skill1, skill2, skill3;
	public Image stone;
	public Sprite damageClass, tankClass, healerClass;
	public GameObject spaceEcho, cosmicRays, godRay;
    public GameObject detailsPanel, statPanel, skillPopUp, tapToContinue, noticeWindow;
	public Transform spawnPoint;

	public List<GameObject> firstPrefabs;

	private int type = 1, rank = 0, tut = 0, pullNumber = 0;
	private GameObject summonedPrefab;
	private GameManager manager;
	#endregion

	void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		manager.GetComponent<CrossfadeAudiosource>().ChangeMusic(manager, music, 0.75f);

		tutButton.onClick.AddListener(delegate { NextTut(); });
		tutText.text = tutorial[tut];
	}

	private void ShowStats() {
		Creature creature = summonedPrefab.GetComponent<Creature>();

		detailsPanel.SetActive(true);
		statPanel.SetActive(true);
		cosmicRays.SetActive(true);

		summonedPrefab.GetComponent<Animator>().SetTrigger("Special");

		new ShowStats().Display(creatureName, rankSlider, hp, atk, def, spd, crit, critDmg, creature, skill1, skill2, skill3, typeIcon, damageClass, tankClass, healerClass);

		++tut;
		tutText.text = tutorial[tut];
		tapToContinue.SetActive(true);
		tutButton.interactable = true;
	}

	private void Summon() {		
		stone.gameObject.SetActive(false);
		spaceEcho.SetActive(false);
		summonedPrefab = Instantiate(firstPrefabs[pullNumber], spawnPoint);
		summonedPrefab.GetComponent<Creature>().info.SetActive(false);
		++pullNumber;

		SaveToOwned();
		Invoke("ShowStats", 2f);
	}

	private void SaveToOwned() {
		Creature creature = summonedPrefab.GetComponent<Creature>();
		manager.creaturesOwned.Add(new CreatureOwned(creature.details.name, manager.nextId, 1, creature.details.rank, 0, creature.details.ranksIncreased, creature.details.canRankUp, creature.type));
		manager.selectedTeamIDs.Add(manager.nextId);
		++manager.nextId;
		++manager.summons;
	}


	#region Button Listeners
	public void NextTut() {
		Extensions.Click(manager, GetComponent<AudioSource>());

		++tut;
		tutText.text = tutorial[tut];

		switch(tut) {
			case 2:
			tutButton.interactable = false;
			spaceEcho.SetActive(true);
			stone.gameObject.SetActive(true);
			tapToContinue.SetActive(false);
			break;
			case 20:
			Destroy(summonedPrefab);
			detailsPanel.SetActive(false);
			statPanel.SetActive(false);
			cosmicRays.SetActive(false);			
			break;
			case 21:
			stone.gameObject.SetActive(true);
			spaceEcho.SetActive(true);
			tutButton.interactable = false;
			tapToContinue.SetActive(false);
			break;
			case 24:
			Destroy(summonedPrefab);
			detailsPanel.SetActive(false);
			statPanel.SetActive(false);
			cosmicRays.SetActive(false);
			break;
			case 25:
			stone.gameObject.SetActive(true);
			spaceEcho.SetActive(true);
			tutButton.interactable = false;
			tapToContinue.SetActive(false);
			break;
			case 29:
			tutButton.onClick.RemoveAllListeners();
			tutButton.onClick.AddListener(delegate { OnFinish(); });
			tapToContinue.SetActive(false);
			break;
		}
	}

	public void OnContinue() {
		noticeWindow.SetActive(false);
	}

	public void OnFinish() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		manager.Save();

		PlayerPrefs.SetInt("First Summon Complete", 1);
		PlayerPrefs.Save();

		SceneManager.LoadScene("First Battle");
	}

	public void OnSummon() {
		Extensions.Click(manager, GetComponent<AudioSource>());

		stone.gameObject.SetActive(false);
		spaceEcho.SetActive(false);
		Instantiate(godRay);

		Summon();
    }
	#endregion
}