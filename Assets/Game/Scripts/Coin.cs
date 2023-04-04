using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Coin : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();
            if (player == null) return;

            player.AddCoin();

            Destroy(gameObject);
        }
    }
}
