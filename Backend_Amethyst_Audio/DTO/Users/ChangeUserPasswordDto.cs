using System.ComponentModel.DataAnnotations;

namespace Backend_Amethyst_Audio.DTO;

public class ChangeUserPasswordDto
{
    public string OldPassword { get; set; } = null!;

    public string NewPassword { get; set; } = null!; // Проверка на совпадение новых паролей пусть будет на клиенте
}