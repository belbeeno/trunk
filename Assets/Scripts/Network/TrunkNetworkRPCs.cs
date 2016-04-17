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
                while (i >= list.Count)
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
        public const short GameOver         = 200;
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
        public Vector3 position;
    }
    public class APBResponse : MessageBase
    {
        public class Hint : MessageBase
        {
            public enum HintType : short
            {
                Wrench = 1,
                Screwdriver,
                Crowbar,
                TireIron,
                Blanket,
                Rope,
                Hostage,        // <- if you get this, you've won!!!!

                INVALID = -1,
            }

            public Vector2 pos = new Vector2();
            public HintType type = HintType.INVALID;

            public Hint()
            {
                this.pos = new Vector2(float.MaxValue, float.MaxValue);
                this.type = HintType.INVALID;
            }
            public Hint(Vector2 pos, HintType type)
            {
                this.pos = pos;
                this.type = type;
            }
            public Hint(Vector2 pos, string typeName)
            {
                this.pos = pos;
                this.type = NameToType(typeName);
            }
            public static HintType NameToType(string typeName)
            {
                switch (typeName)
                {
                    case "Screwdriver":
                        return HintType.Screwdriver;
                    case "Crowbar":
                        return HintType.Crowbar;
                    case "TireIron":
                        return HintType.TireIron;
                    default:
                        return HintType.INVALID;
                }
            }
            public static string TypeToName(HintType type)
            {
                switch (type)
                {
                    case HintType.Screwdriver:
                        return "Screwdriver";
                    case HintType.Crowbar:
                        return "Crowbar";
                    case HintType.TireIron:
                        return "TireIron";
                    default:
                        return string.Empty;
                }
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
        public List<Hint> hints = new List<Hint>();

        public Vector3 origin;

        public override void Serialize(NetworkWriter writer)
        {
            Helpers.SerializeList(writer, ref hints);
            writer.Write(origin);
        }

        public override void Deserialize(NetworkReader reader)
        {
            Helpers.DeserializeList(reader, ref hints);
            origin = reader.ReadVector3();
        }
    }

    public class GameOverMsg : MessageBase
    {
        public double timestamp;
    }
}