using System.Text;
using System.Text.Json;

namespace ApocalypseSim
{
    internal class GPT4
    {
        private static readonly Lazy<GPT4> lazyInstance = new(() => new GPT4());
        private readonly HttpClient _httpClient;
        private Message _systemMessage = new("system", "You are a helpful AI assistant who provides daily apocalypse scenarios.");
        private List<Message> _conversation = new();

        private GPT4()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Environment.GetEnvironmentVariable("OPENAI_API_KEY")}");
            _conversation.Add(_systemMessage);
        }

        public static GPT4 Instance
        {
            get
            {
                return lazyInstance.Value;
            }
        }
        public void SetSystemMessage(string systemMessage)
        {
            _systemMessage = new Message("system", systemMessage);
            _conversation = new List<Message>
            {
                _systemMessage
            };
        }
        public async Task<string> Query(string prompt = "Once upon a time")
        {
            _conversation.Add(new Message("user", prompt));

            var request = new
            {
                model = "gpt-4",
                messages = _conversation,
                temperature = 1,
                max_tokens = 1024,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var jsonContent = JsonSerializer.Serialize(request, jsonOptions);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/chat/completions", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<CompletionResponse>(jsonResponse);
            var generatedText = responseData?.choices?[0]?.message?.content ?? string.Empty;

            // add it to conversation context!
            _conversation.Add(new Message("assistant", generatedText));

            return generatedText;
        }
    }

    // Types for serialization
#pragma warning disable IDE1006 // Naming Styles
    record Message(string role, string content);
    record Choice(Message message);
    record CompletionResponse(Choice[] choices);
#pragma warning restore IDE1006 // Naming Styles
}

