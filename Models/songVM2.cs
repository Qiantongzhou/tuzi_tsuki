using System.ComponentModel.DataAnnotations;

namespace tuzi_tsuki.Models
{
    public class songVM2
    {
        [Display(Name = "txt歌单")]
        [Required(ErrorMessage = "需要歌单-歌名，歌手，专辑")]
        public string? Songtext { get; set; }
    }
}
