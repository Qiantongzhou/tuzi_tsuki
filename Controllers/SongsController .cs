using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using tuzi_tsuki.firebase;
using tuzi_tsuki.Models;
using tuzi_tsuki.Utilities;
namespace tuzi_tsuki.Controllers
{


    public class SongsController : Controller
    {
        FirebaseHelper firebaseHelper;
        IEnumerable<Song> songs;
        int itemCount=10;
        public SongsController()
        {
            this.firebaseHelper = new FirebaseHelper("https://tuzitsuki-default-rtdb.firebaseio.com");
        }

        public async Task<string> GetSong()
        {


            return await firebaseHelper.GetData<string>("abc_123", "Songdb");
        }

        private async Task<List<Song>> GetAllSongs()
        {
            string songsDynamic =await GetSong();
            //Console.WriteLine(songsDynamic);

            var songsDictionary = JsonConvert.DeserializeObject<Dictionary<string, SongVM2>>(songsDynamic);


            int k = 0;
            List<Song> temp = new List<Song>();
            if (songsDictionary != null)
            {

                foreach (var s in songsDictionary)
                {

                    temp.Add(new Song(k, DecodeFromHex( s.Value.Songname), DecodeFromHex(s.Value.author), DecodeFromHex(s.Value.Alumn), DecodeFromHex(s.Value.Type), DecodeFromHex(s.Value.date),"点歌 "+ DecodeFromHex(s.Value.Songname)));
                    k++;
                }
            }

            return temp;
        }
        public static string EncodeToHex(string input)
        {
            if (input == null)
            {
                input = "无";
            }
            var sb = new StringBuilder();
            foreach (byte b in Encoding.UTF8.GetBytes(input))
            {
                sb.AppendFormat("{0:x2}", b);  // Formats the byte as a two-digit hexadecimal number.
            }
            return sb.ToString();
        }

        public static string DecodeFromHex(string hexInput)
        {
            if (hexInput == null)
            {
                return "无";


            }
            var bytes = new byte[hexInput.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexInput.Substring(i * 2, 2), 16);
            }
            return Encoding.UTF8.GetString(bytes);
        }

        public static int pagenum=1;

        public async Task<IActionResult> IndexAsync(string sortOrder,int pagesize=10, int page = 1)
        {
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AuthorSortParm"] = sortOrder == "Author" ? "author_desc" : "Author";
            ViewData["LengthSortParm"] = sortOrder == "Length" ? "Length_desc" : "Length";

            if (songs == null) {
                songs = from s in await GetAllSongs()
               select s;
            }
            pagenum = page;

            switch (sortOrder)
            {
                case "name_desc":
                    songs = songs.OrderByDescending(s => s.Name);
                    break;
                case "Author":
                    songs = songs.OrderBy(s => s.Author);
                    break;
                case "author_desc":
                    songs = songs.OrderByDescending(s => s.Author);
                    break;
                case "Length":
                    songs = songs.OrderBy(s => s.Name.Length);
                    break;
                case "Length_desc":
                    songs = songs.OrderByDescending(s => s.Name.Length);
                    break;
                default:
                    songs = songs.OrderBy(s => s.Name.Length);
                    break;
            }
            int pageSize = pagesize; // Number of items per page
            var songlist = songs
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            var pageInfo = new PageInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = songs.Count()
            };

            var viewModel = new SongsViewModel
            {
                Songs = songlist,
                PageInfo = pageInfo
            };

