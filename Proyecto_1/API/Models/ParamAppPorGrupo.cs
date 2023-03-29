namespace API.Models
{
    public class ParamAppPorGrupo
    {
        public Guid id { get; set; }
        public String userName { get; set; }
        public int valPomodoro { get; set; }
        public int valDescanso { get; set; }
        public int valDescansoLargo { get; set; }
        public string codGrupo { get; set; }
    }
}