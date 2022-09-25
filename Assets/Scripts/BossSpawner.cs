using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossSpawner : MonoBehaviour {
	public TextAsset bossList;
	public BossBattle bb;

    public List<Transform> spawnLocations;

	private GameObject spawn;
	private Bosses bosses = new Bosses();
	private Boss boss;
    public void Spawn() {
		bosses = JsonUtility.FromJson<Bosses>(bossList.text);
		TeamSpawn();
		GetFloorBoss();
	}

    private void TeamSpawn() {
        GameObject spawn;
		List<string> name = new List<string>();

		for(int i = 0; i < 3; i++) {
			for(int j = 0; j < bb.manager.creaturesOwned.Count; j++) {
				if(bb.manager.selectedTeamIDs[i] == bb.manager.creaturesOwned[j].ID) {
					name.Add(bb.manager.creaturesOwned[j].name);
					break;
				}
			}
		}

		for(int i = 0; i < 3; i++) {
			spawn = Instantiate(Resources.Load("Prefabs/" + name[i], typeof(GameObject)), spawnLocations[i]) as GameObject;
            spawn.GetComponent<Creature>().selector.SetActive(false);
            spawn.GetComponent<CreatureInteraction>().enabled = false;
            spawn.transform.localScale = new Vector3(3f, 3f, 3f);
			NewDetails(spawn.GetComponent<Creature>(), bb.manager.selectedTeamIDs[i]);
		}
    }

	private void NewDetails(Creature spawn, int ID) {
		for(int i = 0; i < bb.manager.creaturesOwned.Count; i++) {
			if(ID == bb.manager.creaturesOwned[i].ID) {
				spawn.positionInList = i;
				spawn.details.level = bb.manager.creaturesOwned[i].level;
				spawn.details.rank = bb.manager.creaturesOwned[i].rank;
				spawn.details.ranksIncreased = bb.manager.creaturesOwned[i].ranksIncreased;
				spawn.details.xp = bb.manager.creaturesOwned[i].xp;
				spawn.details.canRankUp = bb.manager.creaturesOwned[i].canRankUp;

				spawn.SetDetails();
				break;
			}
		}
	}

	private void GetFloorBoss() {
		foreach(Boss b in bosses.boss) {
			if(b.floor == bb.manager.selectedFloor)
				boss = b;

			if(b.floor > bb.manager.selectedFloor)
				break;
		}

		SetBoss();
	}

    private void SetBoss() {
		spawn = Instantiate(Resources.Load("Prefabs/" + boss.name, typeof(GameObject)), spawnLocations[3]) as GameObject;
		spawn.GetComponent<Rigidbody>().isKinematic = true;
		spawn.tag = "Enemy";
		spawn.GetComponent<Creature>().info.transform.localRotation = new Quaternion(0, -180, 180, 0);
		spawn.GetComponent<Creature>().damage.transform.localPosition = new Vector3(0, boss.y, boss.z);
		spawn.GetComponent<Creature>().damage.GetComponentInChildren<TextMeshProUGUI>().fontSize = boss.fontsize;
		spawn.GetComponent<Creature>().damage.transform.localRotation = new Quaternion(0, 180, 0, 0);
		spawn.GetComponent<Creature>().selector.SetActive(false);
		spawn.GetComponent<CreatureInteraction>().enabled = false;
        spawn.transform.localScale = new Vector3(boss.scale, boss.scale, boss.scale);
		spawn.transform.localPosition = new Vector3(0, boss.position, 0);
		spawn.GetComponent<CapsuleCollider>().radius = 0.1f;

        Invoke("MakeBoss", 0.25f);
	}

    private void MakeBoss() {
		int diff = bb.manager.difficulty;
		int levelSet;

		switch(diff) {
			case 1:
			levelSet = boss.level;
			break;
			case 2:
			levelSet = boss.level + 2;
			break;
			case 3:
			levelSet = boss.level + 5;
			break;
			default:
			levelSet = 1;
			break;
		}

		spawn.GetComponent<Creature>().SetLevel(levelSet);
		spawn.GetComponent<Creature>().Bossify();

		bb.bossBar.maxValue = spawn.GetComponent<Creature>().baseHP;
		bb.bossBar.value = spawn.GetComponent<Creature>().baseHP;
		bb.bossHealth.text = Extensions.KFormat(Mathf.CeilToInt(spawn.GetComponent<Creature>().baseHP));

		bb.Initialize();
	}
}