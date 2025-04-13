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

            // TODO : Cleanup
            if (!Directory.Exists(SystemConstants.USER_PATH))
            {
                var client = new BackendClient(new Logger<BackendClient>(new LoggerFactory()));
                var result = client.GenerateUserId().Result;

                if (result.Success)
                {
                    File.WriteAllText(SystemConstants.USER_PATH, result.Data.ToString());

                    UserID = result.Data;
                }
            } 
            else
            {
                UserID = Guid.Parse(File.ReadAllText(SystemConstants.USER_PATH));
            }
        }

        private async void OnCreateRoomClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(CreateRoom.Text);
            var userId = Guid.Parse(File.ReadAllText(SystemConstants.USER_PATH));

            // Go the Create Room Page
            await Shell.Current.GoToAsync($"//{nameof(CreateRoomPage)}?UserID={UserID}");
        }

        private void OnJoinRoomClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(JoinRoom.Text);
        }
    }

}
