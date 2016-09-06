using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.SharePoint;


namespace ServiceStack.SP
{
    public class HelloService : Service
    {
        public object Any(Hello input)
        {
            return "Hello: " + input.Name + ".  You are in the SharePoint site: " + SPContext.Current.Web.Url;
        }
    }

    [Api("Service to say hello.")]
    [ApiResponse(HttpStatusCode.BadRequest, "You submitted a bad request")]
    [ApiResponse(HttpStatusCode.InternalServerError, "There was an error")]
    [Route("/Hello")]
    public class Hello : IReturn<string>
    {
        public string Name { get; set; }
    }
}
