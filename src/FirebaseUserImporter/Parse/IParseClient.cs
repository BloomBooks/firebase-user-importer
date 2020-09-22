using System;
using System.Collections.Generic;
using FirebaseUserImporter.Parse.Model;

namespace FirebaseUserImporter.Parse
{
	interface IParseClient
	{
		IList<User> GetUsers(string inputParseProdUsersJsonFile, string inputParseDevUsersJsonFile);
	}
}
