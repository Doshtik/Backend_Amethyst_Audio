namespace Backend_Amethyst_Audio.DTO;

public class UserHistoryDto
{
    public UserInfoDto User { get; set; }
    
    public TrackInfoDto Track { get; set; }
    
    public int TotalListeningSec { get; set; }
}