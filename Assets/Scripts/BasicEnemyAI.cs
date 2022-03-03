using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAI : MonoBehaviour
{

    private Rigidbody2D rb;
    private Vector3 dir;

    // A way to change force and impulse, controls how force is applied.
    public ForceMode2D fMode;

    public float speed = 300f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get direction and multiply by the speed
        getDir();
        dir *= speed * Time.fixedDeltaTime;

        // Move the AI
        rb.AddForce(dir, fMode);
    }

    private void getDir()
    {
        if(transform.localScale.x == 1)
        {
            dir = new Vector3(-1, 0, 0);
        } else
        {
            dir = new Vector3(1, 0, 0);
        }
    }
}
