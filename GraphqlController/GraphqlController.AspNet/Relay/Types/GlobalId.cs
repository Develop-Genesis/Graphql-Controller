using GraphqlController.AspNetCore.Extensions;

namespace GraphqlController.AspNetCore.Relay.Types
{
    public class GlobalId
    {
        public string Id { get; }

        public string ObjectType { get; }

        public GlobalId(string globalId)
        {
            var value = GlobalIdTools.GetEntityTypeAndId(globalId);
            Id = value.Id;
            ObjectType = value.Name;
        }

        public GlobalId(string id, string objectType)
        {
            Id = id;
            ObjectType = objectType;
        }

        public string Deserialize()
        {
            return GlobalIdTools.GetGlobalId(ObjectType, Id);
        }

        public override bool Equals(object obj)
        {
            var value2 = obj as GlobalId;
            return Id == value2.Id && ObjectType == value2.ObjectType;
        }

        public override int GetHashCode()
        {
            return (Id + ObjectType).GetHashCode();
        }

        public override string ToString()
        {
            return Deserialize();
        }

    }


    internal static class GlobalIdTools
    {
        public static (string Id, string Name) GetEntityTypeAndId(string globalId)
        {
            var uniqueId = globalId.Base64Decode();
            var split = uniqueId.Split('|');
            var typeName = split[0];
            var id = split[1];

            return (id, typeName);
        }

        public static string GetGlobalId(string objectType, string id)
        {
            var typeName = objectType;
            var uniqueId = $"{typeName}|{id}";
            return uniqueId.Base64Encode();
        }
    }

}
