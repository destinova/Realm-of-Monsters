using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class VictoryPanel : MonoBehaviour {
	public TextAsset rewardList;
	public AudioClip music, win, los;
	public AudioSource chime;
	public FirstBattle fb;
	public BattleManager bm;
	public BossBattle bb;
    public GameObject victory, loss;
	public TextMeshProUGUI coinAward, magicAward;
	public bool boss, first;

	[Header("First creature")]
    public Image creature1;
    public GameObject lvldUp1;
    public Slider xpBar1;
    public TextMeshProUGUI lvl1, xp1;

	[Header("Second creature")]
	public Image creature2;
	public GameObject lvldUp2;
	public Slider xpBar2;
	public TextMeshProUGUI lvl2, xp2;

	[Header("Third creature")]
	public Image creature3;
	public GameObject lvldUp3;
	public Slider xpBar3;
	public TextMeshProUGUI lvl3, xp3;

	[HideInInspector]
    public bool won;
    [HideInInspector]
    public List<GameObject> creatures;

    private int coins, xp;
	private float magic;
	private float rewardMultiplier = 1f;
	private RewardsList rewards = new RewardsList();
	private GameManager manager;
    public void Set() {
		rewards = JsonUtility.FromJson<RewardsList>(rewardList.text);

		if(first)
			manager = fb.manager;

		if(boss) {
			manager = bb.manager;
		} else if(!boss && !first) {
			manager = bm.manager;
		}

		manager.GetComponent<CrossfadeAudiosource>().ChangeMusic(manager, music, 0.85f);

		DifficultyMultiplier();
		GetRewards();

        if(won) {
			chime.clip = win;
			victory.SetActive(true);
            Won();
        } else {
			chime.clip = los;
			chime.volume = 0.5f;
			loss.SetActive(true);
            Lost();
        }

        SetFrames();

		if(first) {
			PlayerPrefs.SetInt("First Battle Complete", 1);
			PlayerPrefs.Save();
		}

		manager.Save();
    }

	private void OnEnable() {
		if(manager.sfx == 1) {
			chime.Play();
		}
	}

	private void DifficultyMultiplier() {
		float doubler = 1f;
		if(manager.doubleRewards)
			doubler = 2f;

		switch(manager.difficulty) {
			case 1:
			rewardMultiplier = 1f * doubler;
			break;
			case 2:
			rewardMultiplier = 1.35f * doubler;
			break;
			case 3:
			rewardMultiplier = 2f * doubler;
			break;
		}
	}

	private void GetRewards() {
		foreach(Rewards r in rewards.rewards) {
			if(r.floor == manager.selectedFloor) {
				coins = r.coins;
				magic = r.magic;
				xp = r.xp;
				break;
			}
		}
	}

    private void SetFrames() {
        creature1.sprite = creatures[0].GetComponent<Creature>().sprite;
		ShowLeveledUp(1);
        lvl1.text = creatures[0].GetComponent<Creature>().details.level.ToString();

		creature2.sprite = creatures[1].GetComponent<Creature>().sprite;
		ShowLeveledUp(2);
		lvl2.text = creatures[1].GetComponent<Creature>().details.level.ToString();

		creature3.sprite = creatures[2].GetComponent<Creature>().sprite;
		ShowLeveledUp(3);
		lvl3.text = creatures[2].GetComponent<Creature>().details.level.ToString();
	}

    private void ShowLeveledUp(int i) {
        if(i == 1) {
			if(creatures[i - 1].GetComponent<Creature>().details.level == creatures[i - 1].GetComponent<Creature>().details.maxLevel) {
				xp1.text = "MAX LVL";
			} else {
				if(creatures[i - 1].GetComponent<Creature>().GainBattleXp(xp, manager)) {
					lvldUp1.SetActive(true);
				}
				xp1.text = string.Concat("+", Extensions.KFormat(Mathf.FloorToInt(creatures[i - 1].GetComponent<Creature>().details.xpMultiplier * xp)), " XP");
				xpBar1.maxValue = creatures[i - 1].GetComponent<Creature>().HowMuchXpIsNeeded();
				xpBar1.value = creatures[i - 1].GetComponent<Creature>().details.xp;
			}
		} else if(i == 2) {
			if(creatures[i - 1].GetComponent<Creature>().details.level == creatures[i - 1].GetComponent<Creature>().details.maxLevel) {
				xp2.text = "MAX LVL";
			} else {
				if(creatures[i - 1].GetComponent<Creature>().GainBattleXp(xp, manager)) {
					lvldUp2.SetActive(true);
				}
				xp2.text = string.Concat("+", Extensions.KFormat(Mathf.FloorToInt(creatures[i - 1].GetComponent<Creature>().details.xpMultiplier * xp)), " XP");
				xpBar2.maxValue = creatures[i - 1].GetComponent<Creature>().HowMuchXpIsNeeded();
				xpBar2.value = creatures[i - 1].GetComponent<Creature>().details.xp;
			}
		} else if(i == 3) {
			if(creatures[i - 1].GetComponent<Creature>().details.level == creatures[i - 1].GetComponent<Creature>().details.maxLevel) {
				xp3.text = "MAX LVL";
			} else {
				if(creatures[i - 1].GetComponent<Creature>().GainBattleXp(xp, manager)) {
					lvldUp3.SetActive(true);
				}
				xp3.text = string.Concat("+", Extensions.KFormat(Mathf.FloorToInt(creatures[i - 1].GetComponent<Creature>().details.xpMultiplier * xp)), " XP");
				xpBar3.maxValue = creatures[i - 1].GetComponent<Creature>().HowMuchXpIsNeeded();
				xpBar3.value = creatures[i - 1].GetComponent<Creature>().details.xp;
			}
		}
	}

    private void Won() {
		int wave = 1;

		if(boss) {
			wave = 5;
		} else if(!boss && !first) {
			wave = bm.wave;
		}

		coins = Mathf.CeilToInt(coins * wave * rewardMultiplier);
		magic = Mathf.Round(magic * wave * rewardMultiplier);
		xp = Mathf.CeilToInt(xp * wave * rewardMultiplier);

		manager.coinValue += coins;
		manager.magicValue += (int)magic;

		if(manager.selectedFloor == manager.floorReached)
			manager.floorReached = Mathf.Clamp(manager.floorReached + 1, 0, 15);

		magicAward.text = magic.ToString();
		coinAward.text = Extensions.KFormat(coins);
	}

    private void Lost() {
		int wave = 1;

		if(boss) {
			wave = 2;
		} else if(!boss && !first) {
			wave = bm.wave;
		}

		coins = Mathf.CeilToInt(coins * (wave - 1) * rewardMultiplier);
		magic = Mathf.Round(magic * (wave - 1) * rewardMultiplier);
		xp = Mathf.CeilToInt(xp * (wave - 1) * rewardMultiplier);

		manager.coinValue += coins;
		manager.magicValue += (int)magic;

		magicAward.text = magic.ToString();
		coinAward.text = Extensions.KFormat(coins);
	}

    public void OnTapToContinue() {
		if(boss) {
			Extensions.Click(manager, bb.GetComponent<AudioSource>());
		} else if(!boss && !first) {
			Extensions.Click(manager, bm.GetComponent<AudioSource>());
		}

		if(first) {
			Extensions.Click(manager, fb.GetComponent<AudioSource>());
			SceneManager.LoadScene("Main Screen");
		} else {
			SceneManager.LoadScene("World Map");
		}
    }
}