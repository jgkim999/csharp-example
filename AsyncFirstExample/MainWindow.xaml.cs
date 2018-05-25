using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace AsyncFirstExample
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            textBoxUrl.Text = "https://google.co.kr";

            textBlockContents.TextChanged += (sender, e) =>
            {
                if (textBlockContents.IsVisible)
                {
                    textBlockContents.ScrollToEnd();
                }
            };
        }

        private async void buttonGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = textBoxUrl.Text;
                int contentLength = await AccessTheWebAsync(url);

                textBlockContents.AppendText(string.Format("Length of the downloaded string: {0}.\r\n", contentLength));
            }
            catch (System.UriFormatException ex)
            {
                textBlockContents.AppendText(ex.ToString());
            }
            catch (HttpRequestException ex)
            {
                textBlockContents.AppendText(ex.ToString());
            }
        }

        async Task<int> AccessTheWebAsync(string url)
        {
            // You need to add a reference to System.Net.Http to declare client.
            HttpClient client = new HttpClient();

            // GetStringAsync returns a Task<string>. That means that when you await the
            // task you'll get a string (urlContents).
            Task<string> getStringTask = client.GetStringAsync(url);

            // You can do work here that doesn't rely on the string from GetStringAsync.
            DoIndependentWork();

            // The await operator suspends AccessTheWebAsync.
            //  - AccessTheWebAsync can't continue until getStringTask is complete.
            //  - Meanwhile, control returns to the caller of AccessTheWebAsync.
            //  - Control resumes here when getStringTask is complete.
            //  - The await operator then retrieves the string result from getStringTask.
            string urlContents = await getStringTask;

            // The return statement specifies an integer result.
            // Any methods that are awaiting AccessTheWebAsync retrieve the length value.
            textBlockContents.AppendText(urlContents);
            textBlockContents.AppendText("\r\n");
            return urlContents.Length;
        }

        void DoIndependentWork()
        {
            textBlockContents.AppendText("Working . . . . . . .\r\n");
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            textBlockContents.Clear();
        }
    }
}