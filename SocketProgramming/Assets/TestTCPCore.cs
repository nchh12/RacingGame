using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TCPCore
{
    public TcpClient client;
    public string serverHostname;
    public Int32 serverPort;
    public NetworkStream stream;


    static void PrintFormat(string formatStr, params string[] args)
    {
        Debug.Log("~Time " + Time.time.ToString() + ": " +  string.Format(formatStr, args));
    }

    public TCPCore(string serverHostname, Int32 serverPort)
    {
        this.serverHostname = serverHostname;
        this.serverPort = serverPort;
    }

    
    public void Connect()
    {
        //if (client is not null) return;
        //if (stream is not null) return;

        this.client = new TcpClient(serverHostname, serverPort);
        this.stream = client?.GetStream();
        //this.stream.ReadTimeout = 1000;
        Debug.Log("~Connect->isConnected: " + this.client.Connected);
    }

    async public Task ListenForMessage()
    {
    
        this.Connect();
        this.stream = this.client.GetStream();
        if (stream is null)
        {
            Debug.Log("~ListenForMessage->Null Client");
            return;
        }

        //try
        //{
        while (true)
        {
            Debug.Log("~ListenForMessage->Loop...");
            //var data = new Byte[256 * 100];
               
            //var bytes = this.stream.Read(data, 0, data.Length);
            var streamReader = new StreamReader(stream);
            var str = streamReader.ReadLine();
            
            //var writer = new StreamWriter(stream);

            //PrintFormat("{0}", "Bytes read:" + bytes);
            PrintFormat("[TCP] - {0}", str);
            //if (bytes > 0)
            //{
            //    string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //    PrintFormat("[TCP] - Recieved: {0}", responseData);
            //}
        }

        //string sendData = "{\"payload\":{\"username\":\"client - 1\"},\"type\":\"CLIENT_JOIN_ROOM\"}";
        //var sendByte = System.Text.Encoding.ASCII.GetBytes(sendData);
        //await stream.WriteAsync(sendByte, 0, sendByte.Length);
        //PrintFormat("[TCP] - Sending: {0}", sendData);


        //}
        //catch (ArgumentNullException e)
        //{
        //    PrintFormat("ArgumentNullException: {0}", e.ToString());
        //}
        //catch (SocketException e)
        //{
        //    //stream?.Close();
        //    PrintFormat("SocketException: {0}", e.ToString());
        //}
    }

    async public Task GetMessage()
    {
        PrintFormat("~GetMessage->Start Task", "");

        if (stream is null)
        {
            Debug.Log("~GetMessage->Null Client");
        }

        try
        {   
            var data = new Byte[256 * 100];
            var bytes = await stream.ReadAsync(data, 0, data.Length);

            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            PrintFormat("[TCP] - Recieved: {0}", responseData);

            string sendData = "{\"payload\":{\"username\":\"client - 1\"},\"type\":\"CLIENT_JOIN_ROOM\"}";
            var sendByte = System.Text.Encoding.ASCII.GetBytes(sendData);
            await stream.WriteAsync(sendByte, 0, sendByte.Length);
            PrintFormat("[TCP] - Sending: {0}", sendData);

        }
        catch (ArgumentNullException e)
        {
            PrintFormat("ArgumentNullException: {0}", e.ToString());
        }
        catch (SocketException e)
        {
            stream?.Close();
            PrintFormat("SocketException: {0}", e.ToString());
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
        Application.targetFrameRate = 10;
        Debug.Log("~Start:Start TCP client");
        tcpCore = new TCPCore("4.tcp.ngrok.io", 17391);
        //tcpCore.Connect();
        //var data = new Byte[256 * 100];
        //int bytes = tcpCore.stream.Read(data, 0, data.Length);
        //string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        //Debug.Log("~Start->res:" + responseData);
        Task.Run(() => tcpCore.ListenForMessage());
    }

    void Update()
    {
        //Debug.Log("~Update");
        //if (tcpCore is null) return;
        //var readTask = tcpCore.GetMessage();
        //Task.Run(() => readTask);
        //Debug.Log("[STATUS] - isConnected? = " + tcpCore.client.Connected);
    }

    void OnDestroy()
    {
        //tcpCore.Close();
    }
}



//public static void SetTcpKeepAlive(Socket socket, uint keepaliveTime, uint keepaliveInterval)
//{
//    /* the native structure
//    struct tcp_keepalive {
//    ULONG onoff;
//    ULONG keepalivetime;
//    ULONG keepaliveinterval;
//    };
//    */

//    // marshal the equivalent of the native structure into a byte array
//    uint dummy = 0;
//    byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
//    BitConverter.GetBytes((uint)(keepaliveTime)).CopyTo(inOptionValues, 0);
//    BitConverter.GetBytes((uint)keepaliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
//    BitConverter.GetBytes((uint)keepaliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);

//    // write SIO_VALS to Socket IOControl
//    socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
//}
