using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mgr.Tools
{
    public class WebRequestHandling
    {
        private readonly HttpListenerContext _context;

        public WebRequestHandling(object context)
        {
            _context = (HttpListenerContext)context;
            _context.Response.StatusCode = 200;
            _context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            _context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");

            try
            {
                // -- handling api requests
                if (_context.Request.Url.LocalPath.StartsWith("/api") && _context.Request.HttpMethod == "POST")
                {
                    HandleApiRequest();
                    return;
                }

                // -- handling static files
                HandleStaticFiles();
            }
            catch (Exception)
            {
                _context.Response.StatusCode = 500;
            }
            finally
            {
                _context.Response.OutputStream.Close();
            }
        }

        // -- handling api request and route them in the right way (code endpoint must be handled exceptionally)
        private void HandleApiRequest()
        {
            string requestBody = new StreamReader(_context.Request.InputStream).ReadToEnd();
            JObject data = JObject.Parse(requestBody);

            if ((string)data["type"] == "code")
            {
                string code = data["code"]?.ToString();
                if (code != null)
                {
                    string codeFile = Path.Combine(Get.Wd(), $"{code}.code");
                    if (File.Exists(codeFile))
                    {
                        string userid = File.ReadAllText(codeFile).Trim();
                        File.Delete(codeFile);
                        SendResponse(new { id = userid, error = false });
                    }
                    else SendError("code-file not found");
                }
                else SendError("code is empty");
            }
            else
            {
                if (string.IsNullOrEmpty((string)data["id"])) SendError("id empty");
                else
                {
                    string id = (string)data["id"];
                    object response = null;
                    ClientInfo cInfo = Get.GetClientInfoFromWhatWeHaveGot(id);
                    if (cInfo != null)
                    {
                        response = new WebApiEndpointHandling(data,cInfo).HandleEndpoints();
                        if (response == null)
                        {
                            SendError("something went wrong");
                            return;
                        }
                    }
                    else
                    {
                        SendError("player not found");
                        return;
                    }

                    SendResponse(response);
                }
            }
        }

        // -- handling static files
        private void HandleStaticFiles()
        {
            string requestedFile = _context.Request.Url.LocalPath.TrimStart('/').Trim();
            if (string.IsNullOrEmpty(requestedFile)) requestedFile = "index.html";

            string filePath = Path.Combine(Get.WebRoot(), requestedFile);

            if (!File.Exists(filePath))
            {
                _context.Response.StatusCode = 404;
                return;
            }

            string contentType = GetContentType(Path.GetExtension(filePath));
            byte[] fileBytes = File.ReadAllBytes(filePath);

            _context.Response.ContentType = contentType;
            _context.Response.ContentLength64 = fileBytes.Length;
            _context.Response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
        }

        // -- determine content type for static files
        private string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".html":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".js":
                    return "application/javascript";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                default:
                    return "application/octet-stream";
            }
        }

        // -- sending a response for an api-request
        private void SendResponse(object response)
        {
            _context.Response.ContentType = "application/json";
            string responseString = JsonConvert.SerializeObject(response);
            Logger.Server($"response: {responseString}");
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            _context.Response.ContentLength64 = buffer.Length;
            using (var output = _context.Response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
                output.Flush();
            }
        }

        private void SendError(string message)
        {
            SendResponse(new { message, error = true });
        }
    }
}