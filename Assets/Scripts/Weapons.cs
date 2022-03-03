using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public int rotationOffset = 0;
    //public float lowerBoundShootAngle = -15f;
    //public float upperBoundShootAngle = 30f;

    public Sprite[] listOfWeapons;

    private SpriteRenderer sRen;

    public float fireRate = 2;
    public int damage = 50;
    int currWep = 0;

    public Transform muzzleFlashPrefab;
    public Transform bulletTrailPrefab;
    public Transform bombPrefab; // No trail I think, just projectile.  
    public Transform explodePrefab;
    public float angle = 30f;
    public ParticleSystem bombSmoke; 
    public Transform meleeEffectPrefab; // Shock or fire? Not currently sure about this
    float timeToSpawnEffect;
    public float effectSpawnRate = 10;

    public Transform graphics;

    Transform firePoint;
    float timeToFire = 0f;
    public LayerMask toHit;
    

    private void Awake()
    {
        firePoint = transform.FindChild("FirePoint");
        if (firePoint == null)
            Debug.LogError("No FirePoint object");
    }

    // Start is called before the first frame update
    void Start()
    {
        sRen = transform.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1))
        {
            sRen.sprite = listOfWeapons[0];
            fireRate = 2;
            currWep = 0;
            damage = 50;
        } else if(Input.GetKey(KeyCode.Alpha2))
        {
            sRen.sprite = listOfWeapons[1];
            fireRate = 1;
            currWep = 1;
            damage = 50;
        } else if(Input.GetKey(KeyCode.Alpha3))
        {
            //sRen.sprite = listOfWeapons[2];
            //fireRate = 2;
            //currWep = 0;
            //damage = 30;
        }

        Rotation();

        if (fireRate == 0)
        {
            // GetButtonDown only allows for single click
            if(Input.GetButtonDown("Fire1"))
                Shoot(currWep);
        } else
        {
            // GetButton allows for a button to be pressed down
            if (Input.GetButton("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                Shoot(currWep);
            }
        }
    }

    void Rotation()
    {
        // Subtracting the position of the player to the mouse position
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();

        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg; // Find angle in degrees

        // This is to deal with the z axis changing when the sprite changes direction
        /*if(graphics.localScale.x < 0)
        {
            rotationOffset = 180;
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            //lowerBoundShootAngle = 180f;
            //upperBoundShootAngle = 360f;
        } else
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            rotationOffset = 0;
            //lowerBoundShootAngle = 0f;
            //upperBoundShootAngle = 180f;
        }*/

        // rotZ + rotationOffset
        // Mathf.Clamp(rotZ + rotationOffset, lowerBoundShootAngle, upperBoundShootAngle), cannot really clamp how I want because of how atan2 works
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
    }

    void Shoot(int currWep)
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100, ~toHit);

        // Debug.DrawLine(firePointPosition, (mousePosition - firePointPosition) * 1000, Color.blue);
        if(hit.collider != null && currWep == 0)
        {
            // Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.DamageEnemy(damage);
            }
        }

        // This is like firerate to make sure that so many prefabs won't spawn
        if(Time.time >= timeToSpawnEffect)
        {
            Vector3 hitPos;
            Vector3 hitNormal;

            // hitNormal are for particle effects after shot hits if I wanted them
            if(hit.collider == null)
            {
                hitPos = (mousePosition - firePointPosition) * 1000;
                hitNormal = new Vector3(9999, 9999, 9999);
            } else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }

            // Eventually, effect will also take in a number for different effects (or different methods for each weapon)
            switch (currWep)
            {
                case 0:
                    Effect0(hitPos, hitNormal);
                    break;
                case 1:
                    Vector2 vel = BallisticVel(mousePosition, firePointPosition);
                    StartCoroutine(Effect1(hitPos, hitNormal, vel));
                    break;
                case 2:
                    Effect2(hitPos, hitNormal);
                    break;
                default:
                    Effect0(hitPos, hitNormal);
                    break;
            }
            
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void Effect0(Vector3 hitPos, Vector3 hitNormal)
    {
        Transform trail = Instantiate(bulletTrailPrefab, firePoint.position, firePoint.rotation) as Transform;
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        if(lr != null)
        {
            // SET POSITIONS
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }

        Destroy(trail.gameObject, 0.04f);

        Transform clone = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        clone.parent = firePoint;
        float size = Random.Range(0.2f, 0.5f);
        clone.localScale = new Vector3(size, size, size);
        // Destroy prefab after a little bit
        Destroy(clone.gameObject, 0.05f);
    }

    IEnumerator Effect1(Vector3 hitPos, Vector3 hitNormal, Vector2 vel)
    {
        Transform clone = Instantiate(bombPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D clonerb = clone.GetComponent<Rigidbody2D>();
        clonerb.velocity = vel;

        //clone.parent = firePoint;

        Destroy(clone.gameObject, 2f);

        yield return new WaitForSeconds(1.9f);

        Transform clone2 = Instantiate(explodePrefab, clone.position, clone.rotation) as Transform;
        Destroy(clone2.gameObject, 3f);

        // Taken from Info Gamer's video, Colliders still used even if not 2D
        Collider2D[] colliders = Physics2D.OverlapCircleAll(clone.transform.position, 1.0f);

        if(colliders.Length == 0)
        {
            Debug.Log("No Colliders");
        }

        foreach (Collider2D hit in colliders) {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Maybe add a knockback??
                //rb.AddForce(10.0f, clone.position, 10.0f, 1.0f, ForceMode.Impulse);

                //Debug.Log("Notnull");
                Enemy enemy = hit.GetComponent<Enemy>(); 

                if(enemy != null)
                {
                    enemy.DamageEnemy(damage);
                }
            }
        }
    }

    // For the missile to shoot in parabolic arc
    Vector2 BallisticVel(Vector2 mousePosition, Vector2 firePointPosition)
    {
        Vector2 dir = mousePosition - firePointPosition; // get target direction
        float h = dir.y; // get height difference
        dir.y = 0; // retain only the horizontal direction
        var dist = dir.magnitude; // get horizontal distance
        var a = angle * Mathf.Deg2Rad; // get angle in radians
        dir.y = dist * Mathf.Tan(a); // set dir to the elevation angle
        dist += h / Mathf.Tan(a); // correct for small height differences
        // calculate the velocity magnitude
        float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return vel * dir.normalized;
    }

    void Effect2(Vector3 hitPos, Vector3 hitNormal)
    {

    }
}
