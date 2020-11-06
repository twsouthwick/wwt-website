using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WWT.Providers
{
    [RequestEndpoint("/wwtweb/ExcelAddinUpdate.aspx")]
    public class ExcelAddinUpdateProvider : RequestProvider
    {
        public override string ContentType => ContentTypes.Text;

        public override Task RunAsync(IWwtContext context, CancellationToken token)
        {
            context.Response.AddHeader("Cache-Control", "no-cache");
            context.Response.Write("ClientVersion:");
            context.Response.WriteFile(context.MapPath(Path.Combine("..", "wwt2", "ExcelAddinVersion.txt")));
            context.Response.Write("\nUpdateUrl:");
            context.Response.WriteFile(context.MapPath(Path.Combine("..", "wwt2", "ExcelAddinUpdateUrl.txt")));

            return Task.CompletedTask;
        }
    }
}
