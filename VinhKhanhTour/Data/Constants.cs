using System;
using System.IO;

namespace VinhKhanhTour.Utilities
{
    public static class Constants
    {
        public const string DatabaseFilename = "VinhKhanhTour.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // Mở database ở chế độ đọc/ghi
            SQLite.SQLiteOpenFlags.ReadWrite |
            // Tự động tạo file nếu chưa có
            SQLite.SQLiteOpenFlags.Create |
            // Hỗ trợ truy cập đa luồng (Multi-threading)
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}