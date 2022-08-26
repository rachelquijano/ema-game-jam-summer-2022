using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour {

    float speed = 5f;
    float height = 0.1f;

    void Update() {
        Vector2 pos = transform.position;
        transform.position = new Vector2(pos.x, Mathf.Sin(Time.time * speed)) * height;
    }
}
