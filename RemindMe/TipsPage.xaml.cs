using Microsoft.Extensions.Logging;
using RemindMe.Services;
using System.Collections.ObjectModel;

namespace RemindMe;

public partial class TipsPage : ContentPage
{
	public ObservableCollection<string> TipsList { get; set; }
    
	public TipsPage()
	{
		InitializeComponent();

		TipsList = new ObservableCollection<string>()
		{
			"Keep messages under 1 minute.",
			"Speak clearly and warmly.",
			"Record in a quiet, well-lit space with minimal distractions.",
			"Show the item or action you're referencing.",
			"Use straightforward sentences and repeat key points to enhance retention.",
			"Mention the time and day to help orient the listener.",
			"Offer gentle encouragement and express confidence in their abilities.",
			"Tailor the content to the individual, such as personalizing it with an introduction."
		};

		BindingContext = this;
	}

	private void OnReturnClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync($"//{nameof(ScheduleVideoPage)}").ConfigureAwait(false);
    }
}