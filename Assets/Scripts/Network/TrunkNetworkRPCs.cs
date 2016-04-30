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
        public const short Base = MsgType.Highest;

        public const short VoiceChatPacket      = Base + 1;
        public const short InitSession          = Base + 2;
        public const short LoadSession          = Base + 3;
        public const short ValidateSession      = Base + 4;
        public const short PlayerStatusChange   = Base + 5;

        public const short APB                  = Base + 11;
        public const short TriggerPoliceCar     = Base + 12;
        public const short TriggerHelicopter    = Base + 13;

        public const short GameLost             = Base + 20;
        public const short GameWon              = Base + 21;

        public const short DEBUG_ChangeCullingRadius     = Base + 30;
    }

    public class DEBUGIntMsg : MessageBase 
    {
        public int value;
    }

    public class SeedMsg : MessageBase
    {
        public int seed = -1;
    }

    public class APBRequest : MessageBase
    {
        public Vector3 position;
    }
    public class APBResponse : MessageBase
    {
        public class Hint : MessageBase
        {
            public enum HintType
            {
                INVALID = -1,

                Wrench = 0,
                Screwdriver,
                Crowbar,
                TireIron,
                Blanket,
                Rope,
                Hostage,        // <- if you get this, you've won!!!!
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
            public Hint(Vector3 pos, HintType type)
            {
                this.pos = new Vector2(pos.x, pos.z);
                this.type = type;
            }
            public Hint(Vector2 pos, string typeName)
            {
                this.pos = pos;
                this.type = NameToType(typeName);
            }
            public Hint(Vector3 pos, string typeName)
            {
                this.pos = new Vector2(pos.x, pos.z);
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
                type = (HintType)reader.ReadInt16();
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

    public class TriggerHelicopterMsg : MessageBase
    {
        public float yPos;
        public bool goingRight;
    }

    public class TriggerPoliceMsg : MessageBase
    {
        public Vector2 position;
    }

    public class GameOverMsg : MessageBase
    {
        public double timestamp;
    }

    public class VoiceChatMsg : MessageBase
    {
        public VoiceChatMsg() { }
        public VoiceChatMsg(VoiceChat.VoiceChatPacket packet)
        {
            payload = packet;
        }

        public VoiceChat.VoiceChatPacket payload;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write((byte)payload.Compression);
            writer.WriteBytesAndSize(payload.Data, payload.Length);
            writer.Write(payload.NetworkId);
            writer.Write(payload.PacketId);
        }

        public override void Deserialize(NetworkReader reader)
        {
            payload.Compression = (VoiceChat.VoiceChatCompression)reader.ReadByte();
            payload.Data = reader.ReadBytesAndSize();
            payload.Length = payload.Data.Length;
            payload.NetworkId = reader.ReadInt32();
            payload.PacketId = reader.ReadUInt64();
        }
    }

    public class UpdateStatusMsg : MessageBase
    {
        public int status;
    }
}