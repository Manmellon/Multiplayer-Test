using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Components")]
    //[SerializeField] private PhotonView photonView;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Rigidbody2D _rigidbody;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Slider healthSlider;

    [Header("Settings")]
    [SerializeField] private float walkSpeed = 3.0f;

    private float prevHorizontal;
    private float prevVertical;

    [Header("Gameplay")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float curHealth;

    [SerializeField] private bool isFiring;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _fireSource;

    [SerializeField] private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = curHealth;

        nameText.text = photonView.Owner.NickName;

        prevHorizontal = 0;
        prevVertical = -0.1f;

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Game.singleton.SetGameStarted(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = curHealth;

        if (!photonView.IsMine) return;

        if (!Game.singleton.GameStarted) return;

        if (isDead) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        spriteRenderer.flipX = horizontalInput > 0 || prevHorizontal > 0;

        if (horizontalInput != 0 || verticalInput != 0)
        {
            _animator.SetFloat("SpeedX", horizontalInput);
            _animator.SetFloat("SpeedY", verticalInput);

            prevHorizontal = horizontalInput;
            prevVertical = verticalInput;
        }
        else
        {
            _animator.SetFloat("SpeedX", prevHorizontal / 2);
            _animator.SetFloat("SpeedY", prevVertical / 2);
        }

        _rigidbody.velocity = new Vector2(horizontalInput, verticalInput).normalized * walkSpeed;

        if (Input.GetMouseButtonDown(0))
        {
            float rotationAngle = Mathf.Rad2Deg * Mathf.Atan2(prevVertical, prevHorizontal);
            PhotonNetwork.Instantiate(_bulletPrefab.name, _fireSource.position, Quaternion.Euler(0, 0, rotationAngle));
            //PhotonNetwork.InstantiateRoomObject(_bulletPrefab.name, _fireSource.position, Quaternion.Euler(0, 0, rotationAngle));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(isFiring);
            //stream.SendNext(curHealth);
            stream.SendNext(spriteRenderer.flipX);
            stream.SendNext(isDead);
        }
        else
        {
            // Network player, receive data
            isFiring = (bool)stream.ReceiveNext();
            //curHealth = (float)stream.ReceiveNext();
            spriteRenderer.flipX = (bool)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void DealDamage(float damage)
    {
        //if (!PhotonNetwork.IsMasterClient) return;

        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);

        if (curHealth <= 0)
        {
            _animator.Play("Death", 0);
            isDead = true;
            _collider.enabled = false;
        }
    }

    public void DestroyAfterDeathAnimation()
    {
        //if (!PhotonNetwork.IsMasterClient) return;

        //PhotonNetwork.Destroy(gameObject);
        Destroy(gameObject);
    }
}
