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

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = curHealth;

        nameText.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

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

        healthSlider.value = curHealth;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(isFiring);
            stream.SendNext(curHealth);
        }
        else
        {
            // Network player, receive data
            this.isFiring = (bool)stream.ReceiveNext();
            this.curHealth = (float)stream.ReceiveNext();
        }
    }
}
