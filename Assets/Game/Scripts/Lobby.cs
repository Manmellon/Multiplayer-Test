using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Lobby : MonoBehaviourPunCallbacks
{
    public static Lobby singleton;

    private void Awake()
    {
        if (singleton == null) singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void CreateRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (returnCode == 32766)
        {
            UILobby.singleton.SetInfoText("������� � ����� ������ ��� ����������", true);
        }
        else
        {
            UILobby.singleton.SetInfoText(returnCode + ": " + message, true);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        UILobby.singleton.SetInfoText("������������ ���� � ������� " + UILobby.singleton.GetJoiningRoomName(), false);
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == 32765)
        {
            UILobby.singleton.SetInfoText("������� ���������", true);
        }
        else if (returnCode == 32764)
        {
            UILobby.singleton.SetInfoText("������ ������� ���� �������", true);
        }
        else if (returnCode == 32758)
        {
            UILobby.singleton.SetInfoText("������� � ����� ������ �� ����������", true);
        }
        else
        {
            UILobby.singleton.SetInfoText(returnCode + ": " + message, true);
        }
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
