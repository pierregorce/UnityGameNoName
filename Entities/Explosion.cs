using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ColorChance
{
    public Color color;
    [Range(1, 100)]
    public int chance;
}

public class Explosion : MonoBehaviourExtended
{
    private List<string> triggers = new List<string>() {
        "Type1", "Type2", "Type3"
    };

    [Header("Spécifications")]
    public float scaleMin = 0.3f;
    public float scaleMax = 0.6f;
    [Header("Spécifications")]
    public ColorChance[] colors;

    public override void Init()
    {
        base.Init();
        Animator animator = GetComponent<Animator>();

        // Select a explosion animation
        animator.SetTrigger(triggers[Random.Range(0, triggers.Count)]);

        // Speed it
        animator.speed = Random.Range(1.15f, 1.35f);

        // Scale it
        float s = Random.Range(scaleMin, scaleMax);
        transform.localScale = new Vector3(s, s);

        // Color it
        if (colors != null && colors.Length > 0)
        {
            int colorRoll = Random.Range(0, colors.Sum(m => m.chance));
            int percentCurrent = 0;
            foreach (var item in colors)
            {
                percentCurrent += item.chance;

                if (percentCurrent >= colorRoll)
                {
                    GetComponent<SpriteRenderer>().color = item.color;
                    break;
                }
            }
        }



        //Destroy it
        Disable(3f);
        //Destroy(gameObject, 3f);

    }
}
