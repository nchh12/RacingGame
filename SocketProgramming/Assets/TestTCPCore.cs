using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using PacketHandler;

public class TestTCPCore : MonoBehaviour
{
    const string LOCAL_HOTS = "172.16.2.224";
    const int LOCAL_PORT = 5555;

    void Start()
    {
        Application.targetFrameRate = 10;
        Debug.Log("~Start:Start TCP client");

        API.Instance.ConnectAndListen(LOCAL_HOTS, LOCAL_PORT);
        

        //const string path = "/Users/hung.nh/codespace/yauangon/RacingGame/join_test.json";
        //StringBuilder builder = new StringBuiler();
        //string result;
        //using (StreamReader reader = File.OpenText(path))
        //{
        //    result = reader.ReadLine();
        //    Debug.Log("ReadFile: " + result);
        //}
        //string result = "{\"payload\":{\"id\":\"client-id-b03aab33-efdb-4104-8ae4-d43c07e2a2df\"},\"type\":\"SERVER_NEW_USER\"}";

        //var packet = PacketWrapper<TypePacket>.FromString<TypePacket>(result);
        //Debug.Log("packet->type: " + packet.type);
        //Debug.Log("packet->payload: " + packet.payload);
        //Debug.Log(packet.GetData().GetType());
    }

    void Update()
    {
        if (Time.frameCount == 60)
        {
            Debug.Log("~Update:->SendData");
            string sendData = "{\"payload\": { \"username\":\"client-1\" }, \"type\":\"CLIENT_JOIN_ROOM\" }";
            Task.Run(() => API.Instance.SendData(sendData));
        }
        

        //Debug.Log(packet.Data.Username);
        //Task.Run(() => _TCP.SendData(sendData));

        //if (!_TCP.tcpClient.Connected)
        //{
        //    Debug.Log("~Update->Not Connnect????");
        //    throw new Exception("very sadddddd");
        //}
        //if (_TCP.stream is null)
        //{
        //    throw new Exception("so sadddddd");
        //}

        //var writer = new StreamWriter(_TCP.stream) { AutoFlush = true };
        //string sendData = "{\"payload\":{\"username\":\"client - 1\"},\"type\":\"CLIENT_JOIN_ROOM\"}";
        //Debug.Log("~Update->Start Task");
        //Task.Run(async () =>
        //{
        //    Debug.Log("~Update->Start WritelineAsync");
        //    await writer.WriteLineAsync(sendData);
        //    await writer.FlushAsync();
        //    Debug.Log("~Update->Done WritelineAsync");
        //});
        //Debug.Log("~Update->End Task");
        //}

        //if (_TCP.StaticRequest != null) return;

        //if (_TCP.StaticResponse != null && _TCP.StaticResponse.Length > 0)
        //{
        //    Debug.Log("~Update->StaticResponse: " + _TCP.StaticResponse);
        //    string sendData = "{\"payload\":{\"username\":\"client - 1\"},\"type\":\"CLIENT_JOIN_ROOM\"}";
        //    _TCP.StaticRequest = sendData;
        //}
    }

    void OnDestroy()
    {

    }
}