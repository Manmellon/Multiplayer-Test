using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class Game : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private BoxCollider2D _gameZoneCollider;
    [SerializeField] private bool _gameStarted;
    public bool GameStarted => _gameStarted;

    [SerializeField] private List<Player> _alivePlayers = new List<Player>();
    public List<Player> AlivePlayers => _alivePlayers;

    public static Game singleton;

    private void Awake()
    {
        if (singleton == null) singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        float posX = Random.Range(_gameZoneCollider.bounds.center.x - _gameZoneCollider.bounds.extents.x, _gameZoneCollider.bounds.center.x + _gameZoneCollider.bounds.extents.x);
        float posY = Random.Range(_gameZoneCollider.bounds.center.y - _gameZoneCollider.bounds.extents.y, _gameZoneCollider.bounds.center.y + _gameZoneCollider.bounds.extents.y);
        Player player = PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(posX, posY, 0), Quaternion.identity).GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                UIGame.singleton.ShowWaitText(false);
            }
        }
    }

    public void SetGameStarted(bool state)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _gameStarted = state;

        if (_gameStarted)
        {
            UIGame.singleton.ShowWaitText(false);
        }
    }

    public void AddPlayer(Player player)
    {
        _alivePlayers.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        _alivePlayers.Remove(player);
        if (_alivePlayers.Count <= 1)
        {
            UIGame.singleton.ShowGameOver(_alivePlayers.Count == 0);
        }
    }

    public void QuitFromGame()
    {
        PhotonNetwork.LeaveRoom();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
