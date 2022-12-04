using Newtonsoft.Json;


namespace PacketHandler
{
    public class JoinRoomPacket : IPacket
    {
        public string username;
        //const string _sample = "{ \"payload\":{ \"username\":\"client-1\" }, \"type\":\"CLIENT_JOIN_ROOM\" }";

        string IPacket.GetType()
        {
            return "CLIENT_JOIN_ROOM";
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public JoinRoomPacket(string username) { this.username = username; }
    }
}
