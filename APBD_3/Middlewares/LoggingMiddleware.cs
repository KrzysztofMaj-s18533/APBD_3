using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APBD_3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            string method = httpContext.Request.Method;
            string path = httpContext.Request.Path;
            string body = "";

            using (StreamReader sr = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                body += await sr.ReadToEndAsync();
            }

            string queryString = httpContext.Request.QueryString.ToString();

            using (StreamWriter sw = File.AppendText("requestsLog.txt"))
            {
                sw.WriteLine("=====REQUEST=====");
                sw.WriteLine("Method: " + method);
                sw.WriteLine("Path: " + path);
                sw.WriteLine("Body:\n" + body);
                sw.WriteLine("QueryString: " + queryString);
                sw.WriteLine("=====END OF REQUEST=====\n");
            }

            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            await _next(httpContext);
        }
    }
}
