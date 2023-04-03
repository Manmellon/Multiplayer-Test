using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField creatingRoomNameInputField;
    [SerializeField] private TMP_InputField joiningRoomNameInputField;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        createRoomButton.onClick.AddListener( () => CreateRoom() );
        joinRoomButton.onClick.AddListener( () => JoinRoom() );
        exitButton.onClick.AddListener( () => QuitGame() );
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(creatingRoomNameInputField.text, roomOptions);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joiningRoomNameInputField.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
