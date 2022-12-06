using System.Collections.Generic;
using Newtonsoft.Json;


namespace PacketHandler
{
    public class ClientAnswer : IPacket
    {

        public int round;
        public string answer;

        public ClientAnswer(int round, string answer)
        {
            this.round = round;
            this.answer = answer;
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        string IPacket.GetType()
        {
            return "CLIENT_ANSWER";
        }
    }

}
