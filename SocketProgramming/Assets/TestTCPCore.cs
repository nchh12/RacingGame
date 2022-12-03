using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class _TCP
{
    static int TIMEOUT_MIL = 10000;

    public static string StaticResponse { get; set; } = null;
    public static string StaticRequest { get; set; } = null;
    public static NetworkStream stream { get; set; } = null;
    public static TcpClient tcpClient { get; set; } = null;


    public static async Task ConnectAsTcpClient(string ip, int port)
    {

        try
        {
            await Task.Delay(millisecondsDelay: 1000);
            tcpClient = new TcpClient();
            Debug.Log("[Client] Attempting connection to server " + ip + ":" + port);
            Task connectTask = tcpClient.ConnectAsync(ip, port);
            Task timeoutTask = Task.Delay(millisecondsDelay: TIMEOUT_MIL);

            if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
            {
                throw new TimeoutException();
            }

            Debug.Log("[Client] Connected to server");
            stream = tcpClient.GetStream(); 
        }
        catch (TimeoutException)
        {
            Debug.Log("Timeout!");
        }

    }

    public static async Task ListenResponse()
    {
        while (true)
        {
         
            if (_TCP.stream == null)
            {
                Debug.Log("~ListenResponse->Stream: Null");
                await Task.Delay(millisecondsDelay: 1000);
                continue;
            }

            using (var reader = new StreamReader(stream))
            {
                for (; ; )
                {
                    Debug.Log("~ListenResponse->Loop...");
                    try
                    {
                        for (; ; )
                        {
                            Debug.Log("~ListenResponse->Start Async Read");
                            var response = await reader.ReadLineAsync();
                            if (response == null) { break; }
                            Debug.Log(string.Format("[Client] Server response was '{0}'", response));
                            //if (response.Length > 0)
                            //{
                            //    _TCP.StaticResponse = response;
                            //}
                        }
                        Debug.Log("[Client] Server disconnected");
                    }
                    catch (IOException)
                    {
                        Debug.Log("[Client] Server disconnected");
                    }
                }
            }
        }

        }
}

public class TestTCPCore : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 10;
        Debug.Log("~Start:Start TCP client");
        Task.Run(() => _TCP.ConnectAsTcpClient("6.tcp.ngrok.io", 18008));
        Task.Run(() => _TCP.ListenResponse());

    }

    void Update()
    {
        //Debug.Log("~Update->FrameCount:" + Time.frameCount);
        if(Time.frameCount == 60)
        {
            if (!_TCP.tcpClient.Connected)
            {
                Debug.Log("~Update->Not Connnect????");
                throw new Exception("very sadddddd");
            }
            if (_TCP.stream is null)
            {
                throw new Exception("so sadddddd");
            }

            using var writer = new StreamWriter(_TCP.stream) { AutoFlush = true };
            string sendData = "{\"payload\":{\"username\":\"client - 1\"},\"type\":\"CLIENT_JOIN_ROOM\"}";
            Debug.Log("~Update->Start Task");
            Task.Run(async () =>
            {
                Debug.Log("~Update->Start WritelineAsync");
                await writer.WriteLineAsync(sendData);
                Debug.Log("~Update->Done WritelineAsync");
            });
            Debug.Log("~Update->End Task");
        }
        //if (_TCP.StaticRequest != null) return;

        //if(_TCP.StaticResponse != null && _TCP.StaticResponse.Length > 0)
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