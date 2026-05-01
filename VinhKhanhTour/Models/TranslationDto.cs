namespace VinhKhanhTour.Models
{
    public class TranslationDto
    {
        public int PoiId { get; set; }
        public string TranslatedName { get; set; }
        public string TranslatedAddress { get; set; }
        public string TranslatedTtsScript { get; set; }
    }
}