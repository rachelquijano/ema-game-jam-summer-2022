using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private TempCamFollow camFollow;
    [SerializeField] Vector3 initialPos;
    [SerializeField] GameObject[] doors, coins;

    private void Awake()
    {
        initialPos = FindObjectOfType<PlayerController>().transform.position;
        camFollow = FindObjectOfType<TempCamFollow>();
        doors = GameObject.FindGameObjectsWithTag("Door");
        coins = GameObject.FindGameObjectsWithTag("Coin");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            StartCoroutine(restartLevel(collision));
        }
    }

    IEnumerator restartLevel(Collider2D collision)
    {
        collision.GetComponent<Collider2D>().enabled = false;
        collision.GetComponent<PlayerController>().enabled = false;
        camFollow.enabled = false;
        yield return new WaitForSeconds(2f);

        collision.GetComponent<Collider2D>().enabled = true;
        collision.gameObject.transform.position = initialPos;
        foreach(GameObject door in doors)
        {
            door.SetActive(true);
        }
        foreach (GameObject coin in coins)
        {
            coin.SetActive(true);
        }
        collision.GetComponent<PlayerController>().enabled = true;
        camFollow.enabled = true;
    }
}
