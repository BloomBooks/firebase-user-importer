using FirebaseUserImporter.Parse.Model;
using Newtonsoft.Json;
using System;

namespace FirebaseUserImporter.Firebase.Model
{
    [JsonObject]
    class ImportedUser
    {
        public string localId;

        // FYI, if a user already exists and is re-imported, and the displayName field is not present in the object during an import, it gets set to null or empty
        //      AKA it's always overwritten. You can't get it to only update the speicified fields... at least, not for free.
        public string displayName;
        public string email;
        public bool emailVerified;
        public string passwordHash;

        public ImportedUser()
        {
        }

        public ImportedUser(User parseUser)
            : this(parseUser.ObjectId, parseUser.UserName, parseUser.Email, parseUser.EmailVerified, parseUser.HashedPassword)
        {
        }

        public ImportedUser(string parseObjectId, string username, string email, bool emailVerified, string bcryptHash)
        {
            this.localId = parseObjectId;

            // Unfortunately, we don't have a good value to store for the display name. It should ideally be their first and last name.
            this.displayName = username;   
            this.email = email;
            this.emailVerified = emailVerified;
            
            this.passwordHash = Base64Encode(bcryptHash);
        }

        private static string Base64Encode(string plainText)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }
    }
}
