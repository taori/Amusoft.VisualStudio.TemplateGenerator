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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Generator.Client.Desktop.Controls
{
    /// <summary>
    /// Interaktionslogik für LabeledControl.xaml
    /// </summary>
    [ContentProperty(nameof(ContentControl))]
    public partial class LabeledControl : UserControl
    {
        public LabeledControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
	        nameof(LabelText), typeof(string), typeof(LabeledControl), new PropertyMetadata(default(string)));

        public string LabelText
        {
	        get { return (string) GetValue(LabelTextProperty); }
	        set { SetValue(LabelTextProperty, value); }
        }

        public static readonly DependencyProperty ContentControlProperty = DependencyProperty.Register(
	        nameof(ContentControl), typeof(Visual), typeof(LabeledControl), new PropertyMetadata(default(Visual)));

        public Visual ContentControl
        {
	        get { return (Visual) GetValue(ContentControlProperty); }
	        set { SetValue(ContentControlProperty, value); }
        }

        public static readonly DependencyProperty TooltipProperty = DependencyProperty.Register(
	        nameof(Tooltip), typeof(object), typeof(LabeledControl), new PropertyMetadata(default(object)));

        public object Tooltip
        {
	        get { return (object) GetValue(TooltipProperty); }
	        set { SetValue(TooltipProperty, value); }
        }
    }
}
