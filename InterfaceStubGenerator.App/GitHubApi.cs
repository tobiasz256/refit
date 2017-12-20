using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Refit; // InterfaceStubGenerator looks for this

using static System.Math; // This is here to verify https://github.com/paulcbetts/refit/issues/283

namespace Refit.Tests
{
	
		
  public class TestNested4
    {
        
		
        public string Login { get; set; }
        public int Id { get; set; }
        public string AvatarUrl { get; set; }
        public string GravatarId { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public string FollowersUrl { get; set; }
        public string FollowingUrl { get; set; }
        public string GistsUrl { get; set; }
        public string StarredUrl { get; set; }
        public string SubscriptionsUrl { get; set; }
        public string OrganizationsUrl { get; set; }
        public string ReposUrl { get; set; }
        public string EventsUrl { get; set; }
        public string ReceivedEventsUrl { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
		[Headers("User-Agent: Refit Integration Tests")]
        public interface INestedGitHubApi6
        {
            [Get("/users/{username}")]
            Task GetUser(string userName);

            [Get("/")]
            Task<HttpResponseMessage> GetIndex();

            [Get("/")]
            IObservable<string> GetIndexObservable();

            [Get("/give-me-some-404-action")]
            Task NothingToSeeHere();
        }
        public string Company { get; set; }
        public string Blog { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public bool? Hireable { get; set; }
        public string Bio { get; set; }
        public int PublicRepos { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public int PublicGists { get; set; }
		
		
    }

}
