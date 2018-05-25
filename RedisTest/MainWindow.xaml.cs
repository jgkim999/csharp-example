using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace RedisTest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        ConnectionMultiplexer redis_;
        ISubscriber sub_;

        public MainWindow()
        {
            InitializeComponent();

            textBlockContents.TextChanged += (sender, e) =>
            {
                if (textBlockContents.IsVisible)
                {
                    textBlockContents.ScrollToEnd();
                }
            };

            redis_ = ConnectionMultiplexer.Connect("172.16.1.89:6379");
            string config = redis_.Configuration;
            textBlockContents.AppendText(config);
            textBlockSubscribeStatusBar.Text = "Subscribe OFF";
        }

        private async void buttonGo_Click(object sender, RoutedEventArgs e)
        {
            bool result = await SetGetTest();
            textBlockContents.AppendText(result.ToString());
        }

        async Task<bool> SetGetTest()
        {
            string key = "csharp_test_key";
            string value = "some 썸 띠리링 123";
            IDatabase db = redis_.GetDatabase();

            Task<bool> set_task = db.StringSetAsync(key, value);
            textBlockContents.AppendText("working... string set");
            bool set_result = await set_task;
            if (!set_result)
                return false;

            Task<RedisValue> get_task = db.StringGetAsync(key);
            RedisValue get_value = await get_task;
            textBlockContents.AppendText("Get value:");
            textBlockContents.AppendText(get_value.ToString());
            return true;
        }

        void SubAppendText(RedisChannel channel, string message)
        {
            textBlockSub.AppendText(message);

            string channel_str = channel.IsNullOrEmpty ? "null" : channel.ToString();
            textBlockStatusBar.Text = string.Format("channel:{0} message:{1}", channel_str, message);
        }

        private void buttonSub_Click(object sender, RoutedEventArgs e)
        {
            sub_ = redis_.GetSubscriber();
            if (sub_ == null)
            {
                textBlockStatusBar.Text = "Fail to subscribe the channel";
                textBlockSubscribeStatusBar.Text = "Subscribe OFF";
                return;
            }
            textBlockSubscribeStatusBar.Text = "Subscribe ON";

            sub_.Subscribe("messages", (channel, message) =>
            {
                this.Dispatcher.Invoke(new Action(() => { SubAppendText(channel, message); }));
            });
        }

        private void buttonPub_Click(object sender, RoutedEventArgs e)
        {
            ISubscriber sub = redis_.GetSubscriber();
            sub.Publish("messages", "Hello sub");
        }
    }
}