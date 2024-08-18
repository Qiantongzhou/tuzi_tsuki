namespace tuzi_tsuki.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }

        public string? Alumn { get; set; }
        public string? type { get; set; }

        public string? date { get; set; }

        public string copystring {  get; set; }

        public Song(int id, string name, string author, string? Alumn, string? type, string? date, string copystring)
        {
            Id = id;
            Name = name;
            Author = author;
            this.type = type;
            this.date = date;
            this.copystring = copystring;
            this.Alumn = Alumn;
        }
    }
    public class SongVM2
    {
        public string? Songname { get; set; }
        public string? Type { get; set; }

        public string? Alumn { get; set; }
        public string? author { get; set; }
        public string? date { get; set; }
    }
    public class SongsViewModel
    {
        public List<Song> Songs { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
    }

}
