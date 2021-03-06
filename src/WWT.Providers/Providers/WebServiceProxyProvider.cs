using System;
using System.Net;

namespace WWT.Providers
{
    public class WebServiceProxyProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string returnString = "Erorr: No URL Specified";
            string url = "";
            if (context.Request.Params["targeturl"] != null)
            {
                url = context.Request.Params["targeturl"];

                try
                {
                    if (url.ToLower().StartsWith("http") && !url.Contains("127.0.0.1") && !url.ToLower().Contains("localhost"))
                    {
                        Uri target = new Uri(url);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        using (WebClient wc = new WebClient())
                        {
                            byte[] data = wc.DownloadData(url);

                            context.Response.ContentType = wc.ResponseHeaders["Content-type"].ToString();
                            int length = data.Length;
                            context.Response.OutputStream.Write(data, 0, length);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    returnString = e.Message;
                    context.Response.Write(returnString);
                }
            }
        }
    }
}
