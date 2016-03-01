using UnityEngine;
using System.Collections;

public class OrientableEntity : MonoBehaviour
{

    public enum FacingOrientation
    {
        FACING_LEFT,
        FACING_RIGHT
    }

    private FacingOrientation currentFacingOrientation;
    public FacingOrientation CurrentFacingOrientation
    {
        get
        {
            return currentFacingOrientation;
        }
        set
        {
            if (value == FacingOrientation.FACING_LEFT)
            {
                if (currentFacingOrientation != value)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
            }

            if (value == FacingOrientation.FACING_RIGHT)
            {
                if (currentFacingOrientation != value)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
            }
            currentFacingOrientation = value;
        }
    }

    void Start()
    {
        currentFacingOrientation = FacingOrientation.FACING_RIGHT;
    }


    void Update()
    {
        //if (currentFacingOrientation == FacingOrientation.FACING_LEFT)
        //{
        //    if (transform.localScale.x > 0)
        //    {
        //        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //    }
        //}
        //else
        //{
        //    if (transform.localScale.x < 0)
        //    {
        //        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //    }
        //}
    }
}
