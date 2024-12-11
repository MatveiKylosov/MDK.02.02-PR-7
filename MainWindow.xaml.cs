using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kylosov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void AuthBTN_Click(object sender, RoutedEventArgs e)
        {
            var gogs = new Gogs("Kylosov", "MatveiKylosov");

            // Выполняем асинхронную авторизацию
            bool response = await gogs.AuthorizationAsync();

            if (response)
            {
                // Если авторизация успешна
                Auth.Visibility = Visibility.Hidden;

                // Здесь добавьте вызов метода для получения репозиториев, если он есть
                // Например:
                // var repositories = await gogs.GetRepositoriesAsync();
                MessageBox.Show("Авторизация успешна", "Успех");
            }
            else
            {
                // Если авторизация не удалась
                MessageBox.Show("Не удалось авторизоваться", "Ошибка");
            }
        }
    }
}
