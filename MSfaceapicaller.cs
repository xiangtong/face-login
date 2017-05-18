using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WeddingPlanner
{
    public class MSFaceApiRequest
    {
        // The second parameter is a function that returns a Dictionary of string keys to object values.
        // If an API returned an array as its top level collection the parameter type would be "Action>"
        public static async Task<string> FaceDetect(string apikey, byte[] imagecontent)
        {
            using (var Client = new HttpClient())
            {
                try
                {
                    Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apikey);
                    var uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/detect?returnFaceId=true";
                    var content = new ByteArrayContent(imagecontent);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    HttpResponseMessage Response = await Client.PostAsync(uri, content);
                    Response.EnsureSuccessStatusCode(); // Throw error if not successful.
                    string StringResponse = await Response.Content.ReadAsStringAsync(); 
                    List<Dictionary<string, object>> JsonResponse = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(StringResponse);       
                    return Convert.ToString(JsonResponse[0]["faceId"]);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                    return "Error";
                }
            }
        }
    public static async Task<Dictionary<string, object>> FaceVerify(string apikey, string faceid1,string faceid2)
        {
            // Create a temporary HttpClient connection.
            using (var Client = new HttpClient())
            {
                try
                {
                    Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apikey);
                    var uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/verify";
                    var content=new {faceId1=faceid1,faceId2=faceid2};
                    HttpResponseMessage Response = await Client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
                    Response.EnsureSuccessStatusCode(); // Throw error if not successful.
                    string StringResponse = await Response.Content.ReadAsStringAsync(); 
                    Dictionary<string, object> JsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(StringResponse);             
                    // Finally, execute our callback, passing it the response we got.
                    return JsonResponse;
                }
                catch (HttpRequestException e)
                {
                    // If something went wrong, display the error.
                    Console.WriteLine($"Request exception: {e.Message}");
                    var fail = new Dictionary<string, object>();
                    fail["result"]="fail";
                    return fail;
                }
            }
        }
        // public static async Task FaceDetect(string apikey, byte[] imagecontent,Action<List<Dictionary<string, object>>> Callback)
        // {
        //     // Create a temporary HttpClient connection.
        //     using (var Client = new HttpClient())
        //     {
        //         try
        //         {
        //             Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apikey);
        //             var uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/detect?returnFaceId=true";
        //             // Client.DefaultRequestHeaders.Add("ContentType","application/octet-stream");
        //             var content = new ByteArrayContent(imagecontent);
        //             content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //             HttpResponseMessage Response = await Client.PostAsync(uri, content);
        //             Response.EnsureSuccessStatusCode(); // Throw error if not successful.
        //             string StringResponse = await Response.Content.ReadAsStringAsync(); // Read in the response as a string.
                     
        //             // Then parse the result into JSON and convert to a dictionary that we can use.
        //             // DeserializeObject will only parse the top level object, depending on the API we may need to dig deeper and continue deserializing
        //             List<Dictionary<string, object>> JsonResponse = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(StringResponse);             
        //             // Finally, execute our callback, passing it the response we got.
        //             Callback(JsonResponse);
        //         }
        //         catch (HttpRequestException e)
        //         {
        //             // If something went wrong, display the error.
        //             Console.WriteLine($"Request exception: {e.Message}");
        //         }
        //     }
        // }
        // public static async Task FaceVerify(string apikey, string faceid1,string faceid2,Action<Dictionary<string, object>> Callback)
        // {
        //     // Create a temporary HttpClient connection.
        //     using (var Client = new HttpClient())
        //     {
        //         try
        //         {
        //             Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apikey);
        //             var uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/verify";
        //             var content=new {faceId1=faceid1,faceId2=faceid2};
        //             HttpResponseMessage Response = await Client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
        //             Response.EnsureSuccessStatusCode(); // Throw error if not successful.
        //             string StringResponse = await Response.Content.ReadAsStringAsync(); // Read in the response as a string.
                     
        //             // Then parse the result into JSON and convert to a dictionary that we can use.
        //             // DeserializeObject will only parse the top level object, depending on the API we may need to dig deeper and continue deserializing
        //             Dictionary<string, object> JsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(StringResponse);             
        //             // Finally, execute our callback, passing it the response we got.
        //             Callback(JsonResponse);
        //         }
        //         catch (HttpRequestException e)
        //         {
        //             // If something went wrong, display the error.
        //             Console.WriteLine($"Request exception: {e.Message}");
        //         }
        //     }
        // }
    }
}