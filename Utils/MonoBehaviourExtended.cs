using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MonoBehaviourExtended : MonoBehaviour
{
    protected void Disable(float time)
    {
        StartCoroutine(SetDisable(time));
    }

    IEnumerator SetDisable(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
