namespace Domain.Commands
{
    public class CreateWashCommand
    {
        public string UserId { get; set; }
        public int Type { get; set; }
        public bool StartNow { get; set; }
    }
}