using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.DTO.Pages;

/// <summary>
/// Данные для генерации страницы MyRecord
/// 
/// </summary>
public class ResonanceConfigDto
{
    public List<Pace> AvailablePaces { get; set; }
    
    public List<Mood> AvailableMoods { get; set; }
}