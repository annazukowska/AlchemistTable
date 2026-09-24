namespace AlchemistTable.Core.Entities
{
    public class RefreshToken : EntityBase
    {
        public string Token { get; set; } 
        public DateTime ExpiresAt { get; set; } 
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public Guid AlchemistId { get; set; }

        public RefreshToken() { }
        public RefreshToken(Guid alchemistId, string token)
        {
            Id = Guid.NewGuid();
            Token = token;
            ExpiresAt = DateTime.UtcNow.AddDays(7); // Token valid for 7 days
            AlchemistId = alchemistId;
        }
    }
}
