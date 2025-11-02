using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Flippit.Api.App.EndToEndTests
{
    public class FlippitApiApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(collection =>
            {
                var controllerAssemblyName = typeof(Program).Assembly.FullName;
                collection.AddMvc().AddApplicationPart(Assembly.Load(controllerAssemblyName!));
            });
            return base.CreateHost(builder);
        }
    }
}
