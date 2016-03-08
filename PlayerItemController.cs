using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerItemController : MonoBehaviour
{

    public GameObject canvasFloatingText;


    // Use this for initialization
    void Start()
    {
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
        GameObject canvasText = Instantiate(canvasFloatingText);
        canvasText.transform.SetParent(GameManager.instance.transform);

        float randomX = Random.Range(-0.2f, 0.2f);
        float randomY = Random.Range(-0.2f, 0.2f);

        canvasText.transform.position = transform.position + new Vector3(randomX, randomY, 0);
        canvasText.GetComponent<Animator>().SetTrigger("Hit");

        canvasText.transform.Find("Damage").GetComponent<Text>().text = "-" + life;

        Destroy(canvasText, 1.5f);
    }

}
