using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Middleware;
using Jtext103.CFET2.Core.Sample;
using Jtext103.CFET2.Core.Exception;
using Nancy;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Nancy.Conventions;
using MessagePack;
using Nancy.Configuration;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using System.Net;
using Jtext103.CFET2.Core.Log;

namespace Jtext103.CFET2.NancyHttpCommunicationModule
{

    public class MyBootstrapper : DefaultNancyBootstrapper
    {
        public override void Configure(INancyEnvironment environment)
        {
            
            environment.Json(retainCasing: true);
            
            base.Configure(environment);
        }
    }

    public class RequestModule : NancyModule
    {
        //private ViewSelector viewSelector = new ViewSelector();
        private ICfet2Logger logger = Cfet2LogManager.GetLogger("NancyModule");
        string viewPath = "/views/index.html";
        string windowsPath = "/views/WindowsAppIndex.html";

        public RequestModule()
        {
            Get("/", args =>
            {
                return GetResponse(AccessAction.get);
            });

            Get("/{name*}", args =>
            {
                return GetResponse(AccessAction.get);
            });

            Put("/{name*}", args =>
            {
                return GetResponse(AccessAction.invoke);
            });

            Post("/{name*}", args =>
            {
                return GetResponse(AccessAction.set);
            });

        }
        //public string GetLocalIp()
        //{
        //    ///获取本地的IP地址
        //    string AddressIP = string.Empty;
        //    foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        //    {
        //        if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
        //        {
        //            AddressIP = _IPAddress.ToString();
        //        }
        //    }
        //    return AddressIP;
        //}

        private object GetResponse(AccessAction action, bool shouldReturnView = false)
        {
            if (shouldReturnView)
            {
                return View["index"];
            }

            if (isFromBrowser())
            {
                string requestPath = this.Request.Url.Path;
                string queryString = this.Request.Url.Query.ToString();

                string hashTag;
                if (requestPath.Contains("WindowsAppIndex"))
                {
                    string subRequestPath = requestPath.Substring("/WindowsAppIndex.html".Length);
                    if(subRequestPath.Length == 0)
                    {
                        subRequestPath = "/";
                    }
                    hashTag = windowsPath + "#" + subRequestPath + queryString;
                }
                else
                {
                    hashTag = viewPath + "#" + requestPath + queryString;
                    logger.Info("return index.html");
                }
                return Response.AsRedirect(hashTag, Nancy.Responses.RedirectResponse.RedirectType.Permanent);
            }
            else
            {
                ResourceRequest request = new ResourceRequest(this.Request.Url.Path + this.Request.Url.Query.ToString(), action, null, null, null);
                ISample result;

                try
                {
                    result = NancyServer.TheHub.TryAccessResourceSampleWithUri(request);
                }
                catch (ResourceDoesNotExistException e)
                {
                    var response = new NotFoundResponse();
                    response.StatusCode = Nancy.HttpStatusCode.NotFound;
                    return response;
                }
                catch (Exception e)
                {
                    var response = new NotFoundResponse();
                    response.StatusCode = Nancy.HttpStatusCode.BadRequest;
                    return response;
                }

                result.Context["CFET2CORE_SAMPLE_ISREMOTE"] = true;
                if (this.Request.Headers.Referrer != "")
                {
                    //如果来自浏览器，就返回类似http://192.168.0.119:8001/relay/resource的格式，不是则返回相对路径
                    result.Context["CFET2CORE_SAMPLE_PATH"] = this.Request.Url.Scheme + "://" + this.Request.Url.HostName +":"+ this.Request.Url.Port+ result.Context["CFET2CORE_SAMPLE_PATH"];
                }

                if (Request.Headers.AcceptEncoding.Contains("MessagePack") || Request.Headers.Accept.Where(i=>i.Item1.Contains("application/MessagePack")).Count()>0)
                {
                    //var serializer = MessagePackSerializer.Get<Dictionary<string, object>>();
                    MemoryStream stream = new MemoryStream();

                    try
                    {
                        MessagePackSerializer.Serialize(stream, result.Context, MessagePack.Resolvers.ContractlessStandardResolver.Options);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.ToString());
                    }
                    //serializer.Pack(stream, result.Context);
                    SetData(stream);
                    var rightResponse = new Nancy.Responses.StreamResponse(GetData, "application/octet-stream");
                    rightResponse.Headers.Add(new KeyValuePair<string, string>("Access-Control-Allow-Origin", "*"));
                    rightResponse.Headers.Add(new KeyValuePair<string, string>("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, PATCH, DELETE"));
                    rightResponse.Headers.Add(new KeyValuePair<string, string>("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Authorization , Access-Control-Request-Headers"));
                    rightResponse.Headers.Add(new KeyValuePair<string, string>("Content-Encoding", "MessagePack"));
                    //rightResponse.Headers.Add(new KeyValuePair<string, string>("Content-Type", "application/MessagePack"));
                    rightResponse.Headers.Add(new KeyValuePair<string, string>("Content-Type", "application/x-www-from-urlencoded;charset=UTF-8"));

                    return rightResponse;
                }
                else
                {
                    try
                    {
                        //var rightResponse = JsonConvert.SerializeObject(result.Context, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        Response rightResponse = Response.AsText(JsonConvert.SerializeObject(result.Context));
                        rightResponse.ContentType = "application/json; charset=utf-8";
                        //Response test = Response.AsJson(result.Context);
                        //return Response.AsText(rightResponse);
                        return rightResponse;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.ToString());
                    }
                }
            }
        }

        //配合Nancy返回数据
        Stream middle;
        private void SetData(Stream stream)
        {
            middle = stream;
        }
        private Stream GetData()
        {
            //非常非常非常非常重要！
            middle.Position = 0;

            return middle;
        }

        private bool isFromBrowser()
        {
            var accept = this.Request.Headers.Accept;
            foreach (var i in accept)
            {
                if (i.Item1 == "text/html")
                    return true;
            }
            return false;
        }
    }
}
