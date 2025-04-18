using Microsoft.Extensions.Logging;
using RemindMe.Services;

namespace RemindMe
{
    public partial class MainPage : ContentPage
    {
        Guid? UserID { get; set; }
        BackendClient client;

        public MainPage()
        {
            InitializeComponent();

            client = new BackendClient();

            // TODO : Cleanup

            var userState = client.GetUserInRoomState().Result;

            if (userState.Data == Models.ResponseModels.UserInRoomState.NO_USER)
            {
                var response = client.GenerateUserId().Result;

                if (!response.Success)
                {
                    DisplayAlert("Error", "Could not generate user. Check your internet connection and restart the app.", "OK").RunSynchronously();
                    App.Current.Quit();
                }
            }
            else if (userState.Data == Models.ResponseModels.UserInRoomState.CAREGIVER)
            {
                Shell.Current.GoToAsync($"//{nameof(ScheduleVideoPage)}");
            }
            else if (userState.Data == Models.ResponseModels.UserInRoomState.ASSISTED)
            {
                Shell.Current.GoToAsync($"//{nameof(AssistedRoom)}");
            }
        }

        private async void OnCreateRoomClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(CreateRoom.Text);
            var userId = Guid.Parse(File.ReadAllText(SystemConstants.USER_PATH));

            // Go the Create Room Page
            await Shell.Current.GoToAsync($"//{nameof(CreateRoomPage)}?UserID={UserID}");
        }

        private async void OnJoinRoomClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(JoinRoom.Text);

            await Shell.Current.GoToAsync($"//{nameof(JoinRoomPage)}");
        }
    }

}
