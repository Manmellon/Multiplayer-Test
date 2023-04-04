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
        //photonView.TransferOwnership(PhotonNetwork.MasterClient);

        _rigidbody2D.velocity = transform.right * speed;
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

            if (player.photonView.Owner.Equals(photonView.Owner)) return;

            /*if (PhotonNetwork.IsMasterClient)
            {
                player.DealDamage(damage);
                //PhotonNetwork.Destroy(gameObject);
            }*/
            player.DealDamage(damage);

            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /*if (PhotonNetwork.IsMasterClient && collision.gameObject.layer == LayerMask.NameToLayer("GameZone"))
        {
            PhotonNetwork.Destroy(gameObject);
        }*/
        if (collision.gameObject.layer == LayerMask.NameToLayer("GameZone"))
        {
            Destroy(gameObject);
        }
    }
}
