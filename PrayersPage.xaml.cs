namespace MyPrayer
{
    public partial class PrayersPage : ContentPage
    {
        public PrayersPage()
        {
            InitializeComponent();
        }
        private async void OnPrayerClicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new JesusPrayer());
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
            else
            {
                
            }
        }

    
    }
}
