using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;

        // Navigation
        public User? User { get; set; }
    }
}