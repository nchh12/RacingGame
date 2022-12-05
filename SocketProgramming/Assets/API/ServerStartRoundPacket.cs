using System.Collections.Generic;
using Newtonsoft.Json;


namespace PacketHandler
{
    public class ServerStartRoundPacket : IPacket
    {
        //'{
        //"payload":
        //    {
        //        "round":2,
        //        "question":"7 % 67",
        //        "listRankedUser":[
        //            {"score":0,"id":"client-id-b62125f7-125f-451d-83f0-5b1ee450c8b0","username":"client-3"},
        //            {"score":0,"id":"client-id-e253f076-24cd-4e8d-b10d-56eabe8ffd66","username":"client-1"},
        //            {"score":0,"id":"client-id-78bfb0da-063e-42b4-a0df-332a3ce232fb","username":"client-2"},
        //            {"score":0,"id":"client-id-3355aed1-c550-4de0-b583-1fc2bbd012de","username":"yau2de"}]},
        //"type":"SERVER_START_ROUND"}'

        public int round;
        public string question;
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
