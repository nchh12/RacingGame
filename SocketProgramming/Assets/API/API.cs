using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using PacketHandler;
using System.Collections.Generic;

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

    // Setting
    public const int TIMEOUT_MIL = 2000;
    public const int TRY_INTERVAL = 1000;
    public const int MAX_N_TRIAL = 2;

    static public TcpClient tcpClient;
    static public NetworkStream stream;

    public string IP { get; internal set; }
    public int Port { get; internal set; } 

    public async Task ConnectAsTcpClient(string ip, int port)
    {
        this.IP = ip;
        this.Port = port;
        int tryLeft = MAX_N_TRIAL;
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

            if (tryLeft == 1)
            {
                throw new TimeoutException("~ConnectAsTcpClient->Time out after 10 trail!");
            }
        }

        Debug.Log("[Client] - isConnect: " + tcpClient.Connected);
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

    public void StartSendTask(string data)
    {
        Task.Run(() => API.Instance.SendData(data));
    }

    private async Task<bool> waitForConnection()
    {
        // 2 more trial to compensate
        // for the connection lag
        int n_trial = MAX_N_TRIAL + 2;
        while(stream == null && n_trial > 0)
        {
            n_trial--;
            //Debug.Log("~waitForConnection->Waiting until connection is established...");
            await Task.Delay(millisecondsDelay: TRY_INTERVAL);
            continue;
        }

        if (n_trial == 0) return false;
        return true;
    }

    public async Task ListenResponse()
    {
        // Waitfor the connection is established
        var _connected = await waitForConnection();
        if (!_connected)
        {
            Debug.Log("~ListenResponse -> Failed to get connection");
            return;
        }


        var reader = new StreamReader(stream);

        // Restart lisntening if there is Exception
        for (; ; )
        {
            // If connection is drop -> immediately go return
            if (!tcpClient.Connected) return;
            try
            {
                // Inf. loop to wait and read packets
                for (; ; )
                {
                    if (!tcpClient.Connected) return;
                    var response = await reader.ReadLineAsync();
                    if (response == null) { break; }
                    Debug.Log(string.Format("[Client] Server response was '{0}'", response));

                    // Skip Invalid Packet
                    if (response.Length == 0) continue;

                    foreach (var handler in _handlerList)
                    {
                        handler(response);
                    }

                }
                Debug.Log("[Client] Server disconnected");
            }
            catch (IOException)
            {
                Debug.Log("[Client] Server disconnected");
            }
            catch (Exception e)
            {
                Debug.Log("[Client] - Exception: " + e.Message);
            }
            //}
            
        }
    }

    public void ConnectAndListen(string ip, int port)
    {
        Task connectTask = API.Instance.ConnectAsTcpClient(ip, port);
        Task listenResponseTask = API.Instance.ListenResponse();
        Task.Run(() => connectTask);
        Task.Run(() => listenResponseTask);

    }

    // Handler Logic
    private List<Action<string>> _handlerList = new List<Action<string>>();

    public void AddHandler(Action<string> handler)
    {
        _handlerList.Add(handler);
    }

    public void RemoveHandler(Action<string> handler)
    {
        _handlerList.Remove(handler);
    }

}