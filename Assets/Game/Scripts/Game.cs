using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Game : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] TextMeshProUGUI waitPlayersText;
    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private bool _gameStarted;
    public bool GameStarted => _gameStarted;

    public static Game singleton;

    private void Awake()
    {
        if (singleton == null) singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        waitPlayersText.text = "Игроков: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && PhotonNetwork.IsMasterClient)
        {
            // We own this player: send the others our data
            stream.SendNext(_gameStarted);
        }
        else if (stream.IsReading && !PhotonNetwork.IsMasterClient && info.Sender.IsMasterClient)
        {
            // Network player, receive data
            _gameStarted = (bool)stream.ReceiveNext();

            if (_gameStarted)
            {
                waitPlayersText.gameObject.SetActive(false);
            }
        }
    }

    public void SetGameStarted(bool state)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _gameStarted = state;

        if (_gameStarted)
        {
            waitPlayersText.gameObject.SetActive(false);
        }
    }
}
