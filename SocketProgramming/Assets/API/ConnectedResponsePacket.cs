using Newtonsoft.Json;


namespace PacketHandler
{
    //public class TypePacket : Packet
    //{
    //    public string ToJsonString()
    //    {
    //        return JsonConvert.SerializeObject(this);
    //    }

    //    string Packet.GetType()
    //    {
    //        return "__NONE__";
    //    }
    //}

    public class ConnectedResponsePacket : IPacket
    {
        //{"payload":{"id":"client-id-b03aab33-efdb-4104-8ae4-d43c07e2a2df"},"type":"SERVER_NEW_USER"}
        public string id;

        public ConnectedResponsePacket(string id) { this.id = id; }

        string IPacket.GetType()
        {
            return "SERVER_NEW_USER";
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
