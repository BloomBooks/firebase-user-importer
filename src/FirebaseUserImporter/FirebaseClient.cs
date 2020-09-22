using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Auth.Hash;
using FirebaseUserImporter.Parse.Model;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseUserImporter
{
    /// <summary>
    /// Note: This class is here as a shell in case we want to programatically insert it using the SDK later,
    ///       but currently this isn't actually used because we're exporting a JSON and importing to Firebase using the CLI instead.
    /// </summary>
    class FirebaseClient
    {
        public AbstractFirebaseAuth Auth { get; set; }

        internal FirebaseClient(string serviceAccountKeyJsonPath)
        {
            try
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(serviceAccountKeyJsonPath),
                    ProjectId = "sil-bloomlibrary",
                });;

                Auth = FirebaseAuth.DefaultInstance;
            }
            catch (FirebaseException e)
            {
                Console.Error.WriteLine("Exception thrown: " + e.ToString());
                throw;
            }
        }

        public async Task ImportUsersToFirebaseAsync(IList<User> users)
        {
            if (users.Count > 1)
            {
                throw new ApplicationException("Not ready to run on real data yet!!!");
            }

            // TODO: Need to chunk it into batches of 100
            var userIdentifiers = users.Select(user => new EmailIdentifier(user.Email)).ToList();

            var existingFbUsers = await Auth.GetUsersAsync(userIdentifiers);
            
            Console.Out.WriteLine("Num matched users: " + existingFbUsers.Users.Count());
            foreach (var userRecord in existingFbUsers.Users ?? Enumerable.Empty<UserRecord>())
            {
                Console.Out.WriteLine("Removing user already in Firebase: " + userRecord.ToString());
                users.Remove(users.Where(x => x.Email == userRecord.Email).FirstOrDefault());
            }

            foreach (var user in users)
            {
                // ENHANCE: Maybe don't await in a loop
                await ImportUserAsync(user);
            }
        }
        

        private async Task ImportUserAsync(User user)
        {
            throw new ApplicationException("Not ready to run on real data yet!!!");

            try
            {
                var users = new List<ImportUserRecordArgs>()
                {
                    new ImportUserRecordArgs()
                    {
                        Uid = user.ObjectId,    // Ok (and recommended) to use Parse ID here
                        Email = user.Email,
                        EmailVerified = user.EmailVerified,
                        PasswordHash = Encoding.ASCII.GetBytes(user.HashedPassword),
                    },
                };

                var options = new UserImportOptions()
                {
                    Hash = new Bcrypt(),
                };

                UserImportResult result = await Auth.ImportUsersAsync(users, options);
                foreach (ErrorInfo indexedError in result.Errors)
                {
                    Console.WriteLine($"Failed to import user: {indexedError.Reason}");
                }
            }
            catch (FirebaseAuthException e)
            {
                Console.WriteLine($"Error importing users: {e.Message}");
            }
        }        
    }
}
