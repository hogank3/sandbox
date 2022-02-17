using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy;
    public float respawnTime = 1.0f;
    private Vector2 bounds;
    private int spawnSide;

    // Start is called before the first frame update
    void Start()
    {
        bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        StartCoroutine(enemyWave());
    }

    private void spawnEnemy()
    {
        GameObject gob = Instantiate(enemy) as GameObject;
        //gob.transform.position = new Vector2(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
        //Debug.Log((Random.Range(0, 2) * 2 - 1).ToString());
        spawnSide = Random.Range(0, 4);

        switch (spawnSide)
        {
            case 0: //top
                gob.transform.position = new Vector3(Random.Range(-bounds.x, bounds.x), 0, bounds.y + 1);
                break;
            case 1: //bottom
                gob.transform.position = new Vector3(Random.Range(-bounds.x, bounds.x), 0, -bounds.y - 1);
                break;
            case 2: //left
                gob.transform.position = new Vector3(-bounds.x - 1, 0, Random.Range(-bounds.y, bounds.y));
                break;
            case 3: //right
                gob.transform.position = new Vector3(bounds.x + 1, 0, Random.Range(-bounds.y, bounds.y));
                break;
        }
    }

    IEnumerator enemyWave()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);
            spawnEnemy();
        }
    }
}
