using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public class PlayerStats
    {
        public int maxHealth = 100;

        private int _curHealth;
        public int curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init()
        {
            curHealth = maxHealth;
        }
    }

    public PlayerStats stats = new PlayerStats();

    public int fallBoundary;

    [SerializeField]
    private StatusIndicator statusIndicator;

    // Start is called before the first frame update
    void Start()
    {
        stats.Init();
        
        if(statusIndicator == null)
        {
            Debug.LogError("No Status indicator reference on player");  
        } else
        {
            statusIndicator.setHealth(stats.curHealth, stats.maxHealth);
        }
    }

    private void Update()
    {
        if (transform.position.y <= fallBoundary)
        {
            damagePlayer(99999);
        }
    }

    public void damagePlayer(int damage)
    {
        stats.curHealth -= damage;
        if(stats.curHealth <= 0)
        {
            GameMaster.killPlayer(this);    
        }

        statusIndicator.setHealth(stats.curHealth, stats.maxHealth);
    }
}
