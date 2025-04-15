namespace rainwords;

public partial class BaseMusicPage : ContentPage
{
	protected readonly IAudioService AudioService;

	public BaseMusicPage(IAudioService audioService)
	{
		AudioService = audioService;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if (this is Menu)
			AudioService.PlayMenuMusic();
		else if (this is MainPage)
			AudioService.PlayGameMusic();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		AudioService.StopAllMusic();
	}
}