using Microsoft.Extensions.Logging;
using RemindMe.Services;

namespace RemindMe
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        Guid? UserID { get; set; }

        public MainPage()
        {
            InitializeComponent();

            var userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "user");

            if (!Directory.Exists(userPath))
            {
                var client = new BackendClient(new Logger<BackendClient>(new LoggerFactory()));
                var result = client.GenerateUserId().Result;

                if (result.Success)
                {
                    File.WriteAllText(userPath, result.Data.ToString());

                    UserID = result.Data;
                }
            } 
            else
            {
                UserID = Guid.Parse(File.ReadAllText(userPath));
            }
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
