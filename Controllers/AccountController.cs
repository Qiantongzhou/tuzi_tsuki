using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using tuzi_tsuki.firebase;
using tuzi_tsuki.Models;

namespace tuzi_tsuki.Controllers
{
    public class AccountController : Controller
    {
        FirebaseHelper firebaseHelper;
        private readonly ILogger<HomeController> _logger;
        public AccountController(ILogger<HomeController> logger)
        {
            firebaseHelper = new FirebaseHelper("https://tuzitsuki-default-rtdb.firebaseio.com");
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()=>View(new LoginVM());
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);
            dynamic temp = await GetUser(loginVM);
            if (temp != null)
            {
                if (temp.Passwords == loginVM.Password)
                {
                    var claims = new List<Claim>
            {
                    new Claim(ClaimTypes.Name, loginVM.FullName),
                    new Claim(ClaimTypes.Role, "Administrator")
            };
                    var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);

                    return RedirectToAction("Index", "Home");
                }
            }
            

            TempData["Error"] = "Wrong credentials. Please, try again!";
            return View(loginVM);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }
        public IActionResult Register()
        {
            return View(new RegisterVM());
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterVM registerVM)
        {
             if(await SetUser(registerVM))
            {
                return RedirectToAction("Index", "Home");
            };
            TempData["Error"] = "注册失败，找管理员!";
            return View(registerVM);
        }
        public IActionResult AddSong() => View(new SongVM());
        [HttpPost]
        public async Task<IActionResult> AddSong(SongVM songVM)
        {
            if (await SetSong(songVM))
            {
                TempData["Error"] = "添加"+songVM.SongName+"成功!";
                return View(songVM);

            };
            TempData["Error"] = "添加失败，找管理员!";
            return View(songVM);
        }
        public async Task<IActionResult> UpdateSong(SongVM songVM)
        {

            return View(songVM);
        }
        public async Task<IActionResult> UpdateSong2(SongVM songVM)
        {
            if (await SetSong(songVM))
            {
                TempData["Error"] = "添加" + songVM.SongName + "成功!";
                return RedirectToAction("Index", "Songs", new { page =SongsController.pagenum});

            };
            TempData["Error"] = "更新失败，找管理员!";
            return View(songVM);
        }

        public async Task<IActionResult> AddSong2() => View(new songVM2());

        [HttpPost]
        public async Task<IActionResult> AddSong2(songVM2 songVMs)
        {
            Console.WriteLine("接受成功");
            if (await SetSong(songVMs))
            {
                TempData["Error"] = "添加成功!";
                return View(songVMs);

            };
            TempData["Error"] = "添加失败，找管理员!";
            return View(songVMs);
        }
        public async Task<IActionResult> AddSong3(songVM2 songVMs)
        {
            Console.WriteLine("接受成功");
            if (await SetSong(songVMs))
            {
                TempData["Error"] = "添加成功!";
                return View(songVMs);

            };
            TempData["Error"] = "添加失败，找管理员!";
            return View(songVMs);
        }
        public async Task<bool> SetUser(RegisterVM registerVM)
        {
            return await firebaseHelper.UpdateData("abc_123", "Userdb", registerVM.FullName,new { EmailAddresss=registerVM.EmailAddress,Passwords=registerVM.Password, FullNames = registerVM.FullName });
        }
        public async Task<dynamic> GetUser(LoginVM loginVM)
        {


            return await firebaseHelper.GetData1<dynamic>("abc_123", "Userdb",loginVM.FullName);
        }

    

        public async Task<bool> SetSong(SongVM songVM)
        {


            return await firebaseHelper.UpdateData("abc_123", "Songdb", EncodeToHex(songVM.SongName) + "_" + EncodeToHex(songVM.Actor), new { Songname = EncodeToHex(songVM.SongName), author = EncodeToHex(songVM.Actor), Alumn = EncodeToHex(songVM.alumn), Type = EncodeToHex(songVM.type), date = EncodeToHex(songVM.Date) });
        }
        public async Task<bool> SetSong(songVM2 SongVM1)
        {
            foreach (SongVM songVM in ParseSongs(SongVM1.Songtext))
            {
                if (!await firebaseHelper.UpdateData("abc_123", "Songdb",EncodeToHex( songVM.SongName)+ "_" + EncodeToHex(songVM.Actor), new { Songname = EncodeToHex(songVM.SongName), author = EncodeToHex(songVM.Actor),Alumn= EncodeToHex(songVM.alumn), date = EncodeToHex(songVM.Date) }))
                {
                    Console.WriteLine("失败 " + songVM.SongName + " " + songVM.Actor);
                }
                else
                {
                    Console.WriteLine("添加成功 "+songVM.SongName + " " + songVM.Actor);
                }
            }


            return true;
        }
        static List<SongVM> ParseSongs(string data)
        {
            var songs = new List<SongVM>();
            var lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var details = line.Split('，');
                var song = new SongVM();
                foreach (var detail in details)
                {
                    var parts = detail.Split('：');
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

    }
}
