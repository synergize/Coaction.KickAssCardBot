using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Coaction.KickAssCardBot
{
    public static class Program
    {
        private static void Main() => new Startup().Initialize().GetAwaiter().GetResult();
    }
}
