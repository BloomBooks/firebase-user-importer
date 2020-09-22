using Newtonsoft.Json;
using System;

namespace FirebaseUserImporter.Firebase.Model
{
    [JsonObject]
    class ImportedUserCommand
    {
        public ImportedUser[] users;
    }
}
