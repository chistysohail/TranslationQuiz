namespace WebApiQuiz.DTO
{

    public class SearchResults
    {
        public string Header { get; set; }
        public List<string> File1Results { get; set; }
        public List<string> File2Results { get; set; }
        public List<string> File3Results { get; set; }

        public SearchResults(string header)
        {
            Header = header;
            File1Results = new List<string>();
            File2Results = new List<string>();
            File3Results = new List<string>();
        }
    }
}