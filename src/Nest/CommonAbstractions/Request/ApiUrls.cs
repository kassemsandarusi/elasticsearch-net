using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
	/// <summary>
	/// Each Request type holds a static instance of this class which creates cached builders for each
	/// of the defined urls in the json spec.
	/// </summary>
	internal class ApiUrls
	{
		/// <summary>
		/// If the spec only defines a single non parameterizable route this allows us to shortcircuit and avoid hitting
		/// the cached string builders.
		/// </summary>
		private readonly string _fixedUrl;

		/// <summary>
		/// Creates a lookup for number of parts <=> list of routes with that number of parts.
		/// <see cref="UrlLookup.Matches"/> allows us to quickly find the right url to use in the list.
		/// </summary>
		public Dictionary<int, List<UrlLookup>> Routes { get; }

		private readonly string _errorMessageSuffix;

		/// <summary> Only intended to be created once per request and stored in a static </summary>
		internal ApiUrls(string[] routes)
		{
			if (routes == null || routes.Length == 0) throw new ArgumentException(nameof(routes), "urls is null or empty");
			if (routes.Length == 1 && !routes[0].Contains("{")) _fixedUrl = routes[0];
			else
			{
				foreach (var route in routes)
				{
					var bracketsCount = route.Count(c => c.Equals('{'));
					if (Routes == null) Routes = new Dictionary<int, List<UrlLookup>>();
					if (Routes.ContainsKey(bracketsCount))
						Routes[bracketsCount].Add(new UrlLookup(route));
					else
						Routes.Add(bracketsCount, new List<UrlLookup> { new UrlLookup(route) });
				}
			}

			_errorMessageSuffix = string.Join(",", routes);

			// received multiple urls without brackets we resolve to first
			if (Routes == null) _fixedUrl = routes[0];
		}

		public string Resolve(RouteValues routeValues, IConnectionSettingsValues settings)
		{
			if (_fixedUrl != null) return _fixedUrl;

			var resolved = routeValues.Resolve(settings);

			if (!Routes.TryGetValue(resolved.Count, out var routes))
				throw new Exception($"No route taking {resolved.Count} parameters{_errorMessageSuffix}");

			if (routes.Count == 1)
				return routes[0].ToUrl(resolved);

			//find the first url that has all provided parameters
			foreach (var u in routes)
			{
				if (u.Matches(resolved))
					return u.ToUrl(resolved);
			}
			throw new Exception($"No route taking {routeValues.Count} parameters{_errorMessageSuffix}");
		}
	}
}
