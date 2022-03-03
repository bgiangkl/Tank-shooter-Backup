using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
public class FollowEnemyAI : MonoBehaviour
{
    public Transform afterSmokePrefab;
    public Transform smokePoint;
    float timeToSpawnEffect;
    public float effectSpawnRate = 10f;

    // last known direction the enemy was moving
    private Vector3 direction;

    // Smoothing of the transition between two rotations
    public float turningRate = 100f;

    // Rotation offset on where the enemy is facing
    public float rotationOffset = -90;

    // Who to chase
    public Transform target;

    // How many per second we will update our path
    public float updateRate = 2f;

    // Caching
    private Seeker seeker;
    private Rigidbody2D rb;

    // Storing the calculated path
    public Path path;

    // The AI's speed per second (not framerate dependent)
    public float speed = 300f;
    // A way to change between force and impulse, controls how force is applied
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathHasEnded = false;

    // The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3f;

    // The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private bool searchingForPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if(target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine("searchForPlayer");
            }

            return;
        }

        // Start a new path to the target position, and return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine("UpdatePath");
    }

    IEnumerator UpdatePath()
    {
        if(target == null)
        {
            if(!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine("searchForPlayer");
            }
            yield return false;
        }

        // Start a new path to the target position, and return the result to the OnPathComplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f / updateRate);

        StartCoroutine("UpdatePath");
    }

    IEnumerator searchForPlayer()
    {
        // If you didn't find player, keep repeating until found. If found, then set target and updatepath again.
        GameObject sResult = GameObject.FindGameObjectWithTag("Player");
        if(sResult == null)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine("searchForPlayer");
        } else
        {
            searchingForPlayer = false;
            target = sResult.transform;
            StartCoroutine("UpdatePath");
            yield return false;
        }
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("We got a path. Did it have an error? " + p.error);
        // If there is not an error, use the path
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void Update()
    {
        float degree = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRot = Quaternion.Euler(0f, 0f, degree + rotationOffset);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turningRate * Time.deltaTime);

        if(Time.time >= timeToSpawnEffect)
        {
            Effect();
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    // Used for physics things
    void FixedUpdate()
    {
        if (target == null)
        {
            // Make sure that coroutine is not running more than once
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine("searchForPlayer");
            }

            return;
        }

        //TODO: always look at the player, or facing toward where it is moving

        if(path == null)
        {
            return;
        }

        // Each path has a list of vectors to follow, so each path will have a new set of direction vectors
        if(currentWaypoint >= path.vectorPath.Count)
        {
            if(pathHasEnded)
            {
                return;
            }
            Debug.Log("End of path reached.");
            pathHasEnded = true;
            return;
        }

        pathHasEnded = false;

        // Find direction to next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        direction = dir;

        // Move the AI
        rb.AddForce(dir, fMode);

        // Go to the next waypoint if you're a set distance away from the waypoint
        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if(dist < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    private void Effect()
    {
        Transform clone = Instantiate(afterSmokePrefab, smokePoint.position, smokePoint.rotation) as Transform;

        Destroy(clone.gameObject, 1f);
    }
}
