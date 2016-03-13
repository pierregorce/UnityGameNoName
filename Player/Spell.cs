using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Spell : MonoBehaviour
{

    public float cooldown = 1;
    private float nextAttackTime;

    public GameObject textCooldown;
    public GameObject panelCooldown;

    //public delegate void DoSpellEvent();
    //public event DoSpellEvent DoSpell = delegate { };

    void Start()
    {

    }

    void Update()
    {
        //Affiche le CD sur l'UI
        float currentCoolDownTime = nextAttackTime - Time.time;
        string coolDownTime = currentCoolDownTime.ToString("0.0");

        if (currentCoolDownTime < 0)
        {
            coolDownTime = "";
        }
        if (textCooldown != null)
        {
            textCooldown.GetComponent<Text>().text = coolDownTime;
        }

        if (panelCooldown != null)
        {
            panelCooldown.GetComponent<Image>().fillAmount = currentCoolDownTime / cooldown;
        }
    }

    public bool CanDoSpell()
    {
        if (Time.time > nextAttackTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DoSpell()
    {
        if (Time.time > nextAttackTime)
        {
            nextAttackTime = Time.time + cooldown;
            //DoSpell();
        }
        else
        {
            //we are on cooldown
        }
    }







}
