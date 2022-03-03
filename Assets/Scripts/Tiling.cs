using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tiling : MonoBehaviour
{
    // Makes sure that the duplicate will be set before camera pans over
    public int offsetX = 2;

    // Determines if sprite has a left or right buddy
    public bool hasRightBuddy = false;
    public bool hasLeftBuddy = false; 

    public bool reverseScale = false; // If a sprite does not have repeatability after duplicating

    private float spriteWidth = 0f; // Width of transform
    private Camera cam; // Main camera
    private Transform myTransform; // Transform that the script is on
    
    public float xScale = 0f; // How much the sprite in the editor is stretched in the x direction

    private void Awake()
    {
        cam = Camera.main;
        myTransform = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sRen = GetComponent<SpriteRenderer>();
        spriteWidth = sRen.sprite.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Does it still need buddies? If not, do nothing.
        if (hasLeftBuddy == false || hasRightBuddy == false)
        {
            // Calculate the camera's extend (half the width) of what the camera can see in world coordinates
            float camHorizontalExtend = cam.orthographicSize * Screen.width / Screen.height;

            // Calculate the x position where the camera can see the edge of the sprite (element)
            float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth / 2) - camHorizontalExtend;
            float edgeVisiblePositionLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtend;

            // Checks if you can see the edge of the element sprite, then calling the new function makeNewBuddy
            // Could be more efficient if you check a right buddy first
            if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && hasRightBuddy == false)
            {
                makeNewBuddy(1);
                hasRightBuddy = true;
            }
            else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && hasLeftBuddy == false)
            {
                makeNewBuddy(-1);
                hasLeftBuddy = true;
            }
        }
    }

    // Will instatiate a right or left buddy based on the parameter rightOrLeft
    // rightOrLeft is an int that will either be 1 (for a right buddy) or -1 (for a left buddy)
    void makeNewBuddy(int rightOrLeft)
    {
        // Calculating the new position for our new buddy
        Vector3 newPosition = new Vector3(myTransform.position.x + spriteWidth * xScale * rightOrLeft, myTransform.position.y, myTransform.position.z);

        // Instantiating our new body and storing it as a transform
        Transform newBuddy = Instantiate(myTransform, newPosition, myTransform.rotation) as Transform;

        // if not tilable, reverse the x scale to get rid of ugly seams
        // Calling xScale = myTransform.localScale.x in start method breaks this, not sure why.
        if (reverseScale == true)
        {
            newBuddy.localScale = new Vector3(newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
        } 

        newBuddy.parent = myTransform.parent;

        // the NEW sprite made will have a left buddy depending on the integer rightOrLeft
        if (rightOrLeft > 0)
        {
            newBuddy.GetComponent<Tiling>().hasLeftBuddy = true;
        }
        else
        {
            newBuddy.GetComponent<Tiling>().hasRightBuddy = true;
        }
    }
}
