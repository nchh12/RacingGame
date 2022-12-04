using System.Collections;
using UnityEngine;
using Newtonsoft.Json;


namespace PacketHandler
{

    public class PacketWrapper<T> where T : IPacket
    {
        public string type;
        public T payload;

        public static PacketWrapper<D> FromData<D>(D _data) where D : IPacket
        {

            return new PacketWrapper<D>(
                type: _data.GetType(),
                payload: _data
                );
        }

        public static PacketWrapper<D> FromString<D>(string jsonString) where D : IPacket
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
}
