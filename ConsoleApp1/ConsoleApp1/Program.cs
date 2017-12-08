using System;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace CSHttpClientSample
{
    static class Program
    {
        static void Main()
        {
            Console.Write("Enter the path to a JPEG image file:");
            string imageFilePath = Console.ReadLine();

            MakeRequest(imageFilePath);

            Console.WriteLine("\n\n\nWait for the result below, then hit ENTER to exit...\n\n\n");
            Console.ReadLine(); // wait for ENTER to exit program
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        static async void MakeRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9310b12896734586adcd598a6219cd79"); // 

            // NOTE: You must use the same region in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westcentralus, replace "westus" in the 
            //   URI below with "westcentralus".
            string uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?";
            HttpResponseMessage response;
            string responseContent;

            // Request body. Try this sample with a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                responseContent = response.Content.ReadAsStringAsync().Result;
            }

            // A peek at the raw JSON response.
            //Console.WriteLine(responseContent);

            // Processing the JSON into manageable objects.
            JToken rootToken = JArray.Parse(responseContent).First;

            // First token is always the faceRectangle identified by the API.
            JToken faceRectangleToken = rootToken.First;

            // Second token is all emotion scores.
            JToken scoresToken = rootToken.Last;

            // Show all face rectangle dimensions
            JEnumerable<JToken> faceRectangleSizeList = faceRectangleToken.First.Children();
            foreach (var size in faceRectangleSizeList)
            {
                //Console.WriteLine(size);
            }

            // Show all scores
            JEnumerable<JToken> scoreList = scoresToken.First.Children();
            double highestScore = 0;

            var output = "";

            foreach (var score in scoreList)
            {
                string emotion = "";

                //Console.WriteLine(output);
                //Console.WriteLine(score);

                double x = score.ToObject<double>();
                
                if (highestScore < x)
                {
                    highestScore = x;
                    emotion = score.ToString();
                    output = Regex.Replace(emotion, @"[^a-z]", string.Empty);

                }
            }

            string song = "";
            string url = "";

            if (output == "anger")
            {
                url = "https://www.youtube.com/watch?v=j0lSpNtjPM8";
                song = "Last Resort";
            }

            if (output == "happiness")
            {
                song = "Happy";
                url = "https://www.youtube.com/watch?v=ZbZSe6N_BXs";
            }

            if (output == "sadness")
            {
                song = "How To Save a Life";
                url = "https://www.youtube.com/watch?v=cjVQ36NhbMk";
            }

            if (output == "contempt")
            {
                song = "Forget You";
                url = "https://www.youtube.com/watch?v=bKxodgpyGec";
            }

            if (output == "disgust")
            {
                song = "Blurred Lines";
                url = "https://www.youtube.com/watch?v=yyDUC1LUXSU&has_verified=1";
            }

            if (output == "fear")
            {
                song = "Monster Mash";
                url = "https://www.youtube.com/watch?v=l2PoSljk8cE";
            }

            if (output == "neutral")
            {
                song = "Everlasting Light";
                url = "https://www.youtube.com/watch?v=QruSZ7xO7z4";
            }


            if (output == "surprise")
            {
                song = "Dick in a Box";
                url = "https://www.youtube.com/watch?v=VHQBgOZKk6k";
            }

            Console.WriteLine("Your emotion is: " + output + ". You should listen to: " + song + ".");

            Console.WriteLine("\nLoading song now...");

            System.Threading.Thread.Sleep(3000);

            Process.Start("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe", url);
        }
    }
}