using System.IdentityModel.Tokens.Jwt;

namespace Api.Token
{
    public class TokenJwt
    {
        private JwtSecurityToken _token;
        public DateTime ValidTo => _token.ValidTo;
        public string value => new JwtSecurityTokenHandler().WriteToken(this._token);

        public TokenJwt(JwtSecurityToken token)
        {
            _token = token;
        }
    }
}
