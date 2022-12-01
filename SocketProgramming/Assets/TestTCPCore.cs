using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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

    public static void SetTcpKeepAlive(Socket socket, uint keepaliveTime, uint keepaliveInterval)
    {
        /* the native structure
        struct tcp_keepalive {
        ULONG onoff;
        ULONG keepalivetime;
        ULONG keepaliveinterval;
        };
        */

        // marshal the equivalent of the native structure into a byte array
        uint dummy = 0;
        byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
        BitConverter.GetBytes((uint)(keepaliveTime)).CopyTo(inOptionValues, 0);
        BitConverter.GetBytes((uint)keepaliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
        BitConverter.GetBytes((uint)keepaliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);

        // write SIO_VALS to Socket IOControl
        socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
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
            PrintFormat("Message: {0}", responseData);
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
        

        tcpCore = new TCPCore("2.tcp.ngrok.io", 11401);
        tcpCore.Connect();
        // tcpCore.GetMessage();
    }

    void Update()
    {
        Debug.Log("~Update");
        if (tcpCore is null) return;
        tcpCore.GetMessage();
    }

    void OnDestroy()
    {
        //tcpCore.Close();
    }
}
