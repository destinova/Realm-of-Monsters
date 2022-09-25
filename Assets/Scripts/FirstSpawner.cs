using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FirstSpawner : MonoBehaviour {
	public FirstBattle fb;

    public List<Transform> spawnLocations;
	public List<Mob> waveMobs = new List<Mob>();

	private bool teamSpawned = false;

    public void Spawn() {
		if(!teamSpawned) {
			TeamSpawn();
			SetMobs(0, 3);
			fb.Initialize();
		} else {
			SetMobs(3, 6);
		}
    }

    private void TeamSpawn() {
        GameObject spawn;
		List<string> name = new List<string>();

		for(int i = 0; i < 3; i++) {
			for(int j = 0; j < fb.manager.creaturesOwned.Count; j++) {
				if(fb.manager.selectedTeamIDs[i] == fb.manager.creaturesOwned[j].ID) {
					name.Add(fb.manager.creaturesOwned[j].name);
					break;
				}
			}
		}

        for(int i = 0; i < 3; i++) {
			spawn = Instantiate(Resources.Load("Prefabs/" + name[i], typeof(GameObject)), spawnLocations[i]) as GameObject;
            spawn.GetComponent<Creature>().selector.SetActive(false);
            spawn.GetComponent<CreatureInteraction>().enabled = false;
            spawn.transform.localScale = new Vector3(3f, 3f, 3f);
			NewDetails(spawn.GetComponent<Creature>(), fb.manager.selectedTeamIDs[i]);
        }
    }

	private void NewDetails(Creature spawn, int ID) {
		for(int i = 0; i < fb.manager.creaturesOwned.Count; i++) {
			if(ID == fb.manager.creaturesOwned[i].ID) {
				spawn.positionInList = i;
				spawn.details.level = fb.manager.creaturesOwned[i].level;
				spawn.details.rank = fb.manager.creaturesOwned[i].rank;
				spawn.details.ranksIncreased = fb.manager.creaturesOwned[i].ranksIncreased;
				spawn.details.xp = fb.manager.creaturesOwned[i].xp;
				spawn.details.canRankUp = fb.manager.creaturesOwned[i].canRankUp;

				spawn.SetDetails();
				break;
			}	
		}

		teamSpawned = true;
	}

	private void SetMobs(int start, int finish) {
		int spawnpoint = 0;
		Creature enemy;
		for(int i = start; i < finish; i++) {
			enemy = Instantiate(Resources.Load("Prefabs/" + waveMobs[i].name, typeof(GameObject)), spawnLocations[spawnpoint + 3]).GetComponent<Creature>();
			enemy.tag = "Enemy";
			enemy.GetComponent<Creature>().info.transform.localRotation = new Quaternion(0, -180, 180, 0);
			enemy.GetComponent<Creature>().damage.transform.localPosition = new Vector3(0, 2, 1);
			enemy.GetComponent<Creature>().damage.transform.localRotation = new Quaternion(0, 180, 0, 0);
			enemy.GetComponent<Creature>().selector.SetActive(false);
			enemy.GetComponent<CreatureInteraction>().enabled = false;
			enemy.transform.localScale = new Vector3(3f, 3f, 3f);
			enemy.SetLevel(1);
			++spawnpoint;
		}
	}
}