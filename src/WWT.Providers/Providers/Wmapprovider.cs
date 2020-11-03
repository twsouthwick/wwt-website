using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class WmapProvider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;

        public WmapProvider(IPlateTilePyramid plateTiles, FilePathOptions options)
        {
            _plateTiles = plateTiles;
            _options = options;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level < 8)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "wmap.plate", level, tileX, tileY, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                }
            }

            return;;
        }
    }
}