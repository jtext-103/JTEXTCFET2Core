using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Conventions;
using Nancy.Json;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{
    /// <summary>
    /// add static file for views
    /// </summary>
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/views"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/css","/views/css"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/js", "/views/js"));
            base.ConfigureConventions(nancyConventions);
        }

        public override void Configure(INancyEnvironment environment)
        {
            environment.Json(retainCasing: true);
            base.Configure(environment);
        }


        //protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        //{
        //    pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
        //    {
        //        ctx.Response.WithHeader("Access-Control-Allow-Origin", "a")
        //            .WithHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, PATCH, DELETE")
        //            .WithHeader("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Authorization , Access-Control-Request-Headers");
        //    });
        //}

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            try
            {
                string referrer = context.Request.Headers.Referrer;
                if(referrer != "")
                {
                    referrer = referrer.Substring(0, referrer.Length - 1);
                    //CORS Enable
                    pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
                    {
                        ctx.Response.WithHeader("Access-Control-Allow-Origin", referrer)
                            .WithHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, PATCH, DELETE")
                            //.WithHeader("Access-Control-Allow-Headers", "*");
                            .WithHeader("Access-Control-Allow-Headers", " X-Requested-With, Access-Control-Allow-Origin, Host, Connection, Pragma, Cache-Control, Accept, Access-Control-Request-Method, Access-Control-Request-Headers, Origin, User-Agent, Sec-Fetch-Mode, Referer, Accept-Encoding, Accept-Language");

                    });
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
