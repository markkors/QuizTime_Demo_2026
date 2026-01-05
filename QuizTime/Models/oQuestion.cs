using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuizTime.Models
{
    public class oQuestion
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("question")]
        public string? Question { get; set; }

        [JsonPropertyName("options")]
        public List<string>? Options { get; set; }

        [JsonPropertyName("answer")]
        public int Answer { get; set; }

        public string GetCorrectAnswer()
        {
            return Options[Answer];
        }

        public bool IsCorrectAnswer(int selectedIndex) => selectedIndex == Answer;
    }
}
