using Common;
using Osprey.Parsing;
using Osprey.Routing;
using Sprache;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Osprey
{
    public class Server
    {

        // Incoming data from the client.  
        public static void Start(Binding binding, IEnumerable<CgiProgramOptions> programs)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(binding.Hostname);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, binding.Port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            var router = new Router();

            foreach (var program in programs)
            {
                router.AddRoute(program.Method, program.Route, (Req request) => CgiProgram.HandleRequestAsync(request, program.Command, program.Arguments));
            }

            // Bind the socket to the local endpoint and
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    string data = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        //if (bytesRec == 0 || data.IndexOf("<EOF>") > -1)
                        if (bytesRec != handler.ReceiveBufferSize)
                        {
                            break;
                        }
                    }

                    var response = HandleRequest(router, data);

                    handler.Send(response);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        private static byte[] HandleRequest(Router router, string data)
        {
            Req request;
            try
            {
                request = RequestGrammar.Request.Parse(data);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            var route = router.TryMatch(request.Method, request.Path.TrimStart('/').TrimEnd('/'));
            if (route == null)
            {
                return NotFoundResponse();
            }

            foreach (var routeValue in route.RouteValues)
            {
                request.Query[routeValue.Key] = routeValue.Value;
            }

            try
            {
                var response = route.Handler(request);
                return Response(Common.HttpVersion.Version1_1, 200, "OK", "text/plain", response);
            }
            catch (Exception ex)
            {
                return InternalError();
            }
        }

        private static byte[] BadRequest()
        {
            return Response(Common.HttpVersion.Version1_1, 400, "Not Found", "text/plain", string.Empty);
        }

        private static byte[] NotFoundResponse()
        {
            return Response(Common.HttpVersion.Version1_1, 404, "Not Found", "text/plain", string.Empty);
        }

        private static byte[] InternalError()
        {
            return Response(Common.HttpVersion.Version1_1, 500, "Internal Error", "text/plain", string.Empty);
        }

        public static byte[] Response(Common.HttpVersion version, int statusCode, string reasonPhrase, string contentType, string body)
        {
            var versionString = version == Common.HttpVersion.Version1_1 ? "1.1" : "2.0";
            var statusLine = $"HTTP/{versionString} {statusCode} {reasonPhrase}";
            var contentTypeLine = $"Content-Type: {contentType}";
            var response = $"{statusLine}\r\n{contentTypeLine}\r\n\r\n{body}";

            return Encoding.ASCII.GetBytes(response);
        }
    }
}