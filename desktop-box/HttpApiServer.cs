using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using desktop_box.entity;

namespace desktop_box
{
    public class HttpApiServer
    {
        public HttpListener listener;
        public string url = "http://localhost:8090/";
        public List<Func<HttpListenerRequest, String>> handles = new List<Func<HttpListenerRequest, String>>();
        public bool runServer = true;


        public async Task HandleIncomingConnections()
        {
            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();
                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                // Print out some info about the request
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();
                // Write the response info
                string resStr = null;
                foreach (var handle in handles)
                {
                    resStr = handle(req);
                    if (resStr != null)
                    {
                        break;
                    }
                }
                if (resStr == null)
                {
                    resStr = "{}";
                }
                byte[] data = Encoding.UTF8.GetBytes(resStr);
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }


        public void addHandle(Func<HttpListenerRequest, String> func)
        {
            handles.Add(func);
        }
        private byte[] ReadLineAsBytes(Stream SourceStream)
        {
            var resultStream = new MemoryStream();
            while (true)
            {
                int data = SourceStream.ReadByte();
                resultStream.WriteByte((byte)data);
                if (data == 10)
                    break;
            }
            resultStream.Position = 0;
            byte[] dataBytes = new byte[resultStream.Length];
            resultStream.Read(dataBytes, 0, dataBytes.Length);
            return dataBytes;
        }

        public void Run(Controller contrller)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);
            addHandle((req) =>
            {
                try
                {
                    StreamReader stream = new StreamReader(req.InputStream);
                    string body = stream.ReadToEnd();
                    if (!"".Equals(body))
                    {
                        Body reqBody = JsonConvert.DeserializeObject<Body>(body);
                        // 新的api
                        if (req.RawUrl.Equals("/api"))
                        {
                            contrller.setProp(reqBody);
                            if (!string.IsNullOrEmpty(reqBody.showPath))
                            {
                                contrller.ShowPath(reqBody.showPath, reqBody);
                            }
                            else if (reqBody.X != null && reqBody.Y != null && reqBody.X1 != null && reqBody.Y1 != null && reqBody.duration != null)
                            {
                                contrller.form.MoveWindows((int)reqBody.X, (int)reqBody.Y, (int)reqBody.X1, (int)reqBody.Y1, reqBody.duration);
                            }
                            else if (reqBody.X != null && reqBody.Y != null)
                            {
                                contrller.form.MoveWindows((int)reqBody.X, (int)reqBody.Y);
                            }
                            else
                            {
                                contrller.updateBitMap() ;
                            }
                            return JsonConvert.SerializeObject(reqBody);
                        }
                        // 废弃api
                        else if (req.RawUrl.Equals("/move"))
                        {
                            if (reqBody.X != null && reqBody.Y != null)
                            {
                                contrller.form.MoveWindows((int)reqBody.X, (int)reqBody.Y);
                            }
                        }
                        else if (req.RawUrl.Equals("/swap"))
                        {
                            contrller.form.MoveWindows((int)reqBody.X, (int)reqBody.Y, (int)reqBody.X1, (int)reqBody.Y1, reqBody.duration);
                        }
                        else if (req.RawUrl.Equals("/state"))
                        {
                            contrller.Transparency(reqBody.transparency??1);
                        }
                        else if (req.RawUrl.Equals("/ShowPath"))
                        {
                            contrller.ShowPath(reqBody.showPath, reqBody);
                        }
                    }
                    return JsonConvert.SerializeObject(new Body());
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            });
            // Handle requests
            Task listenTask = HandleIncomingConnections();
        }

    }
}
