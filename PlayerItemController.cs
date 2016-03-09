using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerItemController : MonoBehaviour
{
    //rename en playerStat plutot...
    public GameObject canvasFloatingText;
    private Mortality mortality;



    // Use this for initialization
    void Start()
    {
        mortality = GetComponent<Mortality>();
    }

    void Update()
    {
    }

    public void LootItem(int gold)
    {
        GameObject canvasText = Instantiate(canvasFloatingText);
        //canvasText.transform.SetParent(transform);

        float randomX = Random.Range(-0.6f, 0.6f);
        float randomY = Random.Range(-0.6f, 0.6f);

        canvasText.transform.position = transform.position + new Vector3(randomX, randomY, 0);
        canvasText.GetComponent<Animator>().SetTrigger("Item");

        canvasText.transform.Find("Item").GetComponent<Text>().text = "+" + gold;

        Destroy(canvasText, 1.5f);
    }

    public void GainLife(int life)
    {
        GameManager.instance.uiManager.SetLife(mortality.health , mortality.health + life);


        GameObject canvasText = Instantiate(canvasFloatingText);
        
        canvasText.transform.SetParent(GameManager.instance.transform);

        float randomX = (Random.Range(0, 2) * 2) - 1; //Random entre 0 et 1

        float randomY = Random.Range(-0.2f, 0.2f);

        canvasText.transform.position = transform.position + new Vector3(randomX, randomY, 0);
        canvasText.GetComponent<Animator>().SetTrigger("Life");

        canvasText.transform.Find("Life").GetComponent<Text>().text = "+" + life + "HP";

        Destroy(canvasText, 1.5f);
    }

    public void LooseLife(int life)
    {

        //Ajout la vie déja retranché par le composant mortality qui appelle cette méthode
        GameManager.instance.uiManager.SetLife(mortality.health + life, mortality.health);



        GameObject canvasText = Instantiate(canvasFloatingText);
        canvasText.transform.SetParent(GameManager.instance.transform);

        float randomX = Random.Range(-0.2f, 0.2f);
        float randomY = Random.Range(-0.2f, 0.2f);

        canvasText.transform.position = transform.position + new Vector3(randomX, randomY, 0);
        canvasText.GetComponent<Animator>().SetTrigger("Hit");

        canvasText.transform.Find("Damage").GetComponent<Text>().text = "-" + life;

        Destroy(canvasText, 1.5f);
    }

    public void GainXp(int xp)
    {
        GameManager.instance.uiManager.SetXp(GetComponent<PlayerController>().currentXp, GetComponent<PlayerController>().currentXp + xp);
        //todo combat texte
    }

    public int GetRequiertXP(int level)
    {
        return (level + 1) * 200;
    }

    public int GetSumXpForLevel(int level)
    {
        int total = 0;

        for (int i = 0; i < level; i++)
        {
            total += GetRequiertXP(i);
        }
        return total;
    }

    public int GetLevelForXP(int xp)
    {
        for (int i = 1; i < 80; i++)
        {
            if (xp < GetSumXpForLevel(i))
            {
                return i;
            }
        }
        return 0;
    }

}
