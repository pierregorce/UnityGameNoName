using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MonoBehaviourExtended : MonoBehaviour
{
    //float nextdisableTime = 0;
    //bool setDisable = false;

    public virtual void Init()
    {
        gameObject.SetActive(true);
        gameObject.transform.localScale = Vector2.one;
        gameObject.transform.localPosition = Vector2.zero;
        gameObject.transform.localRotation = Quaternion.identity;
    }

    protected virtual void Update()
    {
        //if (setDisable && Time.time > nextdisableTime)
        //{
        //    gameObject.SetActive(false);
        //    setDisable = false;
        //}
    }

    protected void Disable(float time)
    {
        //nextdisableTime = Time.time + time;
        //setDisable = true;
        StartCoroutine(SetDisable(time));
    }

    IEnumerator SetDisable(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
