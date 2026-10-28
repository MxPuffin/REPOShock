using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.Serialization.Utilities;
using System.Linq;
using REPOShock.src.PiShockLibrary.Services;
using REPOShock.src.PiShockLibrary.Models;
using REPOShock.src.PiShockLibrary.Enums;

namespace REPOShock.src
{
    internal class PiShockController
    {
        private PiShockAPI _piShockAPI;
        private PiShockUserInfo _userInfo;
        private AsyncPiShockRequestQueue _queue = new();
        public PiShockController(PiShockAPI piShockAPI, PiShockUserInfo piShockUserInfo)
        {
            _piShockAPI = piShockAPI;
            _userInfo = piShockUserInfo;
        }

        public static int ClampShock(int intensity) =>
            Math.Clamp(intensity, 1, ConfigHandler.MaxIntensity.Value);

        public void OperatePiShock(int intensity, int duration, PiShockOperation op)
        {
            Task.Run(async () =>
            {
                var tasks = new List<Task<bool>>();

                _userInfo.Devices.ForEach(device =>
                {
                    var body = new PiShockOperateRequestBody(op, intensity, duration, "R.E.P.O Shock");
                    tasks.Add(_piShockAPI.SendOperationRequest(body, device.ShockerId));
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
