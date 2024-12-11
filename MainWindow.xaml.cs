using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Diagnostics;

namespace Kylosov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApiClient ApiClient;
        public MainWindow()
        {
            InitializeComponent();

            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.Expect100Continue = false;
        }

        private async void AuthBTN_Click(object sender, RoutedEventArgs e)
        {
            var apiClient = new ApiClient();
            try
            {
                string result = await apiClient.AuthenticateAsync(Login.Text, Password.Password, Server.Text);

                MessageBox.Show(result, "Результат авторизации", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}