using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour {
	public TextAsset mobList;
	public BattleManager bm;

    public List<Transform> spawnLocations;

    private bool teamSpawned = false;
    private List<Creature> enemies = new List<Creature>();
	private Mobs mobs = new Mobs();
	private List<Mob> floorMobs = new List<Mob>();
	private List<Mob> waveMobs = new List<Mob>();

    public void Spawn() {
		if(!teamSpawned) {
			mobs = JsonUtility.FromJson<Mobs>(mobList.text);
			TeamSpawn();
			GetFloorMobs();
			GetWave();
			bm.Initialize();
		} else {
			GetWave();
		}
    }

    private void TeamSpawn() {
        GameObject spawn;
		List<string> name = new List<string>();

		for(int i = 0; i < 3; i++) {
			for(int j = 0; j < bm.manager.creaturesOwned.Count; j++) {
				if(bm.manager.selectedTeamIDs[i] == bm.manager.creaturesOwned[j].ID) {
					name.Add(bm.manager.creaturesOwned[j].name);
					break;
				}
			}
		}

        for(int i = 0; i < 3; i++) {
			spawn = Instantiate(Resources.Load("Prefabs/" + name[i], typeof(GameObject)), spawnLocations[i]) as GameObject;
            spawn.GetComponent<Creature>().selector.SetActive(false);
            spawn.GetComponent<CreatureInteraction>().enabled = false;
            spawn.transform.localScale = new Vector3(3f, 3f, 3f);
			NewDetails(spawn.GetComponent<Creature>(), bm.manager.selectedTeamIDs[i]);
        }
    }

	private void NewDetails(Creature spawn, int ID) {
		for(int i = 0; i < bm.manager.creaturesOwned.Count; i++) {
			if(ID == bm.manager.creaturesOwned[i].ID) {
				spawn.positionInList = i;
				spawn.details.level = bm.manager.creaturesOwned[i].level;
				spawn.details.rank = bm.manager.creaturesOwned[i].rank;
				spawn.details.ranksIncreased = bm.manager.creaturesOwned[i].ranksIncreased;
				spawn.details.xp = bm.manager.creaturesOwned[i].xp;
				spawn.details.canRankUp = bm.manager.creaturesOwned[i].canRankUp;

				spawn.SetDetails();
				break;
			}	
		}
	}

	private void GetFloorMobs() {
		foreach(Mob v in mobs.mob) {
			if(v.floor == bm.manager.selectedFloor)
				floorMobs.Add(v);

			if(v.floor > bm.manager.selectedFloor)
				break;
		}

		teamSpawned = true;
	}

	private void GetWave() {
		for(int i = 0; i < floorMobs.Count; i++) {
			if(floorMobs[i].wave == bm.wave) {
				waveMobs.Add(floorMobs[i]);
			}
		}
		SetMobs();
	}

	private void SetMobs() {
		for(int i = 0; i < waveMobs.Count; i++) {
			enemies.Add(Instantiate(Resources.Load("Prefabs/" + waveMobs[i].name, typeof(GameObject)), spawnLocations[i + 3]).GetComponent<Creature>());
			enemies[i].tag = "Enemy";
			enemies[i].GetComponent<Creature>().info.transform.localRotation = new Quaternion(0, -180, 180, 0);
			enemies[i].GetComponent<Creature>().damage.transform.localPosition = new Vector3(0, 2, 1);
			enemies[i].GetComponent<Creature>().damage.transform.localRotation = new Quaternion(0, 180, 0, 0);
			enemies[i].GetComponent<Creature>().selector.SetActive(false);
			enemies[i].GetComponent<CreatureInteraction>().enabled = false;
			enemies[i].transform.localScale = new Vector3(3f, 3f, 3f);
		}

		Invoke("SetLevels", 0.25f);
		
	}

    private void SetLevels() { 
        int levelSet;
		int diff = bm.manager.difficulty;

		for(int i = 0; i < 3; i++) {
			switch(diff) {
				case 1:
				levelSet = waveMobs[i].level;
				break;
				case 2:
				levelSet = waveMobs[i].level + 2;
				break;
				case 3:
				levelSet = waveMobs[i].level + 5;
				break;
				default:
				levelSet = 1;
				break;
			}

			enemies[i].SetLevel(levelSet);
		}

		waveMobs.Clear();
		enemies.Clear();
	}
}