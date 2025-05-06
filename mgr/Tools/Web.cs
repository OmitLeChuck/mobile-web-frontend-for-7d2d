using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace mgr.Tools
{
    public class Web
    {
        private readonly HttpListener _listener;
        private readonly Thread _serverThread;
        private bool _running;
        private int _port;

        // -- init the webserver
        public Web()
        {
            _listener = new HttpListener();
            
            // -- finding free port and setting the port for the code command later in the game
            _port = Get.MgrPort = FindAvailablePort(GamePrefs.GetInt(EnumGamePrefs.ServerPort) + 1);

            // -- begin listening
            _listener.Prefixes.Add($"http://*:{_port}/");
            _serverThread = new Thread(Listen)
            {
                IsBackground = true
            };
        }

        // -- trying to find a free port
        private int FindAvailablePort(int startPort)
        {
            int port = startPort;

            while (!IsPortAvailable(port))
            {
                port++;
                if (port > 65535)
                    throw new InvalidOperationException("no free port available");
            }

            return port;
        }

        // -- check which port is available
        private bool IsPortAvailable(int port)
        {
            if (GamePrefs.GetBool(EnumGamePrefs.WebDashboardEnabled) &&
                GamePrefs.GetInt(EnumGamePrefs.WebDashboardPort) == port)
                return false;

            if (GamePrefs.GetBool(EnumGamePrefs.TelnetEnabled) &&
                GamePrefs.GetInt(EnumGamePrefs.TelnetPort) == port)
                return false;

            return true;
        }


        // -- starting the webserver / api
        public void Start()
        {
            try
            {
                _running = true;
                _listener.Start();
                _serverThread.Start();
                Logger.Server($"mobile api is up and running on port {_port}");    
            } catch (Exception e)
            {
                Logger.Server($"starting mobile api failed: {e.Message}");
            }
            
        }

        // -- stopping the webserver / api
        public void Stop()
        {
            try
            {
                _running = false;
                _listener.Stop();
                Logger.Server("mobile api is shutting down");    
            } catch (Exception e)
            {
                Logger.Server($"stopping mobile api failed: {e.Message}");
            }
            
        }

        // -- listening for requests
        private void Listen()
        {
            while (_running)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                }
                catch (Exception ex)
                {
                    if (_running)
                        Logger.Server($"start listening failed: {ex.Message}");
                }
            }
        }

        // -- process any incoming requests
        private void ProcessRequest(object context)
        {
            WebRequestHandling webRequestHandling = new WebRequestHandling(context);
        }
    }
}