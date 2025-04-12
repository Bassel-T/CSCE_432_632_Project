namespace RemindMe
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCreateRoomClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(CreateRoom.Text);
        }

        private void OnJoinRoomClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce(JoinRoom.Text);
        }
    }

}
