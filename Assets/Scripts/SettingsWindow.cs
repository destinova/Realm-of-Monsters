using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour {
    public MainScreen main;
    public Toggle music, sfx;
    public Toggle p, q;
    
    private GameManager manager;
    private void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        switch(manager.qualitySetting) {
            case 0:
            p.SetIsOnWithoutNotify(true);
            break;
            case 1:
            q.SetIsOnWithoutNotify(true);
            break;
        }

        if(manager.music == 1) {
            music.SetIsOnWithoutNotify(true);
        } else
            music.SetIsOnWithoutNotify(false);

        if(manager.sfx == 1) {
            sfx.SetIsOnWithoutNotify(true);
        } else
            sfx.SetIsOnWithoutNotify(false);
    }

    public void SetQuality(int setting) {
		Extensions.Click(manager, main.GetComponent<AudioSource>());

		QualitySettings.SetQualityLevel(setting);
        PlayerPrefs.SetInt("Quality Setting", setting);
        PlayerPrefs.Save();
        manager.qualitySetting = setting;
    }

    public void SetMusic() {
		Extensions.Click(manager, main.GetComponent<AudioSource>());

		manager.music = -manager.music;
        PlayerPrefs.SetInt("Music", manager.music);
        PlayerPrefs.Save();

        if(manager.music == 1) {
			manager.GetComponent<AudioSource>().Play();
		} else {
			manager.GetComponent<AudioSource>().Stop();
		}
    }

    public void SetSFX() {
        manager.sfx = -manager.sfx;
		PlayerPrefs.SetInt("SFX", manager.sfx);
		PlayerPrefs.Save();

		Extensions.Click(manager, main.GetComponent<AudioSource>());
	}
}