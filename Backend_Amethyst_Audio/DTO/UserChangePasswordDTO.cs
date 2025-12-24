using System.ComponentModel.DataAnnotations;

namespace Backend_Amethyst_Audio.DTO;

public class UserChangePasswordDTO
{
    [Required]
    public string OldPassword { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль должен быть не менее 8 символов.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", 
        ErrorMessage = "Пароль должен содержать цифры, заглавные и строчные буквы.")]
    public string NewPassword { get; set; } = null!;

    [Compare("NewPassword", ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmNewPassword { get; set; } = null!;
}