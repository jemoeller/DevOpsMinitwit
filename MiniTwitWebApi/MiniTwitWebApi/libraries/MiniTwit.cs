using System;
using System.Net.Http;

namespace MiniTwitWebApi.libraries
{
    public class MiniTwit
    {
        private readonly HttpClient _client;
        private string DATABASE = "/tmp/minitwit.db";
        private int PER_PAGE = 30;
        private Boolean DEBUG = true;
        private string SECRET_KEY = "development key";

        public MiniTwit()
        {

            var client = new HttpClient();

        }



    }
}
