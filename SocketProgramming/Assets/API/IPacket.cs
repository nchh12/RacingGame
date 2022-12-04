namespace PacketHandler
{
    public interface IPacket
    {
        public string GetType();

        public string ToJsonString();
    }
}
