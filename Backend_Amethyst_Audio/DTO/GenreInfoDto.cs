using Backend_Amethyst_Audio.Models.Entities;

namespace Backend_Amethyst_Audio.DTO;

public class GenreInfoDto
{
     public short Id { get; set; }

     public string GenreName { get; set; } = null!;
}