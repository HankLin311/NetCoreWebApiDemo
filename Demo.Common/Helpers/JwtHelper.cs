using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using Demo.Common.Enums;
using Demo.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Demo.Common.Helpers
{
    public class JwtHelper
    {
        readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 建立 JWT
        /// </summary>
        public string CreateJwt(List<Claim> claims)
        {
            string issuer = _configuration["JWT:Issuer"];

            // 建立一組對稱式加密的金鑰
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SignKey"]));

            // Token 資訊
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // 發行者
                Issuer = issuer,
                // 放入 Jwt Payload
                Subject = new ClaimsIdentity(claims),
                // 有效期限
                Expires = DateTime.Now.AddMinutes(5),
                // 用來產生數位簽章的密碼編譯演算法以及安全性演算法。
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // 產生所需要的 JWT securityToken 物件
            var securityToke = tokenHandler.CreateToken(tokenDescriptor);

            // 指定的安全性 Token 序列化為字串
            string serizlizeToken = tokenHandler.WriteToken(securityToke);

            if (string.IsNullOrEmpty(serizlizeToken))
            {
                throw new CustException("Jwt 建立時發生問題");
            }

            return serizlizeToken;
        }

        /// <summary>
        /// 驗證是否能夠登入
        /// </summary>
        public bool CheckAuth(HttpRequest httpRequest, out string jwt)
        {
            // 判斷是否登入，所以需要驗證時間
            var validationParameters = new TokenValidationParameters()
            {
                // 驗證 Issuer
                ValidateIssuer = true,
                // 取得 Issuer
                ValidIssuer = _configuration["Jwt:Issuer"],
                // 不驗證 Audience
                ValidateAudience = false,
                // 驗證 Jwt 的過期時間
                ValidateLifetime = true,
                // 如果 Jwt 的過期時間在此時間以前，則 Jwt 會視為無效
                ClockSkew = TimeSpan.Zero,
                // 使用者的簽章
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:SignKey"])),
            };

            return ValidateToken(httpRequest, validationParameters, out jwt, out _);
        }

        /// <summary>
        /// 嘗試取得 jwt payload
        /// </summary>
        public bool TryGetJwtPayload(HttpRequest httpRequest, out JwtPayloadDto jwtPayloadDto)
        {
            jwtPayloadDto = new JwtPayloadDto();

            // 不驗證 Jwt 時間，來取得資訊
            var validationParameters = new TokenValidationParameters()
            {
                // 驗證 Issuer
                ValidateIssuer = true,
                // 取得 Issuer
                ValidIssuer = _configuration["Jwt:Issuer"],
                // 不驗證 Audience
                ValidateAudience = false,
                // 使用者的簽章
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:SignKey"])),
            };

            // 是否驗證成功
            if (!ValidateToken(httpRequest, validationParameters, out string jwt, out string payload))
            {
                return false;
            }

            // 取得 payload 裡面的資訊
            var jd = JsonDocument.Parse(payload);
            var root = jd.RootElement;

            jwtPayloadDto.Jwt = jwt;

            if (root.TryGetProperty(JwtClaimType.Sub, out JsonElement subJe))
            {
                jwtPayloadDto.Sub = subJe.ToString();
            }

            if (root.TryGetProperty(JwtClaimType.Roles, out JsonElement rolesJe))
            {
                if (rolesJe.ValueKind == JsonValueKind.String)
                {
                    jwtPayloadDto.Roles = new List<DemoRole>()
                    {
                        Enum.Parse<DemoRole>(rolesJe.ToString())
                    };
                }
                else
                {
                    jwtPayloadDto.Roles = rolesJe
                        .EnumerateArray()
                        .Select(x => Enum.Parse<DemoRole>(x.ToString()))
                        .ToList();
                }
            }

            return true;
        }

        /// <summary>
        /// 驗證 Jwt 並回傳 payload
        /// </summary>
        private bool ValidateToken(
            HttpRequest httpRequest, TokenValidationParameters validationParameters, out string jwt, out string payload)
        {
            payload = string.Empty;
            jwt = string.Empty;

            // 取得 Authorization 的 Jwt
            bool hasAuthorizationHeader = httpRequest.Headers.TryGetValue("Authorization", out StringValues authHeaderValue);
            if (hasAuthorizationHeader)
            {
                string[] jwtInfo = authHeaderValue.ToString().Split(" ");

                if (jwtInfo.Length == 2)
                {
                    string type = jwtInfo[0].ToLower();

                    if ("bearer".Equals(type))
                    {
                        jwt = jwtInfo[1];
                    }
                }
            }

            // 是否有 Jwt
            if (!string.IsNullOrEmpty(jwt))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    tokenHandler.ValidateToken(jwt, validationParameters, out SecurityToken validatedToken);
                    payload = ((JwtSecurityToken)validatedToken).Payload.SerializeToJson();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Jwt Payload
    /// </summary>
    public class JwtPayloadDto
    {
        // EMAIL
        public string Sub { get; set; } = string.Empty;

        // 角色
        public IEnumerable<DemoRole> Roles { get; set; } = new List<DemoRole>();

        // Jwt
        public string Jwt { get; set; } = string.Empty;
    }

    /// <summary>
    /// Jwt Payload 特徵名稱
    /// </summary>
    public struct JwtClaimType
    {
        public const string Sub = JwtRegisteredClaimNames.Sub;
        public const string Jti = JwtRegisteredClaimNames.Jti;
        public const string Roles = "roles";
    }
}
