using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
