using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WWTWebservices;

namespace WWT.Providers
{
    public class SDSSToast2Provider : RequestProvider
    {
        private readonly IPlateTilePyramid _plateTiles;
        private readonly FilePathOptions _options;
        private readonly IOctTileMapBuilder _octTileMap;

        public SDSSToast2Provider(IPlateTilePyramid plateTiles, FilePathOptions options, IOctTileMapBuilder octTileMap)
        {
            _plateTiles = plateTiles;
            _options = options;
            _octTileMap = octTileMap;
        }

        public override async Task RunAsync(IWwtContext context, CancellationToken token)
        {
            string query = context.Request.Params["Q"];
            string[] values = query.Split(',');
            int level = Convert.ToInt32(values[0]);
            int tileX = Convert.ToInt32(values[1]);
            int tileY = Convert.ToInt32(values[2]);

            if (level > 14)
            {
                context.Response.Write("No image");
                context.Response.Close();
                return;
            }

            if (level < 9)
            {
                context.Response.ContentType = "image/png";

                using (Stream s = await _plateTiles.GetStreamAsync(_options.WwtTilesDir, "SDSS_8.plate", level, tileX, tileY, token))
                {
                    await s.CopyToAsync(context.Response.OutputStream, token);
                    context.Response.Flush();
                    context.Response.End();
                    return;
                }
            }

            using (var stream = await _octTileMap.GetOctTileAsync(level, tileX, tileY, enforceBoundary: true, token: token))
            {
                if (stream is null)
                {
                    context.Response.Write("No Image");
                }
                else
                {
                    await stream.CopyToAsync(context.Response.OutputStream, token);
                }
            }
        }
    }
}