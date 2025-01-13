using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text.Json;
using tuzi_tsuki.firebase;
using tuzi_tsuki.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tuzi_tsuki.Controllers
{
    public class DataController : Controller
    {
        FirebaseHelper firebaseHelper;
        FirebaseHelper firebaseHelper1;
        int itemCount = 10;
        public DataController()
        {
            this.firebaseHelper = new FirebaseHelper("https://tuzitsuki-default-rtdb.firebaseio.com");
            this.firebaseHelper1 = new FirebaseHelper("https://tuzigiftdb-default-rtdb.firebaseio.com");
        }
        [HttpPost]
        public async Task<IActionResult> ReceiveJsonFans()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                // Read the JSON data as a string
                string json = await reader.ReadToEndAsync();

                if (!await firebaseHelper.UpdateData("abc_123", "Datadb","Fans",gettime(), json))
                {
                    Console.WriteLine("失败 ");
                }
                else
                {
                    Console.WriteLine("添加成功 ");
                }

                return Ok(new { message = "JSON received successfully." });
            }
        }


        //public async Task<IActionResult> ReceiveJson([FromHeader] bilidata1 data)
        //{
        //    if (data == null)
        //    {
        //        return Ok(new { message = "data is null" });
        //    }
        //    if (!await firebaseHelper.UpdateData("abc_123", "Datadb", data.uid))
        //    {//, gettime(),new { Namex = data.Name, Fans = data.fans, Guards = data.guards, Level = data.level, Nextlevelxp = data.nextlevelxp, Currentlevelxp=data.currentlevelxp }
        //        Console.WriteLine("失败 ");
        //        return Ok(new { message = "JSON update received failed." + data.uid });
        //    }
        //    else
        //    {
        //        Console.WriteLine("添加成功 ");

        //        return Ok(new { message = "JSON received successfully."+ data.uid });
        //    }


        //}

        public async Task<IActionResult> ReceiveJson()
        {
            bilidata1 x = new bilidata1();

            // Manually read the body as a string
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string jsonString = await reader.ReadToEndAsync();

                if (string.IsNullOrEmpty(jsonString))
                {
                    return BadRequest("Received data is null or empty.");
                }

                try
                {
                    // Deserialize the JSON string into a dictionary with JsonElements
                    var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

                    // Access the elements in the dictionary and convert to string explicitly
                    x.Name = data["Name"].GetString();
                    x. uid = data["uid"].GetString();
                    x. guards = data["guards"].GetRawText(); // Use GetRawText to get the original value as a string
                    x. fans = data["fans"].GetRawText();
                    x. level = data["level"].GetRawText();
                    x.nextlevelxp = data["nextlevelxp"].GetRawText();
                    x. currentlevelxp = data["currentlevelxp"].GetRawText();

                    // process the data as needed

                   
                }
                catch (Exception ex)
                {
                    return BadRequest("Error processing data: " + ex.Message);
                }
           
            }
            if (!await firebaseHelper.UpdateData("abc_123", "datadb", x.uid, gettime(), new { namex = x.Name, fans = x.fans, guards = x.guards, level = x.level, nextlevelxp = x.nextlevelxp, currentlevelxp = x.currentlevelxp }))
            {//,
                Console.WriteLine("失败 ");
                return Ok(new { message = "json update received failed." + x.uid });
            }
            else
            {
                Console.WriteLine("添加成功 ");

                return Ok(new { message = "json received successfully." + x.uid });
            }

        }
        public async Task<IActionResult> Getzhiboinfofull(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                return BadRequest("Received data is null or empty.");
            }

            // Get the last object from Firebase
            object lastObject = await firebaseHelper.GetData<object>("abc_123", "datadb", uid);

            // If the last object is null, return a NotFound result
            if (lastObject == null)
            {
                return NotFound("No data found for the provided uid.");
            }

            // Return the JSON object directly, no need to serialize again
            return Json(lastObject);
        }
        public async Task<IActionResult> Getzhiboinfo(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                return BadRequest("Received data is null or empty.");
            }

            // Get the last object from Firebase
            object lastObject = await firebaseHelper.GetDatalast<object>("abc_123", "datadb", uid);

            // If the last object is null, return a NotFound result
            if (lastObject == null)
            {
                return NotFound("No data found for the provided uid.");
            }

            // Return the JSON object directly, no need to serialize again
            return Json(JsonConvert.SerializeObject(lastObject));
        }
        public async Task<IActionResult> Getzhiboinfo1(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                return BadRequest("Received data is null or empty.");
            }

            // Get the last object from Firebase
            object lastObject = await firebaseHelper.GetDataSecondLast<object>("abc_123", "datadb", uid);

            // If the last object is null, return a NotFound result
            if (lastObject == null)
            {
                return NotFound("No data found for the provided uid.");
            }

            // Return the JSON object directly, no need to serialize again
            return Json(JsonConvert.SerializeObject(lastObject));
        }
        public async Task<IActionResult> Getzhiboinfogift(string date,string roomId)
        {
            if (string.IsNullOrEmpty(date))
            {
                return BadRequest("Received data is null or empty.");
            }
            if (string.IsNullOrEmpty(roomId))
            {
                return BadRequest("Received data is null or empty.");
            }
            // Get the last object from Firebase
            object lastObject = await firebaseHelper1.GetpureData<object>(date, roomId);

            // If the last object is null, return a NotFound result
            if (lastObject == null)
            {
                return NotFound("No data found for the provided uid.");
            }

            // Return the JSON object directly, no need to serialize again
            return Json(lastObject);
        }
        public string gettime()
        {
            // Get the current date and time in UTC
            DateTime utcNow = DateTime.UtcNow;

            // Define China Standard Time (CST) timezone
            TimeZoneInfo chinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

            // Convert the UTC time to China Standard Time
            DateTime chinaTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, chinaTimeZone);

            // Format it as a single string
            string formattedDateTime = chinaTime.ToString("yyyy-MM-dd_HH-mm-ss");
            return formattedDateTime;
        }


    }
}
