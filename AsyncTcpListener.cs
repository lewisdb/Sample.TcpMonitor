using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Sample.TcpMonitor
{
    class AsyncTcpListener
    {
        #region members
        UInt32 _numHandshakes; //number of successful handshakes
        UInt32 _numActiveConnections; //number of active connections
        CancellationTokenSource _cancellationTokenSource;
        CancellationToken _cancellationToken;
        #endregion
        /// <summary>
        /// class constructor
        /// </summary>
        public AsyncTcpListener() { }

        /// <summary>
        /// call this method to start accepting tcp clients
        /// </summary>
        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            Task.Run(() => StartListening(), _cancellationToken);
        }

        /// <summary>
        /// call this method to stop accepting tcp clients
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// asynchronuous method to start accepting tcp clients
        /// </summary>
        async void StartListening()
        {
            TcpListener tcpListener = null;
            IPAddress ipAddress;
            try
            {
                #region //get default ip;
                IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                ipAddress = hostEntry.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
                #endregion
                //create and start a tcp listener
                tcpListener = new TcpListener(ipAddress, Defines.TCP_PORT);
                tcpListener.Start();

                // continue to listen to incoming connections until Stop() is called
                while (true)
                {
                    Task<TcpClient> t = tcpListener.AcceptTcpClientAsync();
                    t.Wait(_cancellationToken);
                    TcpClient client = await t;
                    ProcessClientCommands(client);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.WriteMessage("operation was cancelled");
            }
            catch (Exception ex)
            {
                Logger.WriteMessage(ex.Message);
            }
            finally
            {
                if (tcpListener != null)
                {
                    tcpListener.Stop();
                }
                _cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// when a client is connected, call this method to process client commands
        /// </summary>
        /// <param name="tcpClient">TcpClient</param>
        async void ProcessClientCommands(TcpClient tcpClient)
        {
            #region local members
            string clientMessage;
            int numOfBytesRead = 0;
            byte[] arrReceiveBuffer;
            byte[] arrSendBuffer;
            NetworkStream stream;
            #endregion

            try
            {
                _numActiveConnections++;
                stream = tcpClient.GetStream();

                //wait for the handshake operation to complete
                Task<bool> t = PerformHandshake(stream);
                await t;

                if (t.Result) //successful handshake 
                {
                    #region increase the number of handshakes by 1
                    //NOTE: theorically this value can reach UInt32.MaxValue
                    //in which case _numHandshakes++ yields the value 0
                    //the project requirements do not mention how to handle this scenario
                    //so let's assume that a wrap around value of zero is OK for now                               
                    #endregion
                    _numHandshakes++;

                    while (true)
                    {
                        if (_cancellationToken.IsCancellationRequested) _cancellationToken.ThrowIfCancellationRequested();

                        arrReceiveBuffer = new byte[Defines.READ_BUFFER_SIZE];
                        Task<int> readTask = stream.ReadAsync(arrReceiveBuffer, 0, arrReceiveBuffer.Length);
                        readTask.Wait(_cancellationToken);
                        numOfBytesRead = await readTask;

                        if (numOfBytesRead > 0)
                        {
                            #region process client messages                        
                            clientMessage = System.Text.Encoding.ASCII.GetString(arrReceiveBuffer, 0, numOfBytesRead);
                            arrSendBuffer = GetReplyBuffer(clientMessage);
                            if (arrSendBuffer != null)
                            {
                                await stream.WriteAsync(arrSendBuffer, 0, arrSendBuffer.Length);
                                await stream.FlushAsync();
                                if (clientMessage == Defines.CLIENT_MSG_TERMINATE)//client requests to close the connection
                                {
                                    break;
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteMessage(ex.Message);
            }
            finally
            {
                //close the tcp client and decrease the number of connections by 1
                if (tcpClient != null && tcpClient.Connected) tcpClient.Close();
                _numActiveConnections--;
            }
        }

        /// <summary>
        /// this method performs a handshake operation between the server and a connecting client
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>true if the handshake occurs within 5 seconds, otherwise false</returns>
        async Task<bool> PerformHandshake(NetworkStream stream)
        {
            #region local members
            string clientMessage = null;
            byte[] arrReadBuffer;
            byte[] arrSendBuffer;
            int numOfBytesRead = 0;
            DateTime handshakeStartTime = DateTime.Now;
            #endregion

            try
            {
                arrReadBuffer = new byte[Defines.READ_BUFFER_SIZE];
                Task task = Task.Run(async () =>
                {
                    numOfBytesRead = await stream.ReadAsync(arrReadBuffer, 0, arrReadBuffer.Length);
                });
                //a handshake command must occur within 5 seconds
                bool success = await Task.WhenAny(task, Task.Delay(Defines.HANDSHAKE_TIMEOUT)) == task;
                if (success) clientMessage = Encoding.ASCII.GetString(arrReadBuffer, 0, numOfBytesRead);

                if (clientMessage == Defines.CLIENT_MSG_HANDSHAKE_COMMAND)
                {
                    arrSendBuffer = ASCIIEncoding.ASCII.GetBytes(Defines.SERVER_MSG_HANDSHAKE_REPLY);
                    await stream.WriteAsync(arrSendBuffer, 0, arrSendBuffer.Length);
                    return (DateTime.Now - handshakeStartTime).TotalMilliseconds > Defines.HANDSHAKE_TIMEOUT ? false : true;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteMessage(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// get the response reply
        /// </summary>
        /// <param name="clientMessage">tcp client's command</param>
        /// <returns>returns the response in bytes, if the command is not known then return null</returns>
        byte[] GetReplyBuffer(string clientMessage)
        {
            switch (clientMessage)
            {
                case Defines.CLIENT_MSG_HANDSHAKE_COUNTS:
                    return ASCIIEncoding.ASCII.GetBytes(Convert.ToString(_numHandshakes));
                case Defines.CLIENT_MSG_ACTIVE_CONNECTIONS:
                    return ASCIIEncoding.ASCII.GetBytes(Convert.ToString(_numActiveConnections));
                case Defines.CLIENT_MSG_PRIME:
                    return ASCIIEncoding.ASCII.GetBytes(Convert.ToString(Defines.PRIME_NUMBER));
                case Defines.CLIENT_MSG_TERMINATE:
                    return ASCIIEncoding.ASCII.GetBytes(Defines.SERVER_MSG_TERMINATE_REPLY);
                default:
                    return null;
            }
        }
    }
}
