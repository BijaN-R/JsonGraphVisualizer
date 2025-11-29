namespace JsonGraphVisualizer.Models
{
    public class JsonEdgeModel
    {
        public string Id { get; set; }
        public string FromNodeId { get; set; }
        public string ToNodeId { get; set; }

        public JsonEdgeModel()
        {
            Id = System.Guid.NewGuid().ToString();
        }
    } 
}
