using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Token
{
    public class TokenJwtBuilder
    {
        private SecurityKey? _securityKey = null;
        private string _subject = "";
        private string _issuer = "";
        private string _audience = "";
        private Dictionary<string, string> _claims = new Dictionary<string, string>();
        private int _expiryInMinutes = 10;

        public TokenJwtBuilder AddSecurityKey(SecurityKey securityKey)
        {
            _securityKey = securityKey;
            return this;
        }

        public TokenJwtBuilder AddSubject(string subject)
        {
            _subject = subject;
            return this;
        }

        public TokenJwtBuilder AddIssuer(string issuer)
        {
            _issuer = issuer;
            return this;
        }

        public TokenJwtBuilder AddAudience(string audience)
        {
            _audience = audience;
            return this;
        }

        public TokenJwtBuilder AddClaims(string type, string value)
        {
            _claims.Add(type, value);
            return this;
        }

        public TokenJwtBuilder AddClaims(Dictionary<string, string> claims)
        {
            _claims.Union(claims);
            return this;
        }

        public TokenJwtBuilder AddExpiry(int expiryInMinutes)
        {
            _expiryInMinutes = expiryInMinutes;
            return this;
        }

        public TokenJwt Builder()
        {
            EnsureArguments();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }.Union(_claims.Select(item => new Claim(item.Key, item.Value)));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryInMinutes),
                signingCredentials: new SigningCredentials(
                    _securityKey,
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new TokenJwt(token);
        }

        private void EnsureArguments()
        {
            if (_securityKey == null)
            {
                throw new ArgumentException("Security Key");
            }

            if (string.IsNullOrEmpty(_subject) == null)
            {
                throw new ArgumentException("Subject");
            }

            if (string.IsNullOrEmpty(_issuer) == null)
            {
                throw new ArgumentException("Issuer");
            }

            if (string.IsNullOrEmpty(_audience) == null)
            {
                throw new ArgumentException("Audience");
            }
        }
    }
}
