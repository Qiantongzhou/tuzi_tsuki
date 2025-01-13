using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace tuzi_tsuki.firebase
{



    public class FirebaseHelper
    {
        private readonly FirebaseClient firebaseClient;
        //private readonly string dateFormat = "yyyyMMdd"; // Example date format: 20240101 for January 1, 2024

        public FirebaseHelper(string baseUrl)
        {
            firebaseClient = new FirebaseClient(baseUrl, new FirebaseOptions
            {
                AuthTokenAsyncFactory =null
            });
        }

        public async Task<string> GetData<T>(string accessString)
        {
            //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
            string typeNode = "userinfo";
            var data = await firebaseClient
                .Child(typeNode)
                .Child(accessString)
                .OnceAsJsonAsync();

            return data;
        }
        public async Task<string> GetData<T>(string accessString,string fieldName)
        {
            //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
            string typeNode = "userinfo";
            var data = await firebaseClient
                .Child(typeNode)
                .Child(accessString)
                .Child(fieldName)
                .OnceAsJsonAsync();

            return data;
        }
        public async Task<string> GetpureData<T>(string accessString, string fieldName)
        {
            //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
            
            var data = await firebaseClient
                .Child(accessString)
                .Child(fieldName)
                .OnceAsJsonAsync();

            return data;
        }
        public async Task<string> GetData<T>(string accessString, string fieldName,string fieldName2)
        {
            //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
            string typeNode = "userinfo";
            var data = await firebaseClient
                .Child(typeNode)
                .Child(accessString)
                .Child(fieldName)
                .Child(fieldName2)
                .OnceAsJsonAsync();

            return data;
        }
        public async Task<T> GetData1<T>(string accessString, string fieldName, string fieldName2)
        {
            //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
            string typeNode = "userinfo";
            var data = await firebaseClient
                .Child(typeNode)
                .Child(accessString)
                .Child(fieldName)
                .Child(fieldName2)
                .OnceSingleAsync<T>();

            return data;
        }
        public async Task<T> GetDatalast<T>(string accessString, string fieldName, string fieldName2)
        {
            //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
            string typeNode = "userinfo";
            var data = await firebaseClient
                .Child(typeNode)
                .Child(accessString)
                .Child(fieldName)
                .Child(fieldName2)
                .OrderByKey()
                .LimitToLast(1)
                .OnceAsync<T>();

            return data.FirstOrDefault().Object;
        }
        public async Task<T> GetDataSecondLast<T>(string accessString, string fieldName, string fieldName2)
        {
            string typeNode = "userinfo";

            // Retrieve the last two items
            var data = await firebaseClient
                .Child(typeNode)
                .Child(accessString)
                .Child(fieldName)
                .Child(fieldName2)
                .OrderByKey()
                .LimitToLast(5)
                .OnceAsync<T>();

            // Check if we have at least two items
            if (data.Count < 5)
            {
                return default(T); // Return the default value if there are less than two items
            }

            // Return the second-to-last item
            return data.First().Object;
        }
        public async Task DeleteData(string accessString, string fieldName, string fieldName2)
        {
            string typeNode = "userinfo";
            await firebaseClient
                .Child(typeNode)
                .Child(accessString)
                .Child(fieldName)
                .Child(fieldName2)
                .DeleteAsync();
        }

        public async Task<bool> SetData<T>(string accessString, T data)
        {
            try
            {
                // string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
                string typeNode = "userinfo";
                await firebaseClient
                    .Child(typeNode)
                    .Child(accessString)
                    .PutAsync(JsonConvert.SerializeObject(data));

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateData<T>(string accessString, string fieldName, T newValue)
        {
            try
            {
                //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
                string typeNode = "userinfo";
                await firebaseClient
                    .Child(typeNode)
                    .Child(accessString)
                    .Child(fieldName)  // Targeting the specific field
                    .PutAsync(JsonConvert.SerializeObject(newValue));

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateData<T>(string accessString, string fieldName,string fieldName2, T newValue)
        {
            try
            {
                //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
                string typeNode = "userinfo";
                await firebaseClient
                    .Child(typeNode)
                    .Child(accessString)
                    .Child(fieldName)  // Targeting the specific field
                    .Child(fieldName2)
                    .PutAsync(JsonConvert.SerializeObject(newValue));

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateData<T>(string accessString, string fieldName, string fieldName2, string fieldName3, T newValue)
        {
            try
            {
                //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
                string typeNode = "userinfo";
                await firebaseClient
                    .Child(typeNode)
                    .Child(accessString)
                    .Child(fieldName)  // Targeting the specific field
                    .Child(fieldName2)
                    .Child(fieldName3)
                    .PutAsync(JsonConvert.SerializeObject(newValue));

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> AddMessageWithLimit<T>(string accessString, T newMessage, int limit = 5)
        {
            try
            {
                //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
                string typeNode = "userinfo";
                // Get the current list of messages
                var messages = await firebaseClient
                    .Child(typeNode)
                    .Child(accessString)
                    .Child("messages")
                    .OnceSingleAsync<List<T>>();

                if (messages == null)
                {
                    messages = new List<T>();
                }

                // Add the new message
                messages.Add(newMessage);

                // If the list exceeds the limit, remove the oldest message(s)
                while (messages.Count > limit)
                {
                    messages.RemoveAt(0); // Removes the first element
                }
                
                // Update the list back to Firebase
                await firebaseClient
                    .Child(typeNode)
                    .Child(accessString)
                    .Child("messages")
                    .PutAsync(JsonConvert.SerializeObject(messages));

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<string> GetAllMessagesAsString(string accessString, string delimiter = "\n")
        {
            try
            {
                //string dateNode = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
                string typeNode = "userinfo";
                // Get the current list of messages
                var messages = await firebaseClient
                    .Child(typeNode)
                    .Child(accessString)
                    .Child("messages")
                    .OnceSingleAsync<List<string>>();

                if (messages == null || !messages.Any())
                {
                    return "";
                }

                // Concatenate all messages into a single string
                string allMessages = string.Join(delimiter, messages);

                return allMessages;
            }
            catch
            {
                // Handle or log the exception as appropriate
                return "";
            }
        }


    }


}
