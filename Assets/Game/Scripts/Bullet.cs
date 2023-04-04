using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float damage = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.IsMasterClient && collision.CompareTag("Player") && !photonView.AmOwner)
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();
            if (player == null) return;

            player.DealDamage(damage);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (PhotonNetwork.IsMasterClient && collision.gameObject.layer == LayerMask.NameToLayer("GameZone"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
