namespace termoRefeicoes.Models
{
    public class LdapConfig
    {
        public string Server { get; set; }
        public string BindDn { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SearchBase { get; set; }
        public string SearchFilter { get; set; }
        public string MemberOf { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}