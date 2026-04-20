using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System;
using System.Threading.Tasks;

namespace VinhKhanhTour.CMS // Hoặc VinhKhanhTour.CMS.Hubs tùy bạn đang để
{
    public class DeviceInfo
    {
        public string DeviceId { get; set; }
        public string IpAddress { get; set; }
        public string ConnectTime { get; set; }
    }

    public class MonitoringHub : Hub
    {
        public static readonly ConcurrentDictionary<string, DeviceInfo> OnlineDevices = new();

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var deviceId = httpContext?.Request.Query["deviceId"].ToString();

            // ĐIỂM SỬA: Chỉ đếm những kết nối có gửi kèm mã deviceId (từ Điện thoại). 
            // Nếu rỗng (từ trình duyệt Web Admin) thì bỏ qua không đếm.
            if (!string.IsNullOrEmpty(deviceId))
            {
                var ip = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                var newDevice = new DeviceInfo
                {
                    DeviceId = deviceId,
                    IpAddress = ip,
                    ConnectTime = DateTime.Now.ToString("HH:mm:ss")
                };

                OnlineDevices.TryAdd(Context.ConnectionId, newDevice);
                await Clients.All.SendAsync("DeviceConnected", newDevice, OnlineDevices.Count);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (OnlineDevices.TryRemove(Context.ConnectionId, out var removedDevice))
            {
                await Clients.All.SendAsync("DeviceDisconnected", removedDevice.DeviceId, OnlineDevices.Count);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}