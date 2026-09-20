using System.Collections.Concurrent;

/// <summary>
/// Represents a service for revoking JWT tokens.
/// This is a singleton service that maintains a list of revoked tokens in memory.
/// This uses memory and so is suitable only for development or a single-server application.
/// </summary>
public sealed class TokenRevocationService
{
    private readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();

    public void Revoke(string jti, DateTime expiresAt)
    {
        _revokedTokens[jti] = expiresAt;
    }

    public bool IsRevoked(string jti)
    {
        if (!_revokedTokens.TryGetValue(jti, out var expiresAt))
        {
            return false;
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            _revokedTokens.TryRemove(jti, out _);
            return false;
        }

        return true;
    }
}