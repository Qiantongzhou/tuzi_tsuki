using System.ComponentModel.DataAnnotations;

namespace tuzi_tsuki.Models
{
    public class SongVM
    {
        [Display(Name = "歌名")]
        [Required(ErrorMessage = "需要歌名")]
        public string? SongName { get; set; }

        [Display(Name = "歌手")]
        [Required(ErrorMessage = "需要歌手")]
        public string? Actor { get; set; }
        [Display(Name = "专辑")]
        public string? alumn { get; set; }

        [Display(Name = "类型")]
        public string? type { get; set; }


        [Display(Name = "时间")]
        public string? Date { get; set; }

        public SongVM() { }
        public SongVM(string? songName, string? actor, string? alumn, string? type, string? date)
        {
            SongName = songName;
            Actor = actor;
            this.alumn = alumn;
            this.type = type;
            Date = date;
        }
    }

}
