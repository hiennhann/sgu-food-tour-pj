using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace VinhKhanhTour.Services
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }

    public class ModalErrorHandler : IErrorHandler
    {
        // Biến Singleton để gọi ở bất cứ đâu
        public static ModalErrorHandler Instance { get; } = new ModalErrorHandler();

        private SemaphoreSlim _semaphore = new(1, 1);

        private ModalErrorHandler() { }

        public void HandleError(Exception ex)
        {
            _ = DisplayAlert(ex);
        }

        private async Task DisplayAlert(Exception ex)
        {
            try
            {
                await _semaphore.WaitAsync();

                // Hiển thị thông báo đa ngôn ngữ nếu muốn, ở đây tạm dùng tiếng Anh/Việt cơ bản
                string errorTitle = LocalizationResourceManager.Instance["Lỗi"] ?? "Error";
                string okBtn = LocalizationResourceManager.Instance["Đóng"] ?? "OK";

                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(errorTitle, ex.Message, okBtn);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}