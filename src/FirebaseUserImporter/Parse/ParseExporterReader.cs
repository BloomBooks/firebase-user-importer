using FirebaseUserImporter.Parse.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FirebaseUserImporter.Parse
{
    class ParseExporterReader: IParseClient
    {
        public IList<User> GetUsers(string inputParseProdUsersJsonFile, string inputParseDevUsersJsonFile)
        {
            var prodUsers = GetUsersFromFile(inputParseProdUsersJsonFile);
            var devUsers = GetUsersFromFile(inputParseDevUsersJsonFile);

            var combinedUsersMap = new Dictionary<string, User>();
            AddUsersToDictionaryIfNotExist(prodUsers, combinedUsersMap);
            AddUsersToDictionaryIfNotExist(devUsers, combinedUsersMap);

            var combinedUsers = combinedUsersMap.Values.ToList();
            return combinedUsers;
        }

        private IList<User> GetUsersFromFile(string filename)
        {
            string fileContents = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<User[]>(fileContents);            
        }

        private void AddUsersToDictionaryIfNotExist(IList<User> usersToAdd, Dictionary<string, User> dictionary)
        {
            foreach (var user in usersToAdd)
            {
                if (!String.IsNullOrEmpty(user.Email) && !dictionary.ContainsKey(user.Email))
                {
                    dictionary.Add(user.Email, user);
                }
            }
        }
    }
}
