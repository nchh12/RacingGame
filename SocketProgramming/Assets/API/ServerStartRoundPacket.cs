using System.Collections.Generic;
using Newtonsoft.Json;


namespace PacketHandler
{
    public class ServerStartRoundPacket : IPacket
    {
        //        {
        //  "payload": {
        //    "round": 2,
        //    "question": "86 - 73",
        //    "answer": "13",
        //    "listRankedUser": [
        //      {
        //        "score": 0,
        //        "id": "client-id-489812fc-207d-49eb-be8e-cd7a7b7b5975",
        //        "username": "client-3"
        //      },
        //      {
        //        "score": 0,
        //        "id": "client-id-2934c76e-00ea-4881-9abd-34be8c2a44c5",
        //        "username": "client-1"
        //      }
        //    ]
        //  },
        //  "type": "SERVER_START_ROUND"
        //}

        public int round;
        public string question;
        public string answer;

        public List<Dictionary<string, string>> listRankedUser;

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        string IPacket.GetType()
        {
            return "SERVER_START_ROUND";
        }
    }

}
