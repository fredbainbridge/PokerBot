using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
namespace PokerBot.Services {
    public interface IMavenClient<T>
    {
        T Post(Dictionary<string,string> Parameters);
    }

}
//A c# class to interface with Maeven Poker software 6.11
