using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TeamSelection : MonoBehaviour {
	public List<string> tutorial;
	public TextMeshProUGUI tickets, coinsText, tutMessage;
	public Button start, tutButton;
	public GameObject popUp, shop, tutWindow;
	public List<Transform> spawnLocations;
	public Transform listView;
	public GameObject framePrefab, dust;
	public int activeMembers = 0;

	[HideInInspector]
	public List<TeamSelector> frames = new List<TeamSelector>();
	[HideInInspector]
	public List<GameObject> selectedTeam = new List<GameObject>(3);
	[HideInInspector]
	public List<bool> positions = new List<bool>(3);
	[HideInInspector]
	public  GameManager manager;

	private bool shopOpen = false;
	private int tut;
	private void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		tickets.text = Extensions.KFormat(manager.ticketValue);
		coinsText.text = Extensions.KFormat(manager.coinValue);

		int tuto = PlayerPrefs.GetInt("Selection Tutorial", 0);
		if(tuto == 0) {
			tutMessage.text = tutorial[tut];
			tutWindow.SetActive(true);
			tutButton.onClick.AddListener(delegate { NextTut(); });
		}

		Initialize();
	}

	private void Update() {
		if(Input.GetMouseButtonDown(0)) {
			if(!shopOpen) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit;

				if(Physics.Raycast(ray, out raycastHit, 60f) && raycastHit.collider.gameObject.tag.Equals("Monster")) {
					//Click();
					foreach(GameObject o in selectedTeam) {
						if(o == raycastHit.collider.gameObject) {
							FindFrame(raycastHit.collider.gameObject.GetComponent<Creature>().positionInTeam);
							break;
						}
					}
				}
			}
		}
	}

	private void Initialize() {
		GameObject f;

		for(int i = 0; i < 3; i++) {
			positions[i] = false;
		}

		for(int i = 0; i < manager.creaturesOwned.Count; i++) {
			f = Instantiate(framePrefab, listView);
			frames.Add(f.GetComponent<TeamSelector>());
			frames[i].scene = this;
			frames[i].ID = i;
			frames[i].positionInList = manager.creaturesOwned[i].ID;
			frames[i].SetFrame();
			for(int j = 0; j < 3; j++) {
				if(frames[i].positionInList.Equals(manager.selectedTeamIDs[j])) {
					frames[i].Selected();
					break;
				}
			}
		}

		listView.GetComponent<RectTransform>().sizeDelta = new Vector2(manager.creaturesOwned.Count * 256f, 150f);
	}

	private void FindFrame(int pos) {
		TeamSelector frame = new TeamSelector();

		foreach(TeamSelector t in frames) {
			if(t.positionInTeam == pos) {
				frame = t;
				break;
			}
		}
		frame.Unselect();
	}

	public void TeamMemberPressed(TeamSelector frame) {
		Extensions.Click(manager, GetComponent<AudioSource>());
		int pos = frame.positionInTeam;
		frame.positionInTeam = -1;
		start.interactable = false;
		--activeMembers;
		positions[pos] = false;
		selectedTeam[pos].GetComponent<Creature>().positionInTeam = -1;
		Destroy(selectedTeam[pos]);
		selectedTeam[pos] = null;
	}

	private void NotEnough() {
		Click();
		shopOpen = true;
		popUp.SetActive(true);
	}

	#region Button Listeners
	public void NextTut() {
		Extensions.Click(manager, GetComponent<AudioSource>());

		++tut;
		if(tut < 8) {
			tutMessage.text = tutorial[tut];
		}

		if(tut == 8) {
			tutButton.onClick.RemoveAllListeners();
			PlayerPrefs.SetInt("Selection Tutorial", 1);
			PlayerPrefs.Save();
			tutWindow.SetActive(false);
		}
	}

	public void OnBack() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		SceneManager.LoadScene("World Map");
	}

	public void OnStart() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		if(manager.ticketValue < 1 * manager.difficulty) {
			NotEnough();
			return;
		} else {
			manager.ticketValue -= 1 * manager.difficulty;
			tickets.text = Extensions.KFormat(manager.ticketValue);
		}

		for(int i = 0; i < 3; i++) {
			manager.selectedTeamIDs.RemoveAt(i);
			manager.selectedTeamIDs.Insert(i, selectedTeam[i].GetComponent<Creature>().details.ID);
		}

		manager.Save();

		if(manager.selectedFloor % 5 == 0) {
			SceneManager.LoadScene("Boss Battle");
		} else {
			SceneManager.LoadScene("Battle");
		}
	}

	public void Click() {
		Extensions.Click(manager, GetComponent<AudioSource>());
	}

	public void OnCancel() {
		Click();
		popUp.SetActive(false);
		shop.SetActive(false);
		shopOpen = false;
	}

	public void OnPurchase() {
		Click();
		popUp.SetActive(false);
		shop.SetActive(true);
		shopOpen = true;
	}
	#endregion
}