using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


namespace PacketHandler
{
    public interface Packet
    {
        public string GetType();
    }

    public class PacketWrapper<T> where T : Packet
    {
        public string type;
        public T payload;

        public static PacketWrapper<D> FromString<D>(string jsonString) where D : Packet
        {
            PacketWrapper<D> _wrapper = JsonConvert.DeserializeObject<PacketWrapper<D>>(jsonString);

            return new PacketWrapper<D>(
                type: _wrapper.type,
                payload: _wrapper.payload
                );
        }

        public PacketWrapper(string type, T payload)
        {
            this.type = type;
            this.payload = payload;
            //this._data = data;
        }

        public T GetData()
        {
            return payload;
        }

        public bool IsValid()
        {
            return this.type == this.payload.GetType();
        }


        public string StringifyPayload()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class TypePacket : Packet
    {
        string Packet.GetType()
        {
            return "__NONE__";
        }
    }

    public class ConnectedResponsePacket : Packet
    {
        //{"payload":{"id":"client-id-b03aab33-efdb-4104-8ae4-d43c07e2a2df"},"type":"SERVER_NEW_USER"}
        public string id;

        public ConnectedResponsePacket(string id){ this.id = id; }

        string Packet.GetType()
        {
            return "SERVER_NEW_USER";
        }
    }

    public class JoinRoomPacket : Packet
    {
        public string username;
        //const string _sample = "{ \"payload\":{ \"username\":\"client-1\" }, \"type\":\"CLIENT_JOIN_ROOM\" }";

        string Packet.GetType()
        {
            return "CLIENT_JOIN_ROOM";
        }

        public JoinRoomPacket(string username) { this.username = username; }
    }
}
