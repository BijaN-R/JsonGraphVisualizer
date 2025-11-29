using System.Collections.Generic;

namespace JsonGraphVisualizer.Models
{
    public class JsonNodeModel
    {
        public string Id { get; set; }  // GUID یکتا
        public string Title { get; set; }
        public NodeType Type { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public List<JsonNodeModel> Children { get; set; }
        public JsonNodeModel Parent { get; set; }
        public int ArrayCount { get; set; }  // برای array ها
        public object RawData { get; set; }  // داده خام برای نمایش در modal

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public JsonNodeModel()
        {
            Id = System.Guid.NewGuid().ToString();
            Properties = new Dictionary<string, object>();
            Children = new List<JsonNodeModel>();
        }
    } 
}
