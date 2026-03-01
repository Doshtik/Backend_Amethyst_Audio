namespace Backend_Amethyst_Audio.DTO.Pages;

/// <summary>
/// Данные для генерации страницы MyRecord
/// 
/// </summary>
public class PageMyRecordDto
{
    public List<string> AvailablePaces { get; set; }
    
    public List<string> AvailableMoods { get; set; }
}