using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TCPCore
{
    public TcpClient client;
    public string serverHostname;
    public Int32 serverPort;

    static void PrintFormat(string formatStr, params string[] args)
    {
        Debug.Log("~Time: " + Time.time.ToString());
        Debug.Log(string.Format(formatStr, args));
    }

    public TCPCore(string serverHostname, Int32 serverPort)
    {
        this.serverHostname = serverHostname;
        this.serverPort = serverPort;
    }

    public void Connect()
    {
        this.client = new TcpClient(serverHostname, serverPort);
    }

    public string GetMessage()
    {
        if (client is null){
            Debug.Log("~GetMessage->Null Client");
            return string.Empty;
        }
        if (!client.Connected){
            Debug.Log("~GetMessage-Not Connected");
            return string.Empty;
        }
        try
        {
            NetworkStream stream = client.GetStream();

            // Read the first batch of the TcpServer response bytes.
            var data = new Byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            PrintFormat(responseData);
            stream.Close();
            return responseData;
        }
        catch (ArgumentNullException e)
        {
            PrintFormat("ArgumentNullException: {0}", e.ToString());
            return null;
        }
        catch (SocketException e)
        {
            PrintFormat("SocketException: {0}", e.ToString());
            return null;
        }
    }


    public void Close()
    {
        client?.Close();
    }
}


public class TestTCPCore : MonoBehaviour
{
    public TCPCore tcpCore;

    void Start()
    {
        Debug.Log("~Start:Start TCP client");

        var tcpCore = new TCPCore("2.tcp.ngrok.io", 11401);
        tcpCore.GetMessage();

        // tcpCore.Connect();
    }

    private void Update()
    {
        // Debug.Log("~Update");
        // tcpCore.GetMessage();
    }

    private void OnDestroy()
    {
        //tcpCore.Close();
    }
}
