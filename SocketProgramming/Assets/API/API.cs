using System;

using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class API
{

    // SINGLE-TON API
    private static API _singleton = null;
    private API() { }
    public static API Instance
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = new API();
            }
            return _singleton;
        }
    }


    public const int TIMEOUT_MIL = 2000;
    public const int TRY_INTERVAL = 1000;
    static public TcpClient tcpClient;
    static public NetworkStream stream;
    public string ip { get; internal set; }
    public int port { get; internal set; } 

    public async Task ConnectAsTcpClient(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        int tryLeft = 10;
        for(; tryLeft > 0; tryLeft--)
        {
            Debug.Log("[Client] - Trial left: " + tryLeft);
            await Task.Delay(millisecondsDelay: TRY_INTERVAL);
            tcpClient = new TcpClient();
            Debug.Log("[Client] Attempt connection to server " + ip + ":" + port);

            Task connectTask = tcpClient.ConnectAsync(ip, port);
            Task timeoutTask = Task.Delay(millisecondsDelay: TIMEOUT_MIL);

            if (await Task.WhenAny(connectTask, timeoutTask) == connectTask)
            {
                if (tcpClient.Connected)
                    break;
            }

            if (tryLeft == 0)
            {
                throw new TimeoutException("~ConnectAsTcpClient->Time out after 10 trail!");
            }
        }

        Debug.Log("[STATUS] - isConnect: " + tcpClient.Connected);
        Debug.Log("[Client] Connected to server");
        stream = tcpClient.GetStream();
    }

    public async Task SendData(string data)
    {
        if (stream is null)
        {
            Debug.Log("~SendData: Streamer is Null");
            return;
        }
        var reader = new StreamWriter(stream) { AutoFlush = true };
        await reader.WriteLineAsync(data);
    }

    public async Task ListenResponse()
    {
        while (true)
        {

            if (stream == null)
            {
                Debug.Log("~ListenResponse->Stream: Null");
                await Task.Delay(millisecondsDelay: 1000);
                //if(tcpClient != null)
                //{
                //    stream = tcpClient.GetStream();
                //}
                continue;
            }

            var reader = new StreamReader(stream);
            for (; ; )
            {
                try
                {
                    for (; ; )
                    {
                        Debug.Log("~ListenResponse->Start Async Read");
                        var response = await reader.ReadLineAsync();
                        if (response == null) { break; }
                        Debug.Log(string.Format("[Client] Server response was '{0}'", response));

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

    public void ConnectAndListen(string ip, int port)
    {
        Task connectTask = API.Instance.ConnectAsTcpClient(ip, port);
        Task listenResponseTask = API.Instance.ListenResponse();
        Task.Run(() => connectTask);
        Task.Run(() => listenResponseTask);

    }

}



//using PacketHandler;
//using System.Text;

//public class _TCP
//{
//    static int TIMEOUT_MIL = 10000;

//    public static string StaticResponse { get; set; } = null;
//    public static string StaticRequest { get; set; } = null;
//    public static NetworkStream stream { get; set; } = null;
//    public static TcpClient tcpClient { get; set; } = null;

//    public static async Task ConnectAsTcpClientRaw(string ip, int port)
//    {
//        for (; ; )
//        {
//            try
//            {
//                await Task.Delay(millisecondsDelay: 1000);
//                using (var tcpClient = new TcpClient())
//                {
//                    Debug.Log("[Client] Attempting connection to server " + ip + ":" + port);
//                    Task connectTask = tcpClient.ConnectAsync(ip, port);
//                    Task timeoutTask = Task.Delay(millisecondsDelay: 100);
//                    if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
//                    {
//                        throw new TimeoutException();
//                    }

//                    Debug.Log("[Client] Connected to server");
//                    using (var networkStream = tcpClient.GetStream())
//                    using (var reader = new StreamReader(networkStream))
//                    using (var writer = new StreamWriter(networkStream) { AutoFlush = true })
//                    {
//                        //Debug.Log(string.Format("[Client] Writing request '{0}'", ClientRequestString));
//                        //await writer.WriteLineAsync(ClientRequestString);

//                        try
//                        {
//                            for (; ; )
//                            {
//                                var response = await reader.ReadLineAsync();
//                                if (response == null) { break; }
//                                Debug.Log(string.Format("[Client] Server response was '{0}'", response));
//                            }
//                            Debug.Log("[Client] Server disconnected");
//                        }
//                        catch (IOException)
//                        {
//                            Debug.Log("[Client] Server disconnected");
//                        }
//                    }
//                }
//            }
//            catch (TimeoutException)
//            {
//                // reconnect
//                Debug.Log("[Client] Timeout");
//            }
//        }
//    }


//    public static async Task ConnectAsTcpClient(string ip, int port)
//    {
//        try
//        {
//            await Task.Delay(millisecondsDelay: 1000);
//            tcpClient = new TcpClient();
//            Debug.Log("[Client] Attempting connection to server " + ip + ":" + port);
//            Task connectTask = tcpClient.ConnectAsync(ip, port);
//            Task timeoutTask = Task.Delay(millisecondsDelay: TIMEOUT_MIL);

//            if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
//            {
//                throw new TimeoutException();
//            }
//            Debug.Log("[Task] Task isCompleted: " + connectTask.IsCompleted);
//            Debug.Log("[Client] Connected to server");
//            Debug.Log("[Client] Debug Status: " + tcpClient.Connected);
//            //stream = tcpClient.GetStream();

//            var networkStream = tcpClient.GetStream();
//            //Debug.Log("[Stream] IsDataAvailable->" + networkStream.DataAvailable);
//            var reader = new StreamReader(networkStream);
//            var writer = new StreamWriter(networkStream) { AutoFlush = true };

//            string sendData = "{\"payload\":{\"username\":\"client - 1\"},\"type\":\"CLIENT_JOIN_ROOM\"}";
//            Debug.Log(string.Format("[Client] Writing request '{0}'", sendData));
//            await writer.WriteLineAsync(sendData);

//            try
//            {
//                for (; ; )
//                {
//                    Debug.Log("[Client] Start Read Async");
//                    var response = await reader.ReadLineAsync();
//                    if (response == null) { break; }
//                    Debug.Log(string.Format("[Client] Server response was '{0}'", response));
//                }
//                Debug.Log("[Client] Server disconnected");
//            }
//            catch (IOException)
//            {
//                Debug.Log("[Client] Server disconnected");
//            }

//        }
//        catch (TimeoutException)
//        {
//            Debug.Log("Timeout!");
//        }
//    }

//public static async Task SendData(string data)
//{
//    var reader = new StreamWriter(_TCP.stream) { AutoFlush = true };
//    await reader.WriteLineAsync(data);
//}

//public static async Task ListenResponse()
//    {
//        int count = 10;
//        while (true)
//        {

//            if (_TCP.stream == null)
//            {
//                if (count == 0) return;
//                count--;
//                Debug.Log("~ListenResponse->Stream: Null");
//                await Task.Delay(millisecondsDelay: 1000);
//                continue;
//            }

//            using (var reader = new StreamReader(stream))
//            {
//                for (; ; )
//                {
//                    //Debug.Log("~ListenResponse->Loop...");
//                    try
//                    {
//                        for (; ; )
//                        {
//                            Debug.Log("~ListenResponse->Start Async Read");
//                            var response = await reader.ReadLineAsync();
//                            if (response == null) { break; }
//                            //Debug.Log(string.Format("[Client] Server response was '{0}'", response));
//                            Debug.Log(response);
//                            //if (response.Length > 0)
//                            //{
//                            //    _TCP.StaticResponse = response;
//                            //}
//                        }
//                        Debug.Log("[Client] Server disconnected");
//                    }
//                    catch (IOException)
//                    {
//                        Debug.Log("[Client] Server disconnected");
//                    }
//                }
//            }
//        }
//    }
//}
