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
        public int CurrentPoiId { get; set; } // Số 0 nghĩa là đang đi rông ngoài đường, lớn hơn 0 là đang vào radar quán
    }

    public class MonitoringHub : Hub
    {
        public static readonly ConcurrentDictionary<string, DeviceInfo> OnlineDevices = new();

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var deviceId = httpContext?.Request.Query["deviceId"].ToString();

            if (!string.IsNullOrEmpty(deviceId))
            {
                // 1. NẾU LÀ ĐIỆN THOẠI MỞ APP (Có DeviceId)
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
            else
            {
                // 2. NẾU LÀ ADMIN (TRÌNH DUYỆT) VỪA MỞ HOẶC F5 LẠI TRANG DASHBOARD
                // Lấy toàn bộ danh sách đang có sẵn trong RAM gửi riêng cho Admin này
                var currentDevices = OnlineDevices.Values.ToList();
                await Clients.Caller.SendAsync("LoadInitialDevices", currentDevices, currentDevices.Count);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (OnlineDevices.TryRemove(Context.ConnectionId, out var removedDevice))
            {
                await Clients.All.SendAsync("DeviceDisconnected", removedDevice.DeviceId, OnlineDevices.Count);
                // Giảm mật độ trên trạm map
                await BroadcastHeatmap();
            }
            await base.OnDisconnectedAsync(exception);
        }

        // --- CHỨC NĂNG BẢN ĐỒ NHIỆT (ĐÁM ĐÔNG) ---
        public async Task UpdateUserLocation(int poiId)
        {
            if (OnlineDevices.TryGetValue(Context.ConnectionId, out var device))
            {
                if (device.CurrentPoiId != poiId)
                {
                    device.CurrentPoiId = poiId;
                    await BroadcastHeatmap();
                }
            }
        }

        private async Task BroadcastHeatmap()
        {
            // Nhóm và đếm những Device đang khai báo đứng ở 1 quán (PoiId > 0)
            var heatMapCounts = System.Linq.Enumerable.ToDictionary(
                System.Linq.Enumerable.GroupBy(
                    System.Linq.Enumerable.Where(OnlineDevices.Values, d => d.CurrentPoiId > 0),
                    d => d.CurrentPoiId
                ),
                g => g.Key,
                g => g.Count()
            );

            // Truyền mảng Map bằng cú pháp C# Json tự động của SignalR
            await Clients.All.SendAsync("UpdateHeatmap", System.Text.Json.JsonSerializer.Serialize(heatMapCounts));
        }
    }
}