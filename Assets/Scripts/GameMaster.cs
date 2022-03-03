using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{

    public static GameMaster gm;

    public GameManager gameManager;

    void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
        if(gameManager == null)
        {
            gameManager = GetComponent<GameManager>();
        }
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 2;
    public Transform spawnPrefab;

    public GameObject completeLevelUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            // Restart Level
            gameManager.Restart();
        }
    }

    // Not sure if this is needed
    public IEnumerator restartLevel()
    {
        yield return new WaitForSeconds(spawnDelay);

        gameManager.Restart();

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        //Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation);

        //Destroy(clone.gameObject, 3f);
    }

    public static void killPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.restartLevel()); // Instead of this, restart level?
    }

    public static void killEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    public void LevelComplete()
    {
        completeLevelUI.SetActive(true);
    }
}
