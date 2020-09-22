using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FirebaseUserImporter.Parse.Model
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AuthData
    {
        [JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("token")]
		public string Token { get; set; }

        internal bool IsInitialized()
        {
            return !String.IsNullOrWhiteSpace(Id) && !String.IsNullOrWhiteSpace(Token);
        }
    }
}
