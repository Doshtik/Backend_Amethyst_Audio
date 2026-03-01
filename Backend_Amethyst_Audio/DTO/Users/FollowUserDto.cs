using System.ComponentModel.DataAnnotations;

namespace Backend_Amethyst_Audio.DTO;

public class FollowUserDto
{
    [Required] public long IdTargetUser { get; set; }
    // ВАЖНО: Если подписчик — это текущий залогиненный юзер, 
    // его ID лучше брать из JWT токена в контроллере, а не из DTO!
    [Required] public long IdSubscriber { get; set; }
}