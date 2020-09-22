using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FirebaseUserImporter.Parse.Model
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class User : ParseObject
	{
		[JsonProperty("_auth_data_bloom")]
		public AuthData AuthData { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("emailVerified")]
		public bool EmailVerified  { get; set; }

		[JsonProperty("_hashed_password")]
		public string HashedPassword  { get; set; }
		
		[JsonProperty("username")]
		public string UserName  { get; set; }

		// Enhance: Add more fields as needed.
	}
}
