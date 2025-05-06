using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace mgr.Tools
{
    public class WebApiEndpointHandling
    {
        private readonly string _endpoint;
        private readonly JObject _data;
        private readonly ClientInfo _cInfo;
        private readonly string _playerId;
        private readonly EntityPlayer _player;
        private object _response;

        public WebApiEndpointHandling(JObject data, ClientInfo cInfo)
        {
            _endpoint = (string)data["type"];
            _data = data;
            _cInfo = cInfo;
            _playerId = (string)data["id"];
            _player = Get.Player(_cInfo.entityId);
            _response = null;
            Logger.Server($"handling api request for {_endpoint}");
        }

        // -- handling all the endpoints
        public object HandleEndpoints()
        {
            try
            {
                Logger.Server($"data: {JsonConvert.SerializeObject(_data)}");
                switch (_endpoint)
                {
                    case "teleport":
                        try
                        {
                            if (_player != null)
                            {
                                _response = new
                                {
                                    error = Teleport.Player(new Vector3((float)_data["x"], -1, (float)_data["z"]),
                                        _cInfo)
                                };
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        catch (Exception)
                        {
                            _response = new { message = "something went wrong with the teleport", error = true };
                        }

                        break;
                    case "position":
                        _response = new
                            { _player.position.x, _player.position.y, _player.position.z, error = false };
                        break;
                    case "online":
                        bool online = Get.GetClientInfoFromWhatWeHaveGot(_playerId) != null;
                        _response = new { online, error = online };
                        break;
                    case "bed":
                        try
                        {
                            if (_player != null)
                            {
                                Vector3 bedPosition = _player.spawnPoints[0].ToVector3();
                                bedPosition.y = -1;
                                _response = new { error = Teleport.Player(bedPosition, _cInfo) };
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        catch (Exception)
                        {
                            _response = new { message = "something went wrong with the teleport", error = true };
                        }

                        break;
                }

                if (_response != null)
                {
                    Logger.Server($"response: {JsonConvert.SerializeObject(_response)}");
                    return _response;
                }

                throw new Exception();
            }
            catch (Exception)
            {
                Logger.Server("handling api request failed");
            }

            return null;
        }
    }
}