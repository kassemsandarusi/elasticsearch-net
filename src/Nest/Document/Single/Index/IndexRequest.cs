﻿using System.IO;
using Elasticsearch.Net;

namespace Nest
{
	[JsonFormatter(typeof(IndexRequestFormatter<>))]
	public partial interface IIndexRequest<TDocument> : IProxyRequest where TDocument : class
	{
		TDocument Document { get; set; }
	}

	public partial class IndexRequest<TDocument>
		where TDocument : class
	{
		public TDocument Document { get; set; }

		protected override HttpMethod HttpMethod => GetHttpMethod(this);

		void IProxyRequest.WriteJson(IElasticsearchSerializer sourceSerializer, Stream stream, SerializationFormatting formatting) =>
			sourceSerializer.Serialize(Document, stream, formatting);

		internal static HttpMethod GetHttpMethod(IIndexRequest<TDocument> request) =>
			request.Id?.StringOrLongValue != null || (request.RouteValues.Resolved?.ContainsKey("id") ?? false) ? HttpMethod.PUT : HttpMethod.POST;

		partial void DocumentFromPath(TDocument document) => Document = document;
	}

	public partial class IndexDescriptor<TDocument> where TDocument : class
	{
		protected override HttpMethod HttpMethod => IndexRequest<TDocument>.GetHttpMethod(this);
		TDocument IIndexRequest<TDocument>.Document { get; set; }

		void IProxyRequest.WriteJson(IElasticsearchSerializer sourceSerializer, Stream stream, SerializationFormatting formatting) =>
			sourceSerializer.Serialize(Self.Document, stream, formatting);

		partial void DocumentFromPath(TDocument document) => Assign(document, (a, v) => a.Document = v);
	}
}
