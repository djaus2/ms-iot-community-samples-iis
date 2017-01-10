using Nancy;
using Nancy.ModelBinding;
using Octokit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ms_iot_community_samples_svc
{
    public partial class IndexModule : NancyModule
    {
        /// <summary>
        /// Basic authenticate any GitHub developer 
        /// </summary>
        /// <param name="githubUsr">GitHub username</param>
        /// <param name="githubPwd">GitHub user password</param>
        /// <param name="githubRepo">GitHub repository name</param>
        /// <returns>true if successful, false otherwise.</returns>
        private async Task<User> AnyGitHubLogin(string githubUsr, string githubPwd, string githubRepo)
        {
            //throw new NotImplementedException();
            string githuUrl = (string)ConfigurationManager.AppSettings["GitHub.Url"];
            GitHubClient lbasicGitHubClient =
                new GitHubClient(new ProductHeaderValue(githubRepo), new Uri(githuUrl));

            //https://github.com/octokit/octokit.net

            var basicAuth = new Credentials(githubUsr, githubPwd); 
            lbasicGitHubClient.Credentials = basicAuth;
            User user = null;
            try
            {
                user = await lbasicGitHubClient.User.Get(githubUsr);
            }
            catch (Exception ex)
            {
                return null;
            }
            return user;
        }

        private async Task<GitHubClient> GetGitHubClient(string githubUsr, string githubPwd, string githubRepo)
        {
            //throw new NotImplementedException();
            string githuUrl = (string)ConfigurationManager.AppSettings["GitHub.Url"];
            GitHubClient lbasicGitHubClient =
                new GitHubClient(new ProductHeaderValue(githubRepo), new Uri(githuUrl));

            //https://github.com/octokit/octokit.net

            var basicAuth = new Credentials(githubUsr, githubPwd);
            lbasicGitHubClient.Credentials = basicAuth;
            User user = null;
            try
            {
                user = await lbasicGitHubClient.User.Get(githubUsr);
            }
            catch (Exception ex)
            {
                return null;
            }
            if (user != null)
                return lbasicGitHubClient;
            else
                return null;
        }
    }
}
