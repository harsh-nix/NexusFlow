using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Domain.Entities;

namespace NexusFlow.Domain.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        DateTime GetAccessTokenExpiry();
    }
}
