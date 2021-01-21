namespace Domain.Commands
{
    public class UpdateWashCommand
    {
        public string UserId { get; set; }
        public bool StartNow { get; set; }
        public bool Abort { get; set; }
    }
}