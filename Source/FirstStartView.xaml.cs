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

namespace PdfDisplay
{
    /// <summary>
    /// Interaktionslogik für FirstStartView.xaml
    /// </summary>
    public partial class FirstStartView : UserControl
    {
        public FirstStartView()
        {
            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            var dc = this.DataContext as MainViewModel;
            dc.Browse();
        }
    }
}
