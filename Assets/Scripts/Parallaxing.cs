using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Parallaxing : MonoBehaviour
{

    public Transform[] backgrounds; // Array of all things that need parallaxing
    private float[] parallaxScales; // Proportions of the camera's movement to move the backgrounds by
    public float smoothing = 1f;    // How smooth the parallaxing is going to be. Should always be above 0.

    private Transform cam;          // Reference to the camera
    private Vector3 previousCamPos; // The position of the camera in the previous frame

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        previousCamPos = cam.position;

        parallaxScales = new float[backgrounds.Length];

        // Assigning corresponding parallaxScales
        for(int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }

    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < backgrounds.Length; i++)
        {
            // The parallax is the opposite of the camera movement because the previous frame is multiplied by the scale
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];

            // Set the target position which is the current position plus the parallax
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;

            // Set new position with the new target x position
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            // Fade between the current position and target with lerp
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        // Set the previousCamPos to the camera's position at the end of the frame
        previousCamPos = cam.position;
    }
}
