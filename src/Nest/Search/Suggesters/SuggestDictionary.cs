﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Elasticsearch.Net;

namespace Nest
{
	internal class SuggestDictionaryFormatter<T> : IJsonFormatter<ISuggestDictionary<T>>
		where T : class
	{
		public ISuggestDictionary<T> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
		{
			var formatter = formatterResolver.GetFormatter<Dictionary<string, ISuggest<T>[]>>();
			var dict = formatter.Deserialize(ref reader, formatterResolver);
			return new SuggestDictionary<T>(dict);
		}

		public void Serialize(ref JsonWriter writer, ISuggestDictionary<T> value, IJsonFormatterResolver formatterResolver)
		{
			var formatter = new VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, ISuggest<T>[]>();
			formatter.Serialize(ref writer, (SuggestDictionary<T>)value, formatterResolver);
		}
	}

	[JsonFormatter(typeof(SuggestDictionaryFormatter<>))]
	public interface ISuggestDictionary<out T>
		where T : class
	{
		ISuggest<T>[] this[string key] { get; }

		IEnumerable<string> Keys { get; }

		IEnumerable<ISuggest<T>[]> Values { get; }

		bool ContainsKey(string key);
	}


	public class SuggestDictionary<T> : IsAReadOnlyDictionaryBase<string, ISuggest<T>[]>, ISuggestDictionary<T>
		where T : class
	{
		[SerializationConstructor]
		public SuggestDictionary(IReadOnlyDictionary<string, ISuggest<T>[]> backingDictionary) : base(backingDictionary) { }

		public static SuggestDictionary<T> Default { get; } = new SuggestDictionary<T>(EmptyReadOnly<string, ISuggest<T>[]>.Dictionary);

		protected override string Sanitize(string key)
		{
			//typed_keys = true results in suggest keys being returned as "<type>#<name>"
			var hashIndex = key.IndexOf('#');
			return hashIndex > -1 ? key.Substring(hashIndex + 1) : key;
		}

	}
}
