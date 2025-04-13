namespace RemindMe;

public partial class ScheduleVideoPage : ContentPage
{
	public DateTime minDate = DateTime.Now;

	public ScheduleVideoPage()
	{
		InitializeComponent();

		DatePicker.BindingContext = this;
	}
}