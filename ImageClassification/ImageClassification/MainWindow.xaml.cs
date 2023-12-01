using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageClassification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Select an image",
                Multiselect = false,                
                InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString()
            };
            if (dlg.ShowDialog() == true ) 
            {
                this.img.Source =   new BitmapImage(new Uri(dlg.FileName));
            }

        }

        private void btnProcessImage_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            BitmapImage imageSource = (BitmapImage)img.Source;
             using (MemoryStream memoryStream = new MemoryStream())
             {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(imageSource));
                encoder.Save(memoryStream);
                byte[] byteArray = memoryStream.ToArray();

                ML_Mineral_ID.ModelInput sampleData = new ML_Mineral_ID.ModelInput()
                {
                    ImageSource = byteArray,
                };

                // Make a single prediction on the sample data and print results.
                var sortedScoresWithLabel = ML_Mineral_ID.PredictAllLabels(sampleData);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{"Class",-40}{"Score",-20}");
                foreach (var score in sortedScoresWithLabel)
                {
                    sb.AppendLine($"{score.Key,-40}{score.Value,-20}");                 
                }
                txtResult.Text = sb.ToString();
            }
            Mouse.OverrideCursor = null;

        }
    }
}