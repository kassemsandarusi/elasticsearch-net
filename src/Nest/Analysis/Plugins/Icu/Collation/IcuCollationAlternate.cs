using System.Runtime.Serialization;
using Elasticsearch.Net;

namespace Nest
{
	/// <summary>
	/// Sets the alternate handling for strength quaternary to be either shifted or non-ignorable.
	/// Which boils down to ignoring punctuation and whitespace.
	/// </summary>
	[StringEnum]
	public enum IcuCollationAlternate
	{
		[EnumMember(Value = "shifted")] Shifted,
		[EnumMember(Value = "non-ignorable")] NonIgnorable
	}
}
