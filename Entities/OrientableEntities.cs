using UnityEngine;
using System.Collections;

public class OrientableEntities : MonoBehaviour
{

    public FacingOrientation currentFacingOrientation = FacingOrientation.FACING_RIGHT;

    public enum FacingOrientation
    {
        FACING_LEFT,
        FACING_RIGHT
    }

    void Update()
    {
        if (currentFacingOrientation == FacingOrientation.FACING_LEFT)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
