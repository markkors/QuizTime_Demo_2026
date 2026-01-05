using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuizTime.ViewModels;

namespace QuizTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MainViewmodel vm;

        public MainWindow()
        {
            InitializeComponent();
            vm = new MainViewmodel();
            this.DataContext = vm;
            vm.Title = "Welkom bij Quiztime";


        }

       

        
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string t;
            t = (sender as TextBox).Text;
            Debug.WriteLine(t);
            vm.SuperTitel = t;

        }
    }
}