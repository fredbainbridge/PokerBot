using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace PokerBot.Repository.Mavens {
    public interface IMavenClient<T>
    {
        T Post(HttpClient client, Dictionary<string,string> Parameters);
    }

}
//A c# class to interface with Maeven Poker software 6.11
