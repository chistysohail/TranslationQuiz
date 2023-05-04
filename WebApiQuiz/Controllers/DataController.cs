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

        private const string JsonFilePath2 =
            @"C:\Users\db\Documents\data-quran-master\data-quran-master\word-translation\en-qurancom.json";

        private const string JsonFilePath3 =
            @"C:\Users\db\Documents\data-quran-master\data-quran-master\word-transliteration\en-qurancom.json";

        private const string markdownFilePath1 =
            "C:\\Users\\db\\Documents\\data-quran-master\\data-quran-master\\ayah-translation\\en-ahmedali-tanzil.md";

        private const string markdownFilePath2 =
            "C:\\Users\\db\\Documents\\data-quran-master\\data-quran-master\\ayah-transliteration\\en-transliteration-tanzil.md";

        private const string markdownFilePath3 =
            "C:\\Users\\db\\Documents\\data-quran-master\\data-quran-master\\ayah-text\\simple.md";

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
                    return Ok(new { id = id, value = value + value2 });
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


        //[HttpGet("searchWordAya/{word}")]
        //public IActionResult SearchWordAya(string word)
        [HttpGet("searchWordAya/{word}")]
        public IActionResult SearchWordAya(string word)
        {
            //string markdownFilePath = "path/to/your/markdown/file.md";
            // string markdownFilePath2 = "path/to/your/second/markdown/file.md";

            string markdownContent1 = System.IO.File.ReadAllText(markdownFilePath1);
            string markdownContent2 = System.IO.File.ReadAllText(markdownFilePath2);

            // Split the Markdown content into an array of lines
            string[] lines = markdownContent1.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

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
                        // Split the header content into an array of lines
                        string[] headerLines =
                            markdownContent2.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                        // Loop through the lines and find the header that matches the current section
                        string header = "";
                        for (int j = 0; j < headerLines.Length; j++)
                        {
                            if (headerLines[j].IndexOf(heading, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                header = headerLines[j];
                                break;
                            }
                        }

                        // Add the header, content, and header content to the results dictionary
                        results[header] = sectionContent.Trim();
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

        [HttpGet("search/header/{word}")]
        public IActionResult SearchHeader(string word)
        {
            //string markdownFilePath = "path/to/your/markdown/file.md";
            string markdownContent = System.IO.File.ReadAllText(markdownFilePath1);

            // Split the Markdown content into an array of lines
            string[] lines = markdownContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<string> results = new List<string>();

            // Loop through the lines and find the sections that contain the search word
            for (int i = 0; i < lines.Length; i++)
            {
                // Check if the line matches a heading
                if (lines[i].StartsWith("#"))
                {
                    // If it does, get the heading number and store it temporarily
                    string heading = lines[i].TrimStart('#', ' ');
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
                                // If it does, add the heading to the results list
                                results.Add(heading);
                                break;
                            }
                        }
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

        [HttpGet("search/content/{header}")]
        public IActionResult SearchContent(string header)
        {
            //string markdownFilePath1 = "path/to/your/first/markdown/file.md";
            //string markdownFilePath2 = "path/to/your/second/markdown/file.md";
            //string markdownFilePath3 = "path/to/your/third/markdown/file.md";

            string markdownContent1 = System.IO.File.ReadAllText(markdownFilePath1);
            string markdownContent2 = System.IO.File.ReadAllText(markdownFilePath2);
            string markdownContent3 = System.IO.File.ReadAllText(markdownFilePath3);

            // Split the Markdown contents into arrays of lines
            string[] lines1 = markdownContent1.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string[] lines2 = markdownContent2.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string[] lines3 = markdownContent3.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            string sectionContent1 = "";
            string sectionContent2 = "";
            string sectionContent3 = "";

            // Loop through the lines of the first Markdown file and find the section that matches the header
            for (int i = 0; i < lines1.Length; i++)
            {
                // Check if the line matches the header
                if (lines1[i].StartsWith("#") && lines1[i].TrimStart('#', ' ') == header)
                {
                    // If it does, iterate through the lines until we reach the next heading or the end of the file
                    for (int j = i + 1; j < lines1.Length; j++)
                    {
                        // Check if the current line is a heading
                        if (lines1[j].StartsWith("#"))
                        {
                            // If it is, we've reached the next section, so break out of the loop
                            break;
                        }
                        else
                        {
                            // If it's not, append it to the section content
                            sectionContent1 += lines1[j] + Environment.NewLine;
                        }
                    }
                }
            }

            // Loop through the lines of the second Markdown file and find the section that matches the header
            for (int i = 0; i < lines2.Length; i++)
            {
                // Check if the line matches the header
                if (lines2[i].StartsWith("#") && lines2[i].TrimStart('#', ' ') == header)
                {
                    // If it does, iterate through the lines until we reach the next heading or the end of the file
                    for (int j = i + 1; j < lines2.Length; j++)
                    {
                        // Check if the current line is a heading
                        if (lines2[j].StartsWith("#"))
                        {
                            // If it is, we've reached the next section, so break out of the loop
                            break;
                        }
                        else
                        {
                            // If it's not, append it to the section content
                            sectionContent2 += lines2[j] + Environment.NewLine;
                        }
                    }
                }
            }

            // Loop through the lines of the third Markdown file and find the section that matches the header
            for (int i = 0; i < lines3.Length; i++)
            {
                // Check if the line matches the header
                if (lines3[i].StartsWith("#") && lines3[i].TrimStart('#', ' ') == header)
                {
                    // If it does, iterate through the lines until we reach the next        heading or the end of the file
                    for (int j = i + 1; j < lines3.Length; j++)
                    {
                        // Check if the current line is a heading
                        if (lines3[j].StartsWith("#"))
                        {
                            // If it is, we've reached the next section, so break out of the loop
                            break;
                        }
                        else
                        {
                            // If it's not, append it to the section content
                            sectionContent3 += lines3[j] + Environment.NewLine;
                        }
                    }
                }
            }

            // Check if there was a match in all three files
            if (!string.IsNullOrEmpty(sectionContent1) && !string.IsNullOrEmpty(sectionContent2) &&
                !string.IsNullOrEmpty(sectionContent3))
            {
                // Combine the section content from all three files
                string combinedContent = sectionContent1 + Environment.NewLine + sectionContent2 + Environment.NewLine +
                                         sectionContent3;

                // Return the combined section content
                return Ok(combinedContent.Trim());
            }
            // Check if there was a match in the first two files
            else if (!string.IsNullOrEmpty(sectionContent1) && !string.IsNullOrEmpty(sectionContent2))
            {
                // Combine the section content from the first two files
                string combinedContent = sectionContent1 + Environment.NewLine + sectionContent2;

                // Return the combined section content
                return Ok(combinedContent.Trim());
            }
            // Check if there was a match in the first and third files
            else if (!string.IsNullOrEmpty(sectionContent1) && !string.IsNullOrEmpty(sectionContent3))
            {
                // Combine the section content from the first and third files
                string combinedContent = sectionContent1 + Environment.NewLine + sectionContent3;

                // Return the combined section content
                return Ok(combinedContent.Trim());
            }
            // Check if there was a match in the second and third files
            else if (!string.IsNullOrEmpty(sectionContent2) && !string.IsNullOrEmpty(sectionContent3))
            {
                // Combine the section content from the second and third files
                string combinedContent = sectionContent2 + Environment.NewLine + sectionContent3;

                // Return the combined section content
                return Ok(combinedContent.Trim());
            }
            // Check if there was a match in the first file
            else if (!string.IsNullOrEmpty(sectionContent1))
            {
                // Return the section content from the first file
                return Ok(sectionContent1.Trim());
            }
            // Check if there was a match in the second file
            else if (!string.IsNullOrEmpty(sectionContent2))
            {
                // Return the section content from the second file
                return Ok(sectionContent2.Trim());
            }
            // Check if there was a match in the third file
            else if (!string.IsNullOrEmpty(sectionContent3))
            {
                // Return the section content from the third file
                return Ok(sectionContent3.Trim());
            }
            else
            {
                // Return a 404 error if no sections are found that match the search header
                return NotFound();
            }


        }
    }
}
