//namespace WebApiQuiz.Controllers
//{
//    public class DataController
//    {
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace WebApiQuiz.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private const string JsonFilePath =
            @"C:\Users\db\Documents\data-quran-master\data-quran-master\word-text\madani-qurancom.json";
        private const string JsonFilePath2 = @"C:\Users\db\Documents\data-quran-master\data-quran-master\word-translation\en-qurancom.json";

        [HttpGet("{id}")]
        public IActionResult GetData(int id)
        {
            // Read the JSON file
            var json = System.IO.File.ReadAllText(JsonFilePath);

            // Deserialize the JSON into a dictionary
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            // Read the JSON file
            var json2 = System.IO.File.ReadAllText(JsonFilePath2);

            // Deserialize the JSON into a dictionary
            var data2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(json2);

            // Retrieve the value corresponding to the given ID
            if (data.TryGetValue(id.ToString(), out string value))
            {
                if (data2.TryGetValue(id.ToString(), out string value2))
                    // Return the value as JSON
                    return Ok(new { id = id, value = value + value2});
                else
               
                    // Return a 404 error if the ID is not found in the file
                    return NotFound();
               
            }
            else
            {
                // Return a 404 error if the ID is not found in the file
                return NotFound();
            }
        }

        [HttpGet("search/{word}")]
        public IActionResult Search(string word)
        {
            // Read the first JSON file
            var json1 = System.IO.File.ReadAllText(JsonFilePath2);

            // Deserialize the JSON into a dictionary
            var data1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(json1);

            // Search for all keys in the first dictionary that contain the given word
            var keys = data1.Where(x => x.Value.Contains(word))
                .Select(x => x.Key);

            // Read the second JSON file
            var json2 = System.IO.File.ReadAllText(JsonFilePath);

            // Deserialize the JSON into a dictionary
            var data2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(json2);

            // Find all key-value pairs in the second dictionary that have keys that match the search results from the first dictionary
            var results = data2.Where(x => keys.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            if (results.Count > 0)
            {
                // Return the results as JSON
                return Ok(results);
            }
            else
            {
                // Return a 404 error if no results are found
                return NotFound();
            }
        }


    }
}
