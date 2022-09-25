using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class LibraryScene : MonoBehaviour {
	#region Variables
	public List<string> tutorial;

	public AudioClip music;
	public GameObject skillPopUp, tutWindow;
	public Button tutButton;
	public TextMeshProUGUI creatureName, skillDescription, tutMessage;

	public Slider rank;
	public Image type, skill1, skill2, skill3;

	public Sprite damageClass, tankClass, healerClass;

	public Creature selectedCreature;
	public Transform spawnPoint;

	public BookSelector currentFrame;

	private GameManager manager;
	private int tut;
	#endregion

	void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
		manager.GetComponent<CrossfadeAudiosource>().ChangeMusic(manager, music, 0.9f);
		SetCreatureDetails();

		int tuto = PlayerPrefs.GetInt("Lib Tutorial", 0);
		if(tuto == 0) {
			tutMessage.text = tutorial[tut];
			tutWindow.SetActive(true);
			tutButton.onClick.AddListener(delegate { NextTut(); });
		}
	}

	public void SetCreatureDetails() {
		new ShowStats().Display(creatureName, rank, selectedCreature, skill1, skill2, skill3, type, damageClass, tankClass, healerClass);
	}

	#region Button Listeners
	public void OnBack() {
		Extensions.Click(manager, GetComponent<AudioSource>());
		SceneManager.LoadScene("Main Screen");
	}

	public void NextTut() {
		Extensions.Click(manager, GetComponent<AudioSource>());

		++tut;
		if(tut < 5) {
			tutMessage.text = tutorial[tut];
		}

		if(tut == 5) {
			tutButton.onClick.RemoveAllListeners();
			PlayerPrefs.SetInt("Lib Tutorial", 1);
			PlayerPrefs.Save();
			tutWindow.SetActive(false);
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