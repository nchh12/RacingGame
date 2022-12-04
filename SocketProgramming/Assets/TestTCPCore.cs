using System;
using UnityEngine;
using PacketHandler;

public class TestTCPCore : MonoBehaviour
{
    const string LOCAL_HOTS = "192.168.1.89";
    const int LOCAL_PORT = 5555;

    private string _localString = "";

    void Start()
    {
        Application.targetFrameRate = 10;
        API.Instance.ConnectAndListen(LOCAL_HOTS, LOCAL_PORT);

        Action<string> _listenForConnectedEventPacket = (response) =>
        {
            try
            {
                var wrappedPacket = PacketWrapper<ConnectedResponsePacket>.FromString<ConnectedResponsePacket>(response);
                if (!wrappedPacket.IsValid()) return;
                var _data = wrappedPacket.GetData();
                print("~_listenForConnectedEventPacket->ID: " + _data.id);
                _localString = _data.id;
            }
            catch (Exception)
            {

            }
        };

        Action<string> _listenForJoinPacket = (response) =>
        {
            try
            {
                var wrappedPacket = PacketWrapper<ServerAllowJoinRoom>.FromString<ServerAllowJoinRoom>(response);
                if (!wrappedPacket.IsValid()) return;
                var _data = wrappedPacket.GetData();
                print("~_listenForJoinPacket->ROOM     :" + _data.room);
                print("~_listenForJoinPacket->userLists: " + String.Join(",", _data.listUsers));

            }
            catch (Exception)
            {

            }
        };

        API.Instance.AddHandler(_listenForConnectedEventPacket);
        API.Instance.AddHandler(_listenForJoinPacket);
        ;
    }

    void Update()
    {
        if (Time.frameCount == 60)
        {
            Debug.Log("~Update:->SendData");
            var sendObj = new JoinRoomPacket("yauangon");
            var _mes = PacketWrapper<JoinRoomPacket>.FromData(sendObj).StringifyPayload();
            API.Instance.StartSendTask(_mes);
        }

        if (_localString.Length > 0 && Time.frameCount < 70)
        {
            Debug.Log("~Test->_localString" + _localString);
        }
        
    }

    void OnDestroy()
    {

    }
}