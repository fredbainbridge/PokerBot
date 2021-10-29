using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using PokerBot.Repository;
using System.Net.Http;
using PokerBot.Models;
using ImageMagick;
using System.IO;
using PokerBot.Repository.Mavens;

namespace PokerBot.Services
{
    internal interface ISlackUserAvatar
    {
        Task GetGraphics(CancellationToken stoppingToken);
    }
    internal class SlackUserAvatar : ISlackUserAvatar
    {
        static readonly HttpClient client = new HttpClient();
        private ISecrets _secrets;
        private IPokerRepository _pokerRepo;
        private PokerDBContext _pokerDB;
        private ISlackClient _slackClient;
        private IMavenAccountsEdit _mavenAccountsEdit;
        
        public SlackUserAvatar(ISecrets secrets, IPokerRepository pokerRepository, PokerDBContext pokerDBContext, ISlackClient slackClient, IMavenAccountsEdit mavenAccountsEdit)
        {
            _secrets = secrets;
            _pokerRepo = pokerRepository;
            _pokerDB = pokerDBContext;
            _slackClient = slackClient;
            _mavenAccountsEdit = mavenAccountsEdit;
        }
        public async Task GetGraphics(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if(!Directory.Exists(_secrets.AvatarDir()))
                {
                    Directory.CreateDirectory(_secrets.AvatarDir());
                }
                var users = _pokerDB.User.OrderBy(u => u.ID).ToList();
                var size = new MagickGeometry(32, 32);
                size.IgnoreAspectRatio = true;
                //did something change?
                int counter = 0;
                foreach (var user in users)
                {
                    counter++;
                    string uid = user.SlackID;
                    string uri = "https://slack.com/api/users.profile.get?token=" + _secrets.UserToken() + "&user=" + uid;
                    try
                    {
                        string responseBody = await client.GetStringAsync(uri);
                        SlackUser u = SlackUser.FromJson(responseBody);
                        if(u.Profile == null) {
                            continue;
                        }
                        if(!u.Profile.AvatarHash.Equals(user.AvatarHash) || !File.Exists(_secrets.AvatarDir() + user.SlackID + ".png"))
                        {
                            MagickImage i = new MagickImage(await client.GetStreamAsync(u.Profile.Image48));
                            i.Resize(size);
                            if(File.Exists(_secrets.AvatarDir() + user.SlackID + ".png"))
                            {
                                File.Delete(_secrets.AvatarDir() + user.SlackID + ".png");
                            }
                            Console.WriteLine(_secrets.AvatarDir() + user.SlackID + ".png");
                            i.Write(_secrets.AvatarDir() + user.SlackID + ".png");
                            user.AvatarHash = u.Profile.AvatarHash;
                        }
                        if(!u.Profile.DisplayName.Equals(user.SlackUserName))
                        {
                            user.SlackUserName = u.Profile.DisplayName;
                        }
                        user.AvatarIndex = counter;
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }
                    _mavenAccountsEdit.SetAvatarPath(user.UserName, _secrets.AvatarDir() + user.SlackID + ".png");
                    await Task.Delay(2000, cancellationToken);
                }
                    
                _pokerDB.SaveChanges();                
            }
            await Task.Delay(900000, cancellationToken); //very long time
        }
    }
}
