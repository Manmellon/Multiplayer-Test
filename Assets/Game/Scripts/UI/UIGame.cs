using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;

public class UIGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waitPlayersText;

    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private FixedJoystick _fixedJoystick;
    public FixedJoystick FixedJoystick => _fixedJoystick;

    public static UIGame singleton;

    private void Awake()
    {
        if (singleton == null) singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        waitPlayersText.text = "ќжидание игроков: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public void ShowWaitText(bool show)
    {
        waitPlayersText.gameObject.SetActive(show);
    }

    public void UpdateCoins(int coins)
    {
        coinsText.text = coins.ToString();
    }
}
