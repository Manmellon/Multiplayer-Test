using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.EventSystems;

public class Player : MonoBehaviourPun, IPunObservable
{
    [Header("Components")]
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

    [SerializeField] private int _coins;
    public int Coins => _coins;

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

        UIGame.singleton.UpdateCoins(Coins);

        if (!Game.singleton.GameStarted) return;

        if (isDead) return;

        float horizontalInput = UIGame.singleton.FixedJoystick.Horizontal;
        float verticalInput = UIGame.singleton.FixedJoystick.Vertical;

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
            _animator.SetFloat("SpeedX", prevHorizontal / 20);
            _animator.SetFloat("SpeedY", prevVertical / 20);
        }

        _rigidbody.velocity = new Vector2(horizontalInput, verticalInput).normalized * walkSpeed;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            float rotationAngle = Mathf.Rad2Deg * Mathf.Atan2(prevVertical, prevHorizontal);
            PhotonNetwork.Instantiate(_bulletPrefab.name, _fireSource.position, Quaternion.Euler(0, 0, rotationAngle));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(isFiring);
            stream.SendNext(spriteRenderer.flipX);
            stream.SendNext(isDead);
            stream.SendNext(Coins);
        }
        else
        {
            // Network player, receive data
            isFiring = (bool)stream.ReceiveNext();
            spriteRenderer.flipX = (bool)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();
            _coins = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void DealDamage(float damage)
    {
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
        Game.singleton.RemovePlayer(this);
        Destroy(gameObject);
    }

    public void AddCoin()
    {
        _coins++;
    }
}
