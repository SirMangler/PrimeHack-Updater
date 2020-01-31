using System;
using System.Net;


namespace InternalUpdater
{
    class TimedWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest w = base.GetWebRequest(address);
            w.Timeout = 5000;
            return w;
        }
    }
}
