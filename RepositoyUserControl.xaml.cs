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
    /// Логика взаимодействия для RepositoyUserControl.xaml
    /// </summary>
    public partial class RepositoyUserControl : UserControl
    {
        public RepositoyUserControl(string name, string lastUpdate)
        {
            InitializeComponent();
            this.Name.Text = name;
            this.LastUpdate.Text = lastUpdate;
        }
    }
}
