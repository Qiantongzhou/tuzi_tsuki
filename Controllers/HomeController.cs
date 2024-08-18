using Microsoft.AspNetCore.Mvc;
using OpenAI_hztec.Utilities;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using tuzi_tsuki.Models;

namespace tuzi_tsuki.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        public HomeController(IWebHostEnvironment environment,ILogger<HomeController> logger)
        {
            _logger = logger;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index","Songs");
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Share()
        {
            var temp=QrCodeHelper.GerQrCodeStream("https://tuzitsuki.azurewebsites.net/Songs");
            var iconPath = Path.Combine(_environment.WebRootPath, "image", "tuzi_icon.jpg");
            string content = "https://tuzitsuki.azurewebsites.net/Songs";
           Bitmap qrCode = QrCodeHelper.GenerateQRCode(content);
           Bitmap icon = new Bitmap(iconPath);  // Ensure icon size is smaller than QR code size
          Bitmap background = new Bitmap(iconPath);  // Ensure background is larger than QR code

          // Bitmap qrCodeWithIcon = QrCodeHelper.AddIconToQRCode(qrCode, icon);
           // Bitmap qrCodeWithBackground = QrCodeHelper.AddBackgroundToQRCode(qrCodeWithIcon, background);
            // Convert Bitmap to MemoryStream
            MemoryStream stream = new MemoryStream();
            qrCode.Save(stream, ImageFormat.Png);
            stream.Position = 0;

            // Dispose the Bitmap
            qrCode.Dispose();
            return File(stream, "image/png");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