            return View(viewModel);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString)
        {
            if (songs == null)
            {
                songs = from s in await GetAllSongs()
                        select s;
            }
 
            if (!string.IsNullOrEmpty(searchString))
            {
                var filteredsong = songs.Where(n =>
                 n.Name.ToLower().Contains(searchString.ToLower()) ||
                  n.Author.ToLower().Contains(searchString.ToLower())||
                  n.type.ToLower().Contains(searchString.ToLower())
                ).ToList();
                var pageInfo = new PageInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = 200,
                    TotalItems = filteredsong.Count()
                };
                var viewModel = new SongsViewModel
                {
                    Songs = filteredsong,
                    PageInfo = pageInfo
                };
                return View("Index", viewModel);
            }
            var pageInfo2 = new PageInfo
            {
                CurrentPage = 1,
                ItemsPerPage = 200,
                TotalItems = songs.Count()
            };
            var viewModel2 = new SongsViewModel
            {
                Songs = songs.ToList(),
                PageInfo = pageInfo2
            };
            return View("Index", viewModel2);
        }

        [HttpPost]
        public async Task<IActionResult> delete(int id)
        {
            if (songs == null)
            {
                songs = from s in await GetAllSongs()
                        select s;
            }
            var pageInfo = new PageInfo
            {
                CurrentPage = pagenum,
                ItemsPerPage = itemCount,
                TotalItems = songs.Count()
            };
            var temp = songs.Where(n => n.Id == id);
            if (temp != null)
            {
                await firebaseHelper.DeleteData("abc_123", "Songdb",EncodeToHex( temp.First().Name) + "_" +EncodeToHex( temp.First().Author));
            }
            var temp2 = songs.Where(n => n.Id != id);
            var viewModel2 = new SongsViewModel
            {
                Songs = temp2.ToList(),
                PageInfo = pageInfo
            };
            return View("Index", viewModel2);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id)
        {
            if (songs == null)
            {
                songs = from s in await GetAllSongs()
                        select s;
            }

            var temp = songs.Where(n => n.Id == id);
            if (temp != null)
            {
                if (temp.First().type.Equals("无")){
                    temp.First().type = "";
                }
                if (temp.First().date.Equals("无"))
                {
                    temp.First().date = "";
                }
                SongVM temp2 = new(temp.First().Name, temp.First().Author, temp.First().Alumn, temp.First().type, temp.First().date);
                return RedirectToAction("UpdateSong", "Account",temp2);
            }
            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> getallsongs()
        {
            if (songs == null)
            {
                songs = from s in await GetAllSongs()
                        select s;
            }
            return Json(JsonConvert.SerializeObject(songs));
        }

        public async Task<IActionResult> getsonglist()
        {
            HttpHelper x = new HttpHelper();

            string k=await x.getcontextAsync();
            songVM2 c = new songVM2();
            c.Songtext = k;
            Console.WriteLine("redirt");

            Console.WriteLine("接受成功");
            if (songs == null)
            {
                songs = from s in await GetAllSongs()
                        select s;
            }
            if (await SetSong(c))
            {
                TempData["Error"] = "添加成功!";
                return Content(k);

            };
            TempData["Error"] = "添加失败，找管理员!";
            return Content(k);
        }
        public async Task<bool> SetSong(songVM2 SongVM1)
        {
            foreach (SongVM songVM in ParseSongs(SongVM1.Songtext))
            {
                var temp = songs.Where(n => n.Name.Equals( songVM.SongName));
                if (temp.Count() != 0)
                {
                    if (temp.Where(n => n.Author.Equals(songVM.Actor)).Count() != 0)
                    {
                        Console.WriteLine("存在 " +temp.First().Name + " " + temp.First().Author);


                    }
                    else
                    {
                        if (!await firebaseHelper.UpdateData("abc_123", "Songdb", EncodeToHex(songVM.SongName) + "_" + EncodeToHex(songVM.Actor), new { Songname = EncodeToHex(songVM.SongName), author = EncodeToHex(songVM.Actor), Alumn = EncodeToHex(songVM.alumn), date = EncodeToHex(songVM.Date) }))
                        {
                            Console.WriteLine("失败 " + songVM.SongName + " " + songVM.Actor);
                        }
                        else
                        {
                            Console.WriteLine("添加成功 " + songVM.SongName + " " + songVM.Actor);
                        }
                    }


                }
                else
                {
                    if (!await firebaseHelper.UpdateData("abc_123", "Songdb", EncodeToHex(songVM.SongName) + "_" + EncodeToHex(songVM.Actor), new { Songname = EncodeToHex(songVM.SongName), author = EncodeToHex(songVM.Actor), Alumn = EncodeToHex(songVM.alumn), date = EncodeToHex(songVM.Date) }))
                    {
                        Console.WriteLine("失败 " + songVM.SongName + " " + songVM.Actor);
                    }
                    else
                    {
                        Console.WriteLine("添加成功 " + songVM.SongName + " " + songVM.Actor);
                    }


                }


            }


            return true;
        }
        static List<SongVM> ParseSongs(string data)
        {
            List<SongVM> songs = new List<SongVM>();
            var lines = data.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                Console.WriteLine(line);
                var details = line.Split(", ");
                var song = new SongVM();
                foreach (var detail in details)
                {
                    var parts = detail.Split(": ");
                    if (parts[0].Contains("歌曲名"))
                        song.SongName = parts[1];
                    else if (parts[0].Contains("歌手名"))
                        song.Actor = parts[1];
                    else if (parts[0].Contains("专辑名"))
                        song.alumn = parts[1];
                    else if (parts[0].Contains("时间"))
                        song.Date = parts[1];
                }
                songs.Add(song);
            }
            return songs;
        }

    }

}
