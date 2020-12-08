using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace ZoomRecording.App_Code
{
	public static class ZoomToken
	{

		public static string GetZoomToken(string apikey,string apisecret)
		{
			// Token will be good for 20 minutes
			DateTime Expiry = DateTime.UtcNow.AddMinutes(60);


			int ts = (int)(Expiry - new DateTime(1970, 1, 1)).TotalSeconds;

			// Create Security key  using private key above:
			var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(apisecret));

			// length should be >256b
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			//Finally create a Token
			var header = new JwtHeader(credentials);

			//Zoom Required Payload
			var payload = new JwtPayload
		{
			{ "iss", apikey},
			{ "exp", ts },
		};

			var secToken = new JwtSecurityToken(header, payload);
			var handler = new JwtSecurityTokenHandler();

			// Token to String so you can use it in your client
			var tokenString = handler.WriteToken(secToken);

			return tokenString;
		}
	}
}
