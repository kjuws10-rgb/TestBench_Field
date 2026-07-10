using System;
using System.Net;
using System.Net.Sockets;

namespace NetCommon
{
    public static class ClientConnector
    {
        /// <summary>
        /// Stage 서버로 연결한다. 필요하면 localBindIp로 소스 IP를 강제 바인딩한다.
        /// localBindIp가 null/공백이면 바인딩 없이 일반 TcpClient 사용.
        /// </summary>
        public static RpcSession Dial(string serverIp, int serverPort, string localBindIpOrNull, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(serverIp))
                throw new ArgumentException("serverIp is required");

            TcpClient tcp;
            if (!string.IsNullOrWhiteSpace(localBindIpOrNull))
            {
                var local = new IPEndPoint(IPAddress.Parse(localBindIpOrNull), 0);
                tcp = new TcpClient(local) { NoDelay = true };
            }
            else
            {
                tcp = new TcpClient() { NoDelay = true };
            }

            tcp.Connect(IPAddress.Parse(serverIp), serverPort);

            var sess = new RpcSession(tcp) { Logger = logger ?? new NullLogger() };
            sess.Start();
            return sess;
        }
    }
}