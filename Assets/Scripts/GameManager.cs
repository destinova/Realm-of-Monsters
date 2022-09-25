using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public int ticketValue, magicValue, diamondValue, coinValue;
	public int silverValue, solarValue, cosmicValue;
    public int floorReached = 1, selectedFloor = 1;
    public int magicUsed, summons, evos, rankUps;
    public bool doubleRewards;
    public int nextId = 0;

    //[HideInInspector]
	public List<CreatureOwned> creaturesOwned = new List<CreatureOwned>();
	[HideInInspector]
    public int qualitySetting, music, sfx;
    //[HideInInspector]
    public List<int> selectedTeamIDs;
    [HideInInspector]
    public int difficulty = 1;

    private int firstPlay = 1;
    private string saveFile;
    private GameData data = new GameData();

    private void Awake() {
        DontDestroyOnLoad(this);

		saveFile = Application.persistentDataPath + "/monsterSaveData.json";
	}

    void Start() {
        qualitySetting = PlayerPrefs.GetInt("Quality Setting", 1);
        music = PlayerPrefs.GetInt("Music", 1);
        sfx = PlayerPrefs.GetInt("SFX", 1);
        firstPlay = PlayerPrefs.GetInt("First Game", 1);

        Load();

		int firstSumComplete = PlayerPrefs.GetInt("First Summon Complete", 0);
		int firstBattleComplete = PlayerPrefs.GetInt("First Battle Complete", 0);
        if(firstSumComplete == 0) {
            LoadScene("Intro Scene");
        } else if(firstBattleComplete == 0) {
			LoadScene("First Battle");
		} else {
			LoadScene("Main Screen");
		}
    }

    private void LoadScene(string scene) {
		SceneManager.LoadSceneAsync(scene);
	}

    public void Load() {
        if(File.Exists(saveFile)) { 
            string content = File.ReadAllText(saveFile);

            data = JsonUtility.FromJson<GameData>(content);
        }

        ProcessData();
    }

    public void Save() {
        PlayerPrefs.SetInt("First Game", 0);
        PlayerPrefs.Save();

        data.floorReached = floorReached;
        data.tickets = ticketValue;
        data.coins = coinValue;
        data.magic = magicValue;
        data.diamonds = diamondValue;
        data.silverStones = silverValue;
        data.solarStones = solarValue;
        data.cosmicStones = cosmicValue;
        data.magicUsed = magicUsed;
        data.summons = summons;
        data.doubleRewards = doubleRewards;
        data.nextId = nextId;
        data.evos = evos;
        data.rankUps = rankUps;

		data.creaturesOwned = creaturesOwned;
        data.selectedTeamIDs = selectedTeamIDs;

        string content = JsonUtility.ToJson(data);
        File.WriteAllText(saveFile, content);
    }

    private void ProcessData() {
        if(firstPlay == 1) {
            floorReached = 0;
            ticketValue = 20;
            coinValue = 0;
            magicValue = 0;
            diamondValue = 480;
            silverValue = 5;
            solarValue = 2;
            cosmicValue = 1;
            magicUsed = 0;
            summons = 0;
            evos = 0;
            rankUps = 0;
			doubleRewards = false;
            nextId = 0;

        } else {
			floorReached = data.floorReached;
			ticketValue = data.tickets;
			coinValue = data.coins;
			magicValue = data.magic;
			diamondValue = data.diamonds;
			silverValue = data.silverStones;
			solarValue = data.solarStones;
            cosmicValue = data.cosmicStones;
            magicUsed = data.magicUsed;
            summons = data.summons;
            evos = data.evos;
            doubleRewards = data.doubleRewards;
            nextId = data.nextId;
            rankUps = data.rankUps;

			creaturesOwned = data.creaturesOwned;
			selectedTeamIDs = data.selectedTeamIDs;
		}

        selectedFloor = floorReached;
    }
}

[System.Serializable]
public class GameData {
    public int floorReached, tickets, coins, magic, diamonds, silverStones, solarStones, cosmicStones, magicUsed, summons, evos, rankUps, nextId;
    public bool doubleRewards;
    public List<CreatureOwned> creaturesOwned;
    public List<int> selectedTeamIDs;
}

[System.Serializable]
public class CreatureOwned {
    public int ID, level, rank, xp, ranksIncreased;
    public string name;
    public bool canRankUp;
    public Type type;

    public CreatureOwned() { }

    public CreatureOwned(string name, int ID, int level, int rank, int xp, int ranksIncreased, bool canRankUp, Type type) {
        this.name = name;
        this.ID = ID;
        this.level = level;
        this.rank = rank;
        this.xp = xp;
        this.ranksIncreased = ranksIncreased;
        this.canRankUp = canRankUp;
        this.type = type;
    }
}

/*
     public int ID { get; set; }
    public int level { get; set; }
    public int rank { get; set; }
    public int xp { get; set; }
    public int ranksIncreased { get; set; }
    public string name { get; set; }
    public bool canRankUp { get; set; }
    public Type type { get; set; }
 */