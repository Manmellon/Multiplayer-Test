using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;

public class UIGame : MonoBehaviour
{
    [Header("Main Screen")]
    [SerializeField] private TextMeshProUGUI waitPlayersText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Button _pauseButton;

    [SerializeField] private FixedJoystick _fixedJoystick;
    public FixedJoystick FixedJoystick => _fixedJoystick;

    [Header("Pause Screen")]
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _toLobbyButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TextMeshProUGUI _gameOverText;

    public static UIGame singleton;

    private void Awake()
    {
        if (singleton == null) singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _pauseButton.onClick.AddListener( () => _pauseScreen.SetActive(true) );
        _resumeButton.onClick.AddListener( () => _pauseScreen.SetActive(false) );
        _toLobbyButton.onClick.AddListener(() => { PhotonNetwork.LeaveRoom(); PhotonNetwork.LoadLevel("Lobby"); });
        _exitButton.onClick.AddListener( () => Game.singleton.QuitFromGame() );
    }

    // Update is called once per frame
    void Update()
    {
        waitPlayersText.text = "Ожидание игроков: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public void ShowWaitText(bool show)
    {
        waitPlayersText.gameObject.SetActive(show);
    }

    public void UpdateCoins(int coins)
    {
        coinsText.text = coins.ToString();
    }

    public void ShowGameOver(bool draw)
    {
        _resumeButton.gameObject.SetActive(false);

        if (draw)
        {
            _gameOverText.text = "Ничья. Все умерли";
        }
        else
        {
            _gameOverText.text = "Победитель - " + Game.singleton.AlivePlayers[0].photonView.Owner.NickName;
            _gameOverText.text += "\nМонет собрано - " + Game.singleton.AlivePlayers[0].Coins;
        }

        _gameOverText.gameObject.SetActive(true);

        _pauseScreen.SetActive(true);
    }
}
