using System;
using System.IO;
using WWTWebservices;

namespace WWT.Providers
{
    public class GetTileProvider : RequestProvider
    {
        public override void Run(IWwtContext context)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);
            string dataset = values[3];
            string id = dataset;

            string DSSTileCache = WWTUtil.GetCurrentConfigShare("DSSTileCache", true);

            string filename = String.Format(DSSTileCache + "\\imagesTiler\\{3}\\{0}\\{2}\\{2}_{1}.png", level, tileX, tileY, id);

            if (!File.Exists(filename))
            {
                context.Response.StatusCode = 404;
                return;
            }

            context.Response.WriteFile(filename);
        }
    }
}
