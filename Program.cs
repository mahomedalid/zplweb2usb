using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NHttp;
using System.Net;

namespace ZplWeb2Usb
{
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            using (var server = new HttpServer())
            {
                // New requests are signaled through the RequestReceived
                // event.

                server.RequestReceived += (s, e) =>
                {
                    // The response must be written to e.Response.OutputStream.
                    // When writing text, a StreamWriter can be used.

                    using (var writer = new StreamWriter(e.Response.OutputStream))
                    {
                        writer.WriteLine("Hello world!");
                        
                        String validApi = "AAAABBBCCC";

                        if (e.Request.QueryString.Get("apiKey") != validApi) {
                            writer.WriteLine("INVALID API KEY: " + e.Request.QueryString.Get("apiKey"));
                            return;
                        }
                        String data = "A50,50,0,2,1,1,N,\"9129302\"";
                        switch (e.Request.Path.TrimEnd('/')) {
                            case "/print":
                                var bytes = Encoding.ASCII.GetBytes(data);
                                RawPrinterHelper.SendStringToPrinter(e.Request.QueryString.Get("printer"), bytes, bytes.Length);
                                break;
                            default:
                                writer.WriteLine("INVALID OPERATION "+e.Request.RawUrl+" "+e.Request.Url+" "+e.Request.Path);
                                break;
                        }
                    }
                };

                // Start the server on a random port. Use server.EndPoint
                // to specify a specific port, e.g.:
                //
                //     server.EndPoint = new IPEndPoint(IPAddress.Loopback, 80);
                //
                server.EndPoint = new IPEndPoint(IPAddress.Loopback, 65101);
                server.Start();

                // Start the default web browser.
                

                Process.Start(String.Format("http://{0}/", server.EndPoint));

                Console.WriteLine("Presione cualquier tecla para continuar 1...");
                Console.ReadKey();

                // When the HttpServer is disposed, all opened connections
                // are automatically closed.
            }
        }
    }

}