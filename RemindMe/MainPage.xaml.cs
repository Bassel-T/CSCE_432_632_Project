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

        private async void OnCreateRoomClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(CreateRoom.Text);

            // Go the Create Room Page
            await Shell.Current.GoToAsync($"//{nameof(CreateRoomPage)}?UserID={UserID}");
        }

        private void OnJoinRoomClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(JoinRoom.Text);
        }
    }

}
