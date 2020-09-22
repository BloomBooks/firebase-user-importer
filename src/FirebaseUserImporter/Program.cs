using System;
using System.Threading.Tasks;

namespace FirebaseUserImporter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length <= 3)
            {
                throw new ApplicationException("Not enough arguments. " + GetUsageString());
            }

            string serviceAccountKeyJsonFile = args[0];
            string inputProdParseUsersJsonFile = args[1];
            string inputDevParseUsersJsonFile = args[2];
            string outputPathToUserImportJsonFile = args[3];

            await new Importer(serviceAccountKeyJsonFile).RunImport(inputProdParseUsersJsonFile, inputDevParseUsersJsonFile, outputPathToUserImportJsonFile);
        } 

        private static string GetUsageString()
        {
            return "Usage: FirebaseUserImporter.exe {inputProdParseUsersJsonFile} {inputDevParseUsersJsonFile} {outputPathToUserImportJsonFile} {serviceAccountKeyJsonFile}";
        }
    }
}
