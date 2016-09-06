using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Api.Swagger;

namespace ServiceStack.SP
{
    public class ServiceStackAppHost : AppHostBase
    {
        public ServiceStackAppHost() //Tell ServiceStack the name and where to find your web services
            : base("SharePoint ServiceStack", typeof(ServiceStackAppHost).Assembly) { }

        public override void Configure(Funq.Container container)
        {
            Plugins.Add(new SwaggerFeature() { UseBootstrapTheme = false, UseCamelCaseModelPropertyNames = true });
        
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;

          
            SetConfig(new HostConfig
            {
                HandlerFactoryPath = "_layouts/api"
            });

        }



        public static void Start()
        {
            new ServiceStackAppHost().Init();
        }
    }
}
