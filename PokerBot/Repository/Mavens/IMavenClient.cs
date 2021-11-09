using System.Collections.Generic;
using System.Net.Http;

namespace PokerBot.Repository.Mavens {
    public interface IMavenClient<T>
    {
        T Post(HttpClient client, Dictionary<string,string> Parameters);
    }

}
//A c# class to interface with Maeven Poker software 6.11
