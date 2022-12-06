using System.Collections.Generic;
using Newtonsoft.Json;


namespace PacketHandler
{
    public class ServerAllowJoinRoom : IPacket
    {
        //const string _sample = "{ \"payload\":{ \"username\":\"client-1\" }, \"type\":\"CLIENT_JOIN_ROOM\" }";
        //'{
        //  "payload":{
        //      "listUsers":["client-1","client-2","client-3","client-1","client-1","client-1","client-1"],
        //      "room":"room-id-613c5a8b-0c6d-42fc-ac70-a95d3df2a848"},
        //  "type":"SERVER_ALLOW_JOIN_ROOM"
        //  }'

        public string room;
        public List<string> listUsers;

        string IPacket.GetType()
        {
            return "SERVER_ALLOW_JOIN_ROOM";
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public ServerAllowJoinRoom(string room, List<string> listUsers) { this.room = room; this.listUsers = listUsers; }
    }

}
