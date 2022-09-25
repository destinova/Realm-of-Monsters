using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class WorldMap : MonoBehaviour {
    public List<string> tutorial;

	public AudioClip music;
    public TextMeshProUGUI level, desc, tickets, tutMessage;
    public Button next, prev, tutButton;
    public Toggle n, h, e;
    public GameObject tutWindow;

    private GameManager manager;
    private int currentFloor, tut;
    void Start() {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		manager.GetComponent<CrossfadeAudiosource>().ChangeMusic(manager, music, 1f);

		currentFloor = manager.floorReached;
        manager.selectedFloor = currentFloor;
        level.text = currentFloor.ToString();
        tickets.text = Extensions.KFormat(manager.ticketValue);

        if(manager.floorReached == 1) {
			prev.gameObject.SetActive(false);
            next.gameObject.SetActive(false);
		} else if(currentFloor == manager.floorReached)
            next.gameObject.SetActive(false);

        switch(manager.difficulty) {
            case 1:
            n.isOn = true;
            break;
			case 2:
            h.isOn = true;
            break;
			case 3:
            e.isOn = true;
			break;
		}

		int tuto = PlayerPrefs.GetInt("World Tutorial", 0);
		if(tuto == 0) {
			tutMessage.text = tutorial[tut];
			tutWindow.SetActive(true);
			tutButton.onClick.AddListener(delegate { NextTut(); });
		}
	}

	#region Button Listeners
	public void NextTut() {
		Extensions.Click(manager, GetComponent<AudioSource>());

		++tut;
		if(tut < 9) {
			tutMessage.text = tutorial[tut];
		}

		if(tut == 9) {
			tutButton.onClick.RemoveAllListeners();
			PlayerPrefs.SetInt("World Tutorial", 1);
			PlayerPrefs.Save();
			tutWindow.SetActive(false);
		}
	}

	public void OnChangeDifficulty(int i) {
		Extensions.Click(manager, GetComponent<AudioSource>());
		manager.difficulty = i;

        switch(i) {
            case 1:
            desc.text = "<color=#00ff6a><b>Normal Difficulty</b></color>\n\nAll creatures are at their base levels\n\nAll rewards are at their base amount\n\nTickets required: <color=#A26EAE><size=64><b>1</size></color>";
			break;
            case 2:
			desc.text = "<color=#ffc800><b>Hard Difficulty</b></color>\n\nAll creatures are <color=#ffc800><b>2</b></color> levels higher\n\nAll rewards are increased by <color=#ffc800><b>35%</b></color>\n\nTickets required: <color=#A26EAE><size=64><b>2</size></color>";
			break;
            case 3:
			desc.text = "<color=#ff0000><b>Extreme Difficulty</b></color>\n\nAll creatures are <color=#ff0000><b>5</b></color> levels higher\n\nAll rewards are increased by <color=#ff0000><b>100%</b></color>\n\nTickets required: <color=#A26EAE><size=64><b>3</size></color>";
			break;
        }
    }

    public void OnNext() {
		Extensions.Click(manager, GetComponent<AudioSource>());

		if(currentFloor != manager.floorReached) {
            ++currentFloor;
			level.text = currentFloor.ToString();
            manager.selectedFloor = currentFloor;
            if(currentFloor == manager.floorReached) {
                next.gameObject.SetActive(false);
            }

            if(currentFloor != 1) {
                prev.gameObject.SetActive(true);
            }
		}
    }

	public void OnPrev() {
		Extensions.Click(manager, GetComponent<AudioSource>());

		if(currentFloor != 1) {
			--currentFloor;
			level.text = currentFloor.ToString();
			manager.selectedFloor = currentFloor;
			if(currentFloor == 1) {
				prev.gameObject.SetActive(false);
			}

			if(currentFloor != manager.floorReached) {
				next.gameObject.SetActive(true);
			}
		}
	}

	public void OnSelectTeam() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		SceneManager.LoadScene("Team Selection");
    }

    public void OnBack() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		SceneManager.LoadScene("Main Screen");
    }
	#endregion
}