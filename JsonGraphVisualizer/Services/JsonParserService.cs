// Services/JsonParserService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using JsonGraphVisualizer.Models;
using Newtonsoft.Json.Linq;

namespace JsonGraphVisualizer.Services
{
    public class JsonParserService
    {
        private int _nodeCounter = 0;

        // 🆕 متد جدید برای Code-Behind
        public List<JsonNodeModel> ParseJson(string jsonString)
        {
            _nodeCounter = 0;

            if (string.IsNullOrWhiteSpace(jsonString))
                return new List<JsonNodeModel>();

            try
            {
                var token = JToken.Parse(jsonString);
                var rootNode = Parse(token);

                // برگرداندن به صورت لیست
                return new List<JsonNodeModel> { rootNode };
            }
            catch (Exception)
            {
                throw; // خطا رو به بالاتر پاس می‌دیم
            }
        }

        // متد قبلی که Dependency Property ازش استفاده می‌کنه
        public JsonNodeModel Parse(JToken token)
        {
            _nodeCounter++;

            if (token == null)
            {
                return CreateNode("null", NodeType.Primitive, null);
            }

            switch (token.Type)
            {
                case JTokenType.Object:
                    return ParseObject((JObject)token);

                case JTokenType.Array:
                    return ParseArray((JArray)token);

                case JTokenType.String:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Boolean:
                case JTokenType.Null:
                    return CreateNode(
                        $"Value {_nodeCounter}",
                        NodeType.Primitive,
                        token.ToObject<object>()
                    );

                default:
                    return CreateNode("Unknown", NodeType.Primitive, null);
            }
        }

        private JsonNodeModel ParseObject(JObject obj)
        {
            var node = CreateNode($"Object {_nodeCounter}", NodeType.Object, obj);

            foreach (var prop in obj.Properties())
            {
                var value = prop.Value;

                // اگر value خودش Object یا Array پیچیده باشه، child node می‌سازیم
                if (value.Type == JTokenType.Object ||
                    (value.Type == JTokenType.Array && IsComplexArray((JArray)value)))
                {
                    var childNode = Parse(value);
                    childNode.Title = prop.Name; // نام property به عنوان عنوان child
                    node.Children.Add(childNode);
                }
                else
                {
                    // مقادیر ساده رو در Properties نگه می‌داریم
                    node.Properties[prop.Name] = ExtractValue(value);
                }
            }

            return node;
        }

        private JsonNodeModel ParseArray(JArray array)
        {
            var node = CreateNode($"Array [{array.Count}]", NodeType.Array, array);

            // اگر آرایه حاوی Object هست، برای هر Object یک child node می‌سازیم
            if (IsComplexArray(array))
            {
                for (int i = 0; i < array.Count; i++)
                {
                    var childNode = Parse(array[i]);
                    childNode.Title = $"[{i}]";
                    node.Children.Add(childNode);
                }
            }
            else
            {
                // آرایه ساده: آیتم‌ها رو در Properties نمایش می‌دیم
                for (int i = 0; i < array.Count; i++)
                {
                    node.Properties[$"[{i}]"] = ExtractValue(array[i]);
                }
            }

            return node;
        }

        private bool IsComplexArray(JArray array)
        {
            // اگر حداقل یکی از آیتم‌ها Object یا Array باشه، پیچیده است
            return array.Any(item =>
                item.Type == JTokenType.Object ||
                item.Type == JTokenType.Array
            );
        }

        private object ExtractValue(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
                return null;

            switch (token.Type)
            {
                case JTokenType.String:
                    return token.ToObject<string>();
                case JTokenType.Integer:
                    return token.ToObject<long>();
                case JTokenType.Float:
                    return token.ToObject<double>();
                case JTokenType.Boolean:
                    return token.ToObject<bool>();
                default:
                    return token.ToString();
            }
        }

        private JsonNodeModel CreateNode(string title, NodeType type, object rawData)
        {
            return new JsonNodeModel
            {
                Title = title,
                Type = type,
                RawData = rawData
            };
        }
    }
}
