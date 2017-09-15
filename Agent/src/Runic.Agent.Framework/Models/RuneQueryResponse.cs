namespace Runic.Agent.Framework.Models
{
    public class RuneQueryResponse
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public RuneQuery RuneQueryResult { get; set; }
    }
}