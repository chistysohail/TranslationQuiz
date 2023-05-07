using Newtonsoft.Json;

namespace WebApiQuiz.DTO
{
    public class RandomSetData
    {
        public string Arabic { get; set; }
        public string ArabicEn { get; set; }
        public string Meaning { get; set; }

        

        public static class RandomSetGenerator
        {
            public static RandomQuestionClass GenerateRandomSets(string jsonFilePath1, string jsonFilePath2, string jsonFilePath3, int count = 4)
            {
                // Read the first JSON file
                var json1 = System.IO.File.ReadAllText(jsonFilePath1);

                // Deserialize the JSON into a dictionary
                var data1 = JsonConvert.DeserializeObject<Dictionary<string, string>>(json1);

                // Read the second JSON file
                var json2 = System.IO.File.ReadAllText(jsonFilePath2);

                // Deserialize the JSON into a dictionary
                var data2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(json2);

                // Read the third JSON file
                var json3 = System.IO.File.ReadAllText(jsonFilePath3);

                // Deserialize the JSON into a dictionary
                var data3 = JsonConvert.DeserializeObject<Dictionary<string, string>>(json3);

                List<RandomSetData> results = new List<RandomSetData>();
                RandomQuestionClass randomQuestion = new RandomQuestionClass();
                try
                {
                    // Select the specified number of random keys from the first dictionary
                    var randomKeys = data1.Keys.OrderBy(x => Guid.NewGuid()).Take(count);

                    // Build the results list using the randomly selected keys
                    foreach (var randomKey in randomKeys)
                    {
                        var set = new RandomSetData()
                        {
                            Arabic = data1[randomKey],
                            ArabicEn = data3.ContainsKey(randomKey) ? data3[randomKey] : "",
                            Meaning = data2.ContainsKey(randomKey) ? data2[randomKey] : ""
                        };
                        results.Add(set);
                        
                    }

                    
                    randomQuestion.Arabic1=results.FirstOrDefault().Arabic;
                    randomQuestion.ArabicEn1 = results.FirstOrDefault().ArabicEn;
                    randomQuestion.Meaning1 = results.FirstOrDefault().Meaning;
                    randomQuestion.Answer = results.FirstOrDefault().Meaning;
                    randomQuestion.Meaning2 = results.Skip(1).FirstOrDefault().Meaning;
                    randomQuestion.Meaning3 = results.Skip(2).FirstOrDefault().Meaning;
                    randomQuestion.Meaning4 = results.Skip(3).FirstOrDefault().Meaning;

                    // Shuffle the meanings
                    randomQuestion = ShuffleMeanings(randomQuestion);


                }
                catch (Exception ex)
                {
                    // Handle the exception here, e.g. log it, display an error message, etc.
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

                return randomQuestion;
            }
            public static RandomQuestionClass ShuffleMeanings(RandomQuestionClass question)
            {
                // Put the meanings in an array
                string[] meanings = { question.Meaning1, question.Meaning2, question.Meaning3, question.Meaning4 };

                // Shuffle the array using Fisher-Yates shuffle algorithm
                Random rnd = new Random();
                for (int i = meanings.Length - 1; i > 0; i--)
                {
                    int j = rnd.Next(i + 1);
                    string temp = meanings[i];
                    meanings[i] = meanings[j];
                    meanings[j] = temp;
                }

                // Assign the shuffled values back to the object properties
                question.Meaning1 = meanings[0];
                question.Meaning2 = meanings[1];
                question.Meaning3 = meanings[2];
                question.Meaning4 = meanings[3];

                // Return the updated object
                return question;
            }
        }

    }
}
