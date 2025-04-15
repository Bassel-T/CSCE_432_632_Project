using Microsoft.Extensions.Logging;
using RemindMe.Services;

namespace RemindMe;

public partial class JoinRoomPage : ContentPage
{
    bool isPasswordVisible = false;

    public JoinRoomPage()
    {
        InitializeComponent();
    }

    void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;
        PasswordEntry.IsPassword = !isPasswordVisible;
        TogglePasswordButton.Source = isPasswordVisible ? "visibility_off.svg" : "visibility.svg";
    }

    private async void CancelButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

    private async void JoinButtonClicked(object sender, EventArgs e)
    {
        var client = new BackendClient(new Logger<BackendClient>(new LoggerFactory()));
        var response = await client.JoinRoom(Id, RoomNameEntry.Text, PasswordEntry.Text).ConfigureAwait(false);

        if (response.Success)
        {
            await Shell.Current.GoToAsync($"//{nameof(AssistedRoom)}");
        }
        else
        {
            // THIS IS CRASHING THE SYSTEM
            // await DisplayAlert("Error", response.Message, "OK");
        }
    }
}