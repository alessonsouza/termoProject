namespace termoRefeicoes.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public object Data { get; set; }
        public string lastMonth { get; set; }
    }
}