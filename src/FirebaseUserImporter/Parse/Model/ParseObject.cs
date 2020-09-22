using Newtonsoft.Json;

namespace FirebaseUserImporter.Parse.Model
{
	// Contains common fields to every object in a Parse class
	[JsonObject]
	public abstract class ParseObject
	{
		[JsonProperty("_id")]
		public string ObjectId { get; set;  }
		//Date createdAt
		//Date updatedAt
		//ACL  ACL

		public override bool Equals(object other)
		{
			return (other is ParseObject) && this.ObjectId == ((ParseObject)other).ObjectId;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ObjectId?.GetHashCode() ?? 0;
			}
		}
	}
}
