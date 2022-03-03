using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRotation : MonoBehaviour
{
    public int rotationOffset = 0;

    // Update is called once per frame
    void Update()
    {
        // Subtracting the position of the player to the mouse position
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();

        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg; // Find angle in degrees

        // This is to deal with the z axis changing when the sprite changes direction
        /*if(transform.parent.localScale.x < 0)
        {
            rotationOffset = 180;
            //lowerBoundShootAngle = 180f;
            //upperBoundShootAngle = 360f;
        } else
        {
            rotationOffset = 0;
            //lowerBoundShootAngle = 0f;
            //upperBoundShootAngle = 180f;
        }*/

        // rotZ + rotationOffset
        // Mathf.Clamp(rotZ + rotationOffset, lowerBoundShootAngle, upperBoundShootAngle), cannot really clamp how I want because of how atan2 works
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
    }
}
