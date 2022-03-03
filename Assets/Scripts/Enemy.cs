using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [System.Serializable]
    public class EnemyStats
    {
        public int maxHealth = 100;

        private int _curHealth;

        public int curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, 100); }
        }

        public int damage = 40;

        public void Init()
        {
            curHealth = maxHealth;
        }
    }

    public EnemyStats stats = new EnemyStats();

    public Transform explosionPoint;
    public Transform explosionEffectPrefab;

    [Header("Optional: ")]
    [SerializeField]
    private StatusIndicator statusIndicator;

    // Start is called before the first frame update
    void Start()
    {
        stats.Init();

        if(statusIndicator != null)
        {
            statusIndicator.setHealth(stats.curHealth, stats.maxHealth);
        }
     }

    public void DamageEnemy(int damage)
    {
        stats.curHealth -= damage;
        if(stats.curHealth <= 0)
        {
            GameMaster.killEnemy(this);
            if(this.tag == "Suicide")
            {
                DeathEffect();
            }
        }

        if(statusIndicator != null)
        {
            statusIndicator.setHealth(stats.curHealth, stats.maxHealth);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Player _player = collision.collider.GetComponent<Player>();
        if(_player != null)
        {
            _player.damagePlayer(stats.damage);
        }
        if(this.tag == "Suicide")
        {
            this.DamageEnemy(9999);
            DeathEffect();
            return;
        }
        flipDirection();
    }

    public void DeathEffect()
    {
        Transform clone = Instantiate(explosionEffectPrefab, explosionPoint.position, explosionPoint.rotation) as Transform;

        Destroy(clone.gameObject, 3f);
    }

    private void flipDirection()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

}
