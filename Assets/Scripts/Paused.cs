using UnityEngine;

public class Paused : MonoBehaviour {
    public BattleManager bm;
    public BossBattle bb;
    public bool boss;

    public void OnResume() {
        if(boss) {
            bb.OnResume();
        } else {
            bm.OnResume();
		}
    }

    public void OnQuit() {
		if(boss) {
			bb.OnQuit();
		} else {
			bm.OnQuit();
		}
	}
}