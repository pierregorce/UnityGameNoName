using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{

    private int targetLife;
    private int orginalLife;
    GameObject lifeText;
    GameObject lifeBar;

    private int targetXp;
    private int orginalXp;
    GameObject xpText;
    GameObject xpBar;

    private int orginalMoney;
    private int targetMoney;
    GameObject moneyQuantityText;

    GameObject progressRoomText;
    GameObject progressMonterText;


    void Start()
    {
        lifeText = GameObject.Find("LifeText");
        lifeBar = GameObject.Find("LifeBar");

        xpText = GameObject.Find("XpText");
        xpBar = GameObject.Find("XpBar");

        moneyQuantityText = GameObject.Find("MoneyQuantityText");

        progressRoomText = GameObject.Find("ProgressRoomText");
        progressMonterText = GameObject.Find("ProgressMonsterText");

    }

    void Update()
    {
        EaseLife();
        EaseXp();
        EaseMoney();
    }

    private float timeElapsedLife;

    void EaseLife()
    {
        int maxLife = GameManager.instance.player.GetComponent<Mortality>().initialHealth;

        timeElapsedLife += Time.deltaTime;

        //Ease
        /// variation = change in value
        /// elapsed = current time
        /// delay = duration
        /// offset = startValue

        float variation = targetLife - orginalLife;
        float elapsed = timeElapsedLife;
        float delay = 0.7f;
        float offset = orginalLife;

        float finalLife = Ease.CubicOut(variation, elapsed, delay, offset);

        lifeText.GetComponent<UnityEngine.UI.Text>().text = maxLife + " / " + (int)finalLife;
        lifeBar.GetComponent<UnityEngine.UI.Image>().fillAmount = Mathf.Clamp01(finalLife / maxLife);
    }

    private float timeElapsedXp;

    void EaseXp()
    {
        int level = GameManager.instance.player.GetComponent<PlayerStatsController>().GetLevelForXP(targetXp);
        int maxXp = GameManager.instance.player.GetComponent<PlayerStatsController>().GetSumXpForLevel(level);

        timeElapsedXp += Time.deltaTime;

        //Ease
        /// variation = change in value
        /// elapsed = current time
        /// delay = duration
        /// offset = startValue

        float variation = targetXp - orginalXp;
        float elapsed = timeElapsedXp;
        float delay = 0.7f;
        float offset = orginalXp;

        float finalXp = Ease.CubicOut(variation, elapsed, delay, offset);

        xpText.GetComponent<UnityEngine.UI.Text>().text = maxXp + " / " + (int)finalXp;
        xpBar.GetComponent<UnityEngine.UI.Image>().fillAmount = Mathf.Clamp01(finalXp / maxXp);
    }

    private float timeElapsedMoney;

    void EaseMoney()
    {
        timeElapsedMoney += Time.deltaTime;

        //Ease
        /// variation = change in value
        /// elapsed = current time
        /// delay = duration
        /// offset = startValue

        float variation = targetMoney - orginalMoney;
        float elapsed = timeElapsedMoney;
        float delay = 0.7f;
        float offset = orginalMoney;

        float finalMoney = Ease.CubicOut(variation, elapsed, delay, offset);

        moneyQuantityText.GetComponent<UnityEngine.UI.Text>().text = ((int) finalMoney).ToString();
    }

    public void SetLife(int orginalLife, int targetLife)
    {
        this.targetLife = targetLife;
        this.orginalLife = orginalLife;
        timeElapsedLife = 0;
    }

    public void SetXp(int orginalXp, int targetXp)
    {
        this.targetXp = targetXp;
        this.orginalXp = orginalXp;
        timeElapsedXp = 0;
    }

    public void SetMoney(int orginalMoney, int targetMoney)
    {
        this.targetMoney = targetMoney;
        this.orginalMoney = orginalMoney;
        timeElapsedMoney = 0;
    }


    public void SetMonsterProgress(int totalQuantity, int remainingQuantity)
    {
        progressMonterText.GetComponent<UnityEngine.UI.Text>().text = remainingQuantity + "/" + totalQuantity;
    }

    public void SetRoomProgress(int totalRoom, int currentRoom)
    {
        progressRoomText.GetComponent<UnityEngine.UI.Text>().text = currentRoom + "/" + totalRoom;
    }

    /// <summary>
    /// Texte disparaissant automatiquement. 
    /// Utilisé pour afficher un evenemet/message ne demandant pas de click sur l'écran pour continuer par exmeple.
    /// </summary>
    public void ShowInformationUI(string textLeft, string textRight, string textBottom)
    {
        GameObject panel = GameObject.Find("TextSchemeA");
        panel.transform.FindChild("Left").GetComponent<UnityEngine.UI.Text>().text = textLeft;
        panel.transform.FindChild("Right").GetComponent<UnityEngine.UI.Text>().text = textRight;
        panel.transform.FindChild("Bottom").GetComponent<UnityEngine.UI.Text>().text = textBottom;
        panel.GetComponent<Animator>().SetTrigger("Show");
    }
}
