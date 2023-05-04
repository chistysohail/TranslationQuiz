//namespace WebApiQuiz.Controllers
//{
//    public class DataController
//    {
//    }
//}
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace WebApiQuiz.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private const string JsonFilePath1 =
            @"C:\Users\db\Documents\data-quran-master\data-quran-master\word-text\madani-qurancom.json";
        private const string JsonFilePath2 = @"C:\Users\db\Documents\data-quran-master\data-quran-master\word-translation\en-qurancom.json";
        private const string JsonFilePath3 =
            @"C:\Users\db\Documents\data-quran-master\data-quran-master\word-transliteration\en-qurancom.json";
        private const string markdownFilePath1 = "C:\\Users\\db\\Documents\\data-quran-master\\data-quran-master\\ayah-translation\\en-ahmedali-tanzil.md";
        private const string markdownFilePath2= "C:\\Users\\db\\Documents\\data-quran-master\\data-quran-master\\ayah-transliteration\\en-transliteration-tanzil.md";
        [HttpGet("{id}")]
        public IActionResult GetData(int id)
        {
            // Read the JSON file
            var json = System.IO.File.ReadAllText(JsonFilePath1);

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
            var keys = new HashSet<string>(data1.Where(x => x.Value.Contains(word)).Select(x => x.Key));

            // Read the second JSON file
            var json2 = System.IO.File.ReadAllText(JsonFilePath1);

            // Deserialize the JSON into a dictionary
            var data2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(json2);

            // Read the third JSON file
            var json3 = System.IO.File.ReadAllText(JsonFilePath3);

            // Deserialize the JSON into a dictionary
            var data3 = JsonConvert.DeserializeObject<Dictionary<string, string>>(json3);

            Dictionary<string, Dictionary<string, string>> results = null;
            try
            {
                // Find all key-value pairs in the second dictionary that have keys that match the search results from the first dictionary
                results = data2.Where(x => keys.Contains(x.Key))
                               .ToDictionary(x => x.Key, x => new Dictionary<string, string>()
                               {
                           { "Arabic", x.Value },
                          // { "key1", data1.ContainsKey(x.Key) ? x.Key : "" },
                          { "Arabic-en", data3.ContainsKey(x.Key) ? data3[x.Key] : "" },
                           { "Meaning", data1.ContainsKey(x.Key) ? data1[x.Key] : "" }
                           
                               });
            }
            catch (Exception ex)
            {
                // Handle the exception here, e.g. log it, display an error message, etc.
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            if (results != null && results.Count > 0)
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

        [HttpGet("heading/{heading}")]
        public IActionResult GetContentForHeading(string heading)
        {
            
            string markdownContent = System.IO.File.ReadAllText(markdownFilePath1);

            // Split the Markdown content into an array of lines
            string[] lines = markdownContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // Loop through the lines and find the content for the given heading
            string content = null;
            for (int i = 0; i < lines.Length; i++)
            {
                // Check if the line matches the heading
                if (lines[i].StartsWith($"# {heading}"))
                {
                    // Get the content for the section by iterating through the lines until we reach the next heading or the end of the file
                    StringBuilder sectionContent = new StringBuilder();
                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        // Check if the current line is a heading
                        if (lines[j].StartsWith("#"))
                        {
                            // If it is, we've reached the next section, so break out of the loop
                            break;
                        }
                        else
                        {
                            // If it's not, append the line to the section content
                            sectionContent.AppendLine(lines[j]);
                        }
                    }
                    content = sectionContent.ToString().Trim();
                    break;
                }
            }

            if (content != null)
            {
                // Return the content as plain text
                return Content(content, "text/plain");
            }
            else
            {
                // Return a 404 error if the heading is not found
                return NotFound();
            }
        }

        [HttpGet("searchWord/{word}")]
        public IActionResult SearchWord(string word)
        {
            
            string markdownContent = System.IO.File.ReadAllText(markdownFilePath1);

            // Split the Markdown content into an array of lines
            string[] lines = markdownContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            Dictionary<string, string> results = new Dictionary<string, string>();

            // Loop through the lines and find the sections that contain the search word
            for (int i = 0; i < lines.Length; i++)
            {
                // Check if the line matches a heading
                if (lines[i].StartsWith("#"))
                {
                    // If it does, get the heading number and store it temporarily
                    string heading = lines[i].TrimStart('#', ' ');
                    string sectionContent = "";
                    // Iterate through the lines until we reach the next heading or the end of the file
                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        // Check if the current line is a heading
                        if (lines[j].StartsWith("#"))
                        {
                            // If it is, we've reached the next section, so break out of the loop
                            break;
                        }
                        else
                        {
                            // If it's not, check if it contains the search word
                            if (lines[j].IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                // If it does, add the section content to the results dictionary with the heading as the key
                                sectionContent += lines[j] + Environment.NewLine;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sectionContent))
                    {
                        results[heading] = sectionContent.Trim();
                    }
                }
            }

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
