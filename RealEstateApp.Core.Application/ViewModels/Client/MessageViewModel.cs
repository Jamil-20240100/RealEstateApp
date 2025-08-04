namespace RealEstateApp.Core.Application.ViewModels.Client
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public int PropertyId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsFromClient { get; set; }
    }

    public class CreateMessageViewModel
    {
        public string ReceiverId { get; set; }
        public int PropertyId { get; set; }
        public string Content { get; set; }
    }
}
