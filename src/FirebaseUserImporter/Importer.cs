using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using FirebaseUserImporter.Firebase.Model;
using FirebaseUserImporter.Parse;
using FirebaseUserImporter.Parse.Model;
using Newtonsoft.Json;

namespace FirebaseUserImporter
{
    class Importer
    {
        private IParseClient _parseClient;
        private FirebaseClient _firebaseClient;

        internal Importer(string serviceAccountKeyJsonPath)
        {
            _parseClient = new ParseExporterReader();
            _firebaseClient = new FirebaseClient(serviceAccountKeyJsonPath);
        }

        public async Task RunImport(string inputParseProdUsersJsonFile, string inputParseDevUsersJsonFile, string outputFilePath)
        {
            var users = _parseClient.GetUsers(inputParseProdUsersJsonFile, inputParseDevUsersJsonFile);
            var usersToImport = await GetNonFirebaseUsersAsync(users);

            GenerateFirebaseUserImportFile(usersToImport, outputFilePath);
        }

        private async Task<List<User>> GetNonFirebaseUsersAsync(IEnumerable<User> parseUsers)
        {
            var firebaseUsers = await GetUsersAlreadyInFirebaseAsync(parseUsers);            
            var firebaseEmails = new HashSet<string>(firebaseUsers.Select(x => x.Email));
            
            var parseOnlyUsers = new List<User>();
            foreach(var user in parseUsers)
            {
                if (!firebaseEmails.Contains(user.Email))
                {
                    parseOnlyUsers.Add(user);
                }
            }

            return parseOnlyUsers;
        }

        private async Task<List<UserRecord>> GetUsersAlreadyInFirebaseAsync(IEnumerable<User> users)
        {
            var alreadyPresentUserRecords = new List<UserRecord>();

            var userIdentifiers = GetUserEmailIdentifiers(users.Where(user => !String.IsNullOrWhiteSpace(user.Email))).ToList();
            int startIndex = 0;
            int numUsersPerTry = 100;
            while (startIndex < userIdentifiers.Count)
            {
                var userIdsForThisTry = userIdentifiers.Skip(startIndex).Take(numUsersPerTry);
                var getUsersReuslt = await _firebaseClient.Auth.GetUsersAsync(userIdsForThisTry.ToList());
                if (getUsersReuslt?.Users?.Count() >= 1)
                {
                    alreadyPresentUserRecords.AddRange(getUsersReuslt.Users);
                }               

                startIndex += numUsersPerTry;
            }

            return alreadyPresentUserRecords;
        }

        private async Task ListAllFirebaseUsers(IList<User> users)
        {
            var existingUsers = await GetUsersAlreadyInFirebaseAsync(users);
            
            foreach (var userRecord in existingUsers)
            {
                Console.Out.WriteLine($"Found user: email={userRecord.Email}, displayName={userRecord.DisplayName}");
            }
        }

        internal static IEnumerable<EmailIdentifier> GetUserEmailIdentifiers(IEnumerable<User> users)
        {
            return users.Select(user => new EmailIdentifier(user.Email));
        }

        private static void GenerateFirebaseUserImportFile(IList<User> parseUsers, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                var json = GenerateFirebaseUserImportJson(parseUsers);
                writer.WriteLine(json);
            }

            Console.Out.Write($"Import for {parseUsers.Count} users written to ${filePath}");
        }

        private static string GenerateFirebaseUserImportJson(IEnumerable<User> parseUsers)
        {
            var usersToImport = parseUsers.Select(x => new ImportedUser(x)).ToArray();
            var importCommand = new ImportedUserCommand();
            importCommand.users = usersToImport;

            return JsonConvert.SerializeObject(importCommand);
        }
    }
}
