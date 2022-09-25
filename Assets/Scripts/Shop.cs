using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Shop : MonoBehaviour {
    public Button diamonds, coins, magic, tickets, offer, stones;
    public GameObject diamondWindow, coinWindow, magicWindow, ticketWindow, offerWindow, stoneWindow, purchased;
    public TextMeshProUGUI howManyTickets, howMuchCoins, howMuchMagic;
    public Slider coinSlider, magicSlider, ticketSlider;

	[HideInInspector]
	public int window = 0;

    private MainScreen main;
	private UpgradeScene upgrade;
	private SummonScene summon;
	private TeamSelection team;
	private GameManager manager;
	private int scene = 0;

    void Start() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();

		switch(SceneManager.GetActiveScene().name) {
			case "Main Screen":
			scene = 0;
			main = GetComponentInParent<MainScreen>();
			ticketSlider.onValueChanged.AddListener(delegate { OnTicketChange(); });
			break;
			case "Upgrading":
			scene = 1;
			upgrade = GetComponentInParent<UpgradeScene>();
			break;
			case "Summoning":
			scene = 2;
			summon = GetComponentInParent<SummonScene>();
			break;
			case "Team Selection":
			scene = 3;
			team = GetComponentInParent<TeamSelection>();
			ticketSlider.onValueChanged.AddListener(delegate { OnTicketChange(); });
			break;
		}

		switch(window) {
			case 0:
			OnDiamonds();
			break;
			case 1:
			OnCoins();
			break;
			case 2:
			OnMagic();
			break;
			case 3:
			OnTickets();
			break;
			case 4:
			OnStones();
			break;
		}

        coinSlider.onValueChanged.AddListener(delegate { OnCoinsChange(); });
        magicSlider.onValueChanged.AddListener(delegate { OnMagicChange(); });
	}

	#region Button Listeners
	public void OnTicketChange() {
		int m_amount = Mathf.FloorToInt(ticketSlider.value);
		int m_coins = Mathf.FloorToInt(ticketSlider.value * 250f);
		howManyTickets.text = string.Concat("+", Extensions.KFormat(m_amount), " <color=white>tickets</color>\r\n<color=#FFDC00>-", Extensions.KFormat(m_coins), " coins</color>");
	}

	public void OnMagicChange() {
		int m_amount = Mathf.FloorToInt(magicSlider.value);
		int m_coins = Mathf.FloorToInt(magicSlider.value * 120f);
		howMuchMagic.text = string.Concat("+", Extensions.KFormat(m_amount), " <color=#0870F3>magic stuff</color>\r\n<color=#FFDC00>-", Extensions.KFormat(m_coins), " coins</color>");
	}

	public void OnCoinsChange() {
		int m_amount = Mathf.FloorToInt(coinSlider.value * 48f);
		int m_diamonds = Mathf.FloorToInt(coinSlider.value);
		howMuchCoins.text = string.Concat("+", Extensions.KFormat(m_amount), " <color=#FFDC00>coins</color>\r\n<color=#aacbe7>-", Extensions.KFormat(m_diamonds), " diamonds</color>");
	}

    public void OnOffer() {
		Click();
		diamonds.interactable = true;
		coins.interactable = true;
		magic.interactable = true;
		tickets.interactable = true;
		diamondWindow.SetActive(false);
		coinWindow.SetActive(false);
		magicWindow.SetActive(false);
		ticketWindow.SetActive(false);
		offer.interactable = false;
        offerWindow.SetActive(true);

		if(manager.doubleRewards) {
			offerWindow.GetComponentInChildren<Button>().interactable = false;
			purchased.SetActive(true);
		}
    }

	public void OnPurchaseStone(int type) {
		Click();
		bool notEnough = false;

		switch(type) {
			case 1:
			if(manager.coinValue >= 5000) {
				notEnough = false;
				manager.coinValue -= 5000;
				++manager.silverValue;
				summon.silverCount.text = Extensions.KFormat(manager.silverValue);
				summon.coinsText.text = Extensions.KFormat(manager.coinValue);
			} else
				notEnough = true;
			break;
			case 2:
			if(manager.coinValue >= 30_000) {
				manager.coinValue -= 30_000;
				++manager.solarValue;
				summon.solarCount.text = Extensions.KFormat(manager.solarValue);
				summon.coinsText.text = Extensions.KFormat(manager.coinValue);
				notEnough = false;
			} else
				notEnough = true;
			break;
			case 3:
			if(manager.diamondValue >= 1600) {
				manager.diamondValue -= 1600;
				++manager.cosmicValue;
				summon.cosmicCount.text = Extensions.KFormat(manager.cosmicValue);
				summon.diamondsText.text = Extensions.KFormat(manager.diamondValue);
				notEnough = false;
			} else
				notEnough = true;
			break;
		}
		manager.Save();

		if(notEnough) {
#if UNITY_ANDROID && !UNITY_EDITOR
			if(type == 3)
				Extensions.ShowAndroidToastMessage("You don't have enough diamonds");
			else
				Extensions.ShowAndroidToastMessage("You don't have enough coins");
#endif
		}
	}

    public void OnDiamonds() {
		Click();

		diamonds.interactable = false;
        coins.interactable = true;
        magic.interactable = true;
		tickets.interactable = true;
		offer.interactable = true;
		offerWindow.SetActive(false);
		diamondWindow.SetActive(true);
        coinWindow.SetActive(false);
        magicWindow.SetActive(false);
		ticketWindow.SetActive(false);

		if(scene == 2) {
			stones.interactable = true;
			stoneWindow.SetActive(false);
		}
	}

    public void OnCoins() {
		Click();

		diamonds.interactable = true;
        coins.interactable = false;
        magic.interactable = true;
		tickets.interactable = true;
		offer.interactable = true;
		offerWindow.SetActive(false);
		diamondWindow.SetActive(false);
        coinWindow.SetActive(true);
        magicWindow.SetActive(false);
		ticketWindow.SetActive(false);

		if(scene == 2) {
			stones.interactable = true;
			stoneWindow.SetActive(false);
		}

		coinSlider.maxValue = Mathf.Clamp(manager.diamondValue, 0, int.MaxValue);
		coinSlider.value = 0f;
	}

	public void OnMagic() {
		Click();

		diamonds.interactable = true;
		coins.interactable = true;
		magic.interactable = false;
		tickets.interactable = true;
		offer.interactable = true;
		offerWindow.SetActive(false);
		diamondWindow.SetActive(false);
		coinWindow.SetActive(false);
		magicWindow.SetActive(true);
		ticketWindow.SetActive(false);

		magicSlider.maxValue = Mathf.FloorToInt(manager.coinValue / 120f);
		magicSlider.value = 0f;
	}

	public void OnTickets() {
		Click();

		diamonds.interactable = true;
		coins.interactable = true;
		magic.interactable = true;
		tickets.interactable = false;
		offer.interactable = true;
		offerWindow.SetActive(false);
		diamondWindow.SetActive(false);
		coinWindow.SetActive(false);
		magicWindow.SetActive(false);
		ticketWindow.SetActive(true);

		ticketSlider.maxValue = Mathf.FloorToInt(manager.coinValue / 250f);
		ticketSlider.value = 0f;
	}

	public void OnStones() {
		Click();

		diamonds.interactable = true;
		coins.interactable = true;
		diamondWindow.SetActive(false);
		coinWindow.SetActive(false);
		stones.interactable = false;
		stoneWindow.SetActive(true);
	}

	public void OnPurchaseCoins() {
		manager.coinValue += Mathf.FloorToInt(coinSlider.value * 48f);
		manager.diamondValue -= Mathf.FloorToInt(coinSlider.value);
		manager.Save();

		switch(scene) {
			case 0:
			main.coins.text = Extensions.KFormat(manager.coinValue);
			main.diamonds.text = Extensions.KFormat(manager.diamondValue);
			break;
			case 1:
			upgrade.coinText.text = Extensions.KFormat(manager.coinValue);
			break;
			case 2:
			summon.coinsText.text = Extensions.KFormat(manager.coinValue);
			summon.diamondsText.text = Extensions.KFormat(manager.diamondValue);
			break;
			case 3:
			team.coinsText.text = Extensions.KFormat(manager.coinValue);
			break;
		}

		OnCoins();
	}

	public void OnPurchaseMagic() {
		manager.magicValue += Mathf.FloorToInt(magicSlider.value);
		manager.coinValue -= Mathf.FloorToInt(magicSlider.value * 120f);
		manager.Save();

		switch(scene) {
			case 0:
			main.coins.text = Extensions.KFormat(manager.coinValue);
			main.magicStuff.text = Extensions.KFormat(manager.magicValue);
			break;
			case 1:
			upgrade.coinText.text = Extensions.KFormat(manager.coinValue);
			upgrade.magicStuffText.text = Extensions.KFormat(manager.magicValue);
			break;
		}

		OnMagic();
	}

	public void OnPurchaseTickets() {
		manager.ticketValue += Mathf.FloorToInt(ticketSlider.value);
		manager.coinValue -= Mathf.FloorToInt(ticketSlider.value * 250f);
		manager.Save();

		switch(scene) {
			case 0:
			main.coins.text = Extensions.KFormat(manager.coinValue);
			main.tickets.text = Extensions.KFormat(manager.ticketValue);
			break;
			case 3:
			team.coinsText.text = Extensions.KFormat(manager.coinValue);
			team.tickets.text = Extensions.KFormat(manager.ticketValue);
			break;
		}

		OnTickets();
	}

	private void Click() {
		Extensions.Click(manager, GetComponent<AudioSource>());
	}
	#endregion

	#region Purchases
	public void OnDiamondPurchase(int value) {
		Click();

		manager.diamondValue += value;
		manager.Save();

		if(scene == 0) {
			main.diamonds.text = Extensions.KFormat(manager.diamondValue);
		} else if(scene == 2) {
			summon.diamondsText.text = Extensions.KFormat(manager.diamondValue);
		}

		Extensions.ShowAndroidToastMessage("Thank you for your purchase!");
	}

	public void OnDoubleRewardsPurchase() {
		Click();

		manager.doubleRewards = true;
		manager.Save();

		offerWindow.GetComponentInChildren<Button>().interactable = false;
		purchased.SetActive(true);

		Extensions.ShowAndroidToastMessage("Thank you for your purchase!");
	}

	public void OnFailedPurchase() {
		Extensions.ShowAndroidToastMessage("Sorry, something went wrong");
	}
	#endregion

	#region Extentions
	private void showToast(string text, int duration) {
		StartCoroutine(showToastCOR(text, duration));
	}

	private IEnumerator showToastCOR(string text, int duration) {
		Color orginalColor = main.txt.color;

		main.txt.text = text;
		main.txt.enabled = true;

		//Fade in
		yield return fadeInAndOut(main.txt, true, 0.5f);

		//Wait for the duration
		float counter = 0;
		while(counter < duration) {
			counter += Time.deltaTime;
			yield return null;
		}

		//Fade out
		yield return fadeInAndOut(main.txt, false, 0.5f);

		main.txt.enabled = false;
		main.txt.color = orginalColor;
	}

	private IEnumerator fadeInAndOut(TextMeshProUGUI targetText, bool fadeIn, float duration) {
		//Set Values depending on if fadeIn or fadeOut
		float a, b;
		if(fadeIn) {
			a = 0f;
			b = 1f;
		} else {
			a = 1f;
			b = 0f;
		}

		Color currentColor = targetText.color;
		float counter = 0f;

		while(counter < duration) {
			counter += Time.deltaTime;
			float alpha = Mathf.Lerp(a, b, counter / duration);

			targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
			yield return null;
		}
	}
#endregion
}