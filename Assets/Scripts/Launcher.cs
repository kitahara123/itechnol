using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace ITechnol
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_InputField inputNameField;
        [SerializeField] private TMP_InputField roomNameField;
        [SerializeField] private TMP_InputField roomMaxPpl;
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private TextMeshProUGUI progressLabel;

        const string playerNamePrefKey = "PlayerName";
        const string roomNamePrefKey = "RoomName";

        private string roomName;
        private string gameVersion = "1";

        void Awake() => PhotonNetwork.AutomaticallySyncScene = true;

        void Start()
        {
            var defaultName = string.Empty;

            inputNameField.text = defaultName = PlayerPrefs.GetString(playerNamePrefKey, "Player");
            roomName = roomNameField.text = PlayerPrefs.GetString(roomNamePrefKey, "Room1");

            PhotonNetwork.NickName = defaultName;

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.SendRate = 50;
                PhotonNetwork.SerializationRate = 50;
            }

            progressLabel.gameObject.SetActive(false);
            controlPanel.SetActive(true);
        }

        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }

            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        public void SetRoomName(string value)
        {
            roomName = value;
            PlayerPrefs.SetString(roomNamePrefKey, value);
        }

        public void Connect()
        {
            progressLabel.gameObject.SetActive(true);
            controlPanel.SetActive(false);
            if (PhotonNetwork.IsConnected)
            {
                if (string.IsNullOrEmpty(roomName))
                    PhotonNetwork.JoinRandomRoom();
                else
                    PhotonNetwork.JoinRoom(roomName);
            }
            else
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnect");
            progressLabel.gameObject.SetActive(false);
            controlPanel.SetActive(true);
        }

        public override void OnJoinRoomFailed(short returnCode, string message) => CreateRoom();
        public override void OnJoinRandomFailed(short returnCode, string message) => CreateRoom();

        private void CreateRoom()
        {
            Debug.Log("Create " + roomName + " " + roomMaxPpl.text);
            PhotonNetwork.CreateRoom(roomName, new RoomOptions {MaxPlayers = byte.Parse(roomMaxPpl.text)});
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined " + roomName + " " + roomMaxPpl.text);
            if (!PhotonNetwork.IsMasterClient) return;
            PhotonNetwork.LoadLevel(1);
        }
    }
}