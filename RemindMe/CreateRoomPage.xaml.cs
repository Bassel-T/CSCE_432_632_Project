namespace RemindMe;

public partial class CreateRoomPage : ContentPage
{
    bool isPasswordVisible = false;

	public CreateRoomPage()
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

    private void CreateButtonClicked(object sender, EventArgs e)
    {

    }
}