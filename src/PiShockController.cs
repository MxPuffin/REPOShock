using PiShockClassLibrary.Models;
using PiShockClassLibrary.Services;
using PiShockClassLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using PiShockLibrary.Enums;
using System.Threading.Tasks;
using PiShockLibrary.Models;
using Sirenix.Serialization.Utilities;
using System.Linq;

namespace REPOShock.src
{
    internal class PiShockController
    {
        private PiShockAPI _piShockAPI;
        private PiShockUserInfo _userInfo;
        public PiShockController(PiShockAPI piShockAPI, PiShockUserInfo piShockUserInfo)
        {
            _piShockAPI = piShockAPI;
            _userInfo = piShockUserInfo;
        }

        public static int ClampShock(int intensity)
        {
            return Math.Clamp(intensity, 1, ConfigHandler.ConfigMaxIntensity.Value);
        }

        public void OperatePiShock(int intensity, int duration, PiShockOperations op)
        {
            Task.Run(async () =>
            {
                var tasks = new List<Task<bool>>();

                _userInfo.Devices.ForEach(device =>
                {
                    var body = new PiShockOperateRequestBody(_userInfo, op, device.ShareCode, intensity, duration, "R.E.P.O Shock");
                    tasks.Add(_piShockAPI.SendOperationRequest(body));
                });

                var results = await Task.WhenAll(tasks);
                if (results.Any(success => !success))
                {
                    REPOShock.Logger.LogError("One or more PiShock requests failed to execute!");
                }
            });
        }
    }
}
