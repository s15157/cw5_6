using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw5_6.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                string path = context.Request.Path;
                string method = context.Request.Method;
                string queryString = context.Request.QueryString.ToString();
                string bodyStr = "";

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                // Set a variable to the Documents path.
                string docPath = Environment.CurrentDirectory;
                // Create a string array with the additional lines of text
                string[] lines = { method,path,bodyStr,queryString };
                // Append new lines of text to the file
                File.AppendAllLines(Path.Combine(docPath, "requestsLog.txt"), lines);
            }

            if(_next!=null) await _next(context);
        }
    }
}
