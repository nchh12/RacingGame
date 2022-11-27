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
        Debug.Log(string.Format(formatStr, args));
    }

    public TCPCore(string serverHostname, Int32 serverPort)
    {
        this.serverHostname = serverHostname;
        this.serverPort = serverPort;
    }

    public void Connect()
    {
        client = new TcpClient(serverHostname, serverPort);
    }

    public string GetMessage()
    {
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
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start TCP client");

        var tcpCore = new TCPCore("localhost", 9090);
        tcpCore.Connect();
        tcpCore.GetMessage();
        tcpCore.Close();


        Debug.Log("Stop TCP client");
    }

    private void Update()
    {
        
    }
}
