namespace PuzzleRunner
{
    public interface IAoCHttpClient
    {
        Task<string> GetInput(int year, int day);
    }

    public class AoCHttpClient(HttpClient httpClient) : IAoCHttpClient
    {
        public async Task<string> GetInput(int year, int day)
        {
            var url = $"https://adventofcode.com/{year}/day/{day}/input";

            return await httpClient.GetStringAsync(url);
        }
    }
}
