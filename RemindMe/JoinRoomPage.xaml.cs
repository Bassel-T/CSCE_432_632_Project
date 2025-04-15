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
        await DisplayAlert("TODO", "Feature not yet implemented", "OK");
    }
}