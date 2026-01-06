using QuizTime.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace QuizTime.ViewModels
{
    class MainViewmodel : INotifyPropertyChanged
    {

        private string _supertitle;
        private string _title;
        private List<oQuestion> _questions;
        private oQuestion _currentQuestion;
        private string _selectedOption;
        private bool? _isAnswerCorrect;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task getQuestions()
        {

            var httpClient = new HttpClient();
            var URL = "http://restapi.local/all";
            // code to fetch questions from an API or database
            try
            {
                var response = await httpClient.GetAsync(URL);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // Process the content, e.g., deserialize JSON to a list of questions
                    Questions = JsonSerializer.Deserialize<List<oQuestion>>(json);
                    


                }
                else
                {
                    // Handle error response
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., network errors
            }
        }

       
        /// <summary>
        /// constructor
        /// </summary>
        public MainViewmodel()
        {
            SuperTitel = "Welcome to the Quiz!";
            // Initialize the ViewModel, e.g., fetch questions
            _ = getQuestions(); // Uncomment if you implement the method
        }

        public string SuperTitel { 
            get
            {
                return _supertitle;
            } 
            set
            {
                _supertitle = value;
                OnPropertyChanged(nameof(SuperTitel));

            }

        }

        // the questions as property
        public List<oQuestion> Questions {
            get { return _questions; }
            set { 
                _questions = value; 
                OnPropertyChanged(nameof(Questions));
            } 
        }

        public oQuestion CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                OnPropertyChanged(nameof(CurrentQuestion));
                // Reset de kleur naar grijs bij een nieuwe vraag
                IsAnswerCorrect = null;
            }
        }

        public string Title { 
            get { 
                return _title; 
            } 
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));

            }
        }

  
        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                _selectedOption = value;
                OnPropertyChanged(nameof(SelectedOption));
                // Hier kun je checken of het antwoord correct is
                CheckAnswer();
            }
        }

        private void CheckAnswer()
        {
            if (CurrentQuestion != null && SelectedOption != null)
            {
                int selectedIndex = CurrentQuestion.Options.IndexOf(SelectedOption);
                IsAnswerCorrect = selectedIndex == CurrentQuestion.Answer;
                // Doe iets met het resultaat
            }
        }

        
        public bool? IsAnswerCorrect
        {
            get => _isAnswerCorrect;
            set
            {
                _isAnswerCorrect = value;
                OnPropertyChanged(nameof(IsAnswerCorrect));
            }
        }
    }
}
