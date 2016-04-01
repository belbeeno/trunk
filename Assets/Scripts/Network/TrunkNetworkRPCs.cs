using UnityEngine;
using UnityEngine.Networking;

using System.Collections.Generic;

namespace NetMessage
{
    public static class Helpers
    {
        public static void DeserializeList<T>(NetworkReader reader, ref List<T> list) where T : MessageBase, new()
        {
            int listCountRemote = reader.ReadInt32();
            if (listCountRemote < list.Count)
            {
                list.RemoveRange(listCountRemote, list.Count - listCountRemote);
            }
            for (int i = 0; i < listCountRemote; i++)
            {
                while (i > list.Count)
                {
                    list.Add(new T());
                }
                list[i].Deserialize(reader);
            }
        }

        public static void SerializeList<T>(NetworkWriter writer, ref List<T> list) where T : MessageBase
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Serialize(writer);
            }
        }
    }

    public static class ID
    {
        public const short InitSession      = 100;
        public const short Ping             = 101;
        public const short APB              = 102;
    }

    public class InitSessionMsg : MessageBase
    {
        // Don't really do anything yet, but...
        public int citySeed = -1;
        public int pathSeed = -1;
    }
    
    public class PingMsg : MessageBase
    {
        public string msg;
    }

    public class APBRequest : MessageBase
    {
        public Vector2 position;
    }
    public class APBResponse : MessageBase
    {
        public class Trail : MessageBase
        {
            public Vector2 start;
            public Vector2 end;

            public Trail()
            {
                this.start = new Vector2(float.MaxValue, float.MaxValue);
                this.end = new Vector2(float.MaxValue, float.MaxValue);
            }
            public Trail(Vector2 start, Vector2 end)
            {
                this.start = start;
                this.end = end;
            }
            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(start);
                writer.Write(end);
                base.Serialize(writer);
            }
            public override void Deserialize(NetworkReader reader)
            {
                start = reader.ReadVector2();
                end = reader.ReadVector2();
                base.Deserialize(reader);
            }
        }
        public class Hint : MessageBase
        {
            public enum HintType : short
            {
                Wrench = 1,
                Screwdriver,
                Blanket,
                Rope,

                INVALID = -1,
            }

            public Vector2 pos = new Vector2();
            public HintType type = HintType.INVALID;

            public Hint()
            {
                this.pos = new Vector2(float.MaxValue, float.MaxValue);
                this.type = HintType.INVALID;
            }
            Hint(Vector2 pos, HintType type)
            {
                this.pos = pos;
                this.type = type;
            }

            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(pos);
                writer.Write((short)type);
            }
            public override void Deserialize(NetworkReader reader)
            {
                pos = reader.ReadVector2();
                type = (HintType)reader.ReadUInt16();
            }
        }

        public List<Trail> trails = new List<Trail>();
        public List<Hint> hints = new List<Hint>();

        public override void Serialize(NetworkWriter writer)
        {
            Helpers.SerializeList(writer, ref trails);
            Helpers.SerializeList(writer, ref hints);
            base.Serialize(writer);
        }

        public override void Deserialize(NetworkReader reader)
        {
            Helpers.DeserializeList(reader, ref trails);
            Helpers.DeserializeList(reader, ref hints);
            base.Deserialize(reader);
        }
    }


}