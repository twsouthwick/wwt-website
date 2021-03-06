﻿using System.Configuration;
using System.IO;

namespace WWT.Providers
{
    public class CatalogProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string etag = context.Request.Headers["If-None-Match"];
            string filename = "";
            string webDir = Path.Combine(ConfigurationManager.AppSettings["DataDir"], "data");

            if (context.Request.Params["Q"] != null)
            {
                string query = context.Request.Params["Q"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");
                filename = Path.Combine(webDir, query + ".txt");

            }
            else if (context.Request.Params["X"] != null)
            {
                string query = context.Request.Params["X"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");
                filename = Path.Combine(webDir, query + ".xml");
            }
            else if (context.Request.Params["W"] != null)
            {
                //Response.Clear();
                //Response.ContentType = "application/x-wtml";

                string query = context.Request.Params["W"];

                query = query.Replace("..", "");
                query = query.Replace("\\", "");
                query = query.Replace("/", "");
                filename = Path.Combine(webDir, query + ".wtml");
            }

            if (!string.IsNullOrEmpty(filename))
            {
                FileInfo fi = new FileInfo(filename);
                fi.LastWriteTimeUtc.ToString();

                string newEtag = fi.LastWriteTimeUtc.ToString();

                if (newEtag != etag)
                {
                    context.Response.AddHeader("etag", fi.LastWriteTimeUtc.ToString());
                    context.Response.AddHeader("Cache-Control", "no-cache");
                    context.Response.WriteFile(filename);
                }
                else
                {
                    context.Response.Status = "304 Not Modified";
                }
            }

        }
    }
}
