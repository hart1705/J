using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PORTAL.DAL.EF.Helper;
using PORTAL.WEB.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PORTAL.WEB.Extensions
{
    public class JsonOrgChartConverter : JsonConverter
    {
        private readonly bool _isLoggedInUser;
        public JsonOrgChartConverter(bool isLoggedInUser)
        {
            _isLoggedInUser = isLoggedInUser;
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(BinaryTreeModel));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            BinaryTreeModel node = (BinaryTreeModel)value;
            JObject obj = new JObject();
            List<JObject> childrens = new List<JObject>();

            if (node == null)
            {
                return;
            }
            obj.Add("userid", node.userid);
            obj.Add("name", node.name);
            obj.Add("dateregistered", node.dateregistered);
            obj.Add("referrals", node.referrals);
            obj.Add("isdummy", "0");
            obj.Add("position", node.position);

            if (!string.IsNullOrWhiteSpace(node.title))
            {
                obj.Add("title", node.title);
            }

            if (node.left != null)
            {
                childrens.Add(JObject.FromObject(node.left, serializer));
            }
            else if (_isLoggedInUser)
            {
                childrens.Add(JObject.FromObject(new Dummy(node.userid, Enums.BPosition.Left), serializer));
            }

            if (node.right != null)
            {
                childrens.Add(JObject.FromObject(node.right, serializer));
            }
            else if (_isLoggedInUser)
            {
                childrens.Add(JObject.FromObject(new Dummy(node.userid, Enums.BPosition.Right), serializer));
            }

            if (childrens.Any())
            {
                obj.Add("children", JArray.FromObject(childrens));
            }
            obj.WriteTo(writer);
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
