using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace MyPrayer
{
    public partial class MyPrayersPage : ContentPage
    {
        private FirebaseService _firebaseService;
        private List<Prayer> _prayers = new List<Prayer>();

        public MyPrayersPage()
        {
            InitializeComponent();
            _firebaseService = new FirebaseService(); // FirebaseService instanziieren
            LoadPrayers();
        }

        private async void OnAddPrayerClicked(object sender, EventArgs e)
        {
            var addPrayerPage = new AddPrayerToPrayersPage();
            addPrayerPage.PrayerAdded += OnPrayerAdded; // Event abonnieren
            await Navigation.PushAsync(addPrayerPage);
        }

        // Lade alle Gebete aus Firebase
        private async void LoadPrayers()
        {
            _prayers = await _firebaseService.GetPrayersAsync(); // Gebete aus Firebase laden
            UpdatePrayerButtons(); // UI aktualisieren
        }

        // Wenn ein Gebet hinzugef�gt wird
        private async void OnPrayerAdded(object sender, Prayer prayer)
        {
            _prayers.Add(prayer); // Gebet zur Liste hinzuf�gen
            await _firebaseService.AddPrayerAsync(prayer); // Gebet in Firebase speichern
            UpdatePrayerButtons(); // UI aktualisieren
        }

        // Event-Handler f�r das L�schen eines Gebets
        private async void OnDeletePrayerClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var prayer = (Prayer)button.BindingContext;

            if (prayer != null)
            {
                bool isConfirmed = await DisplayAlert("Best�tigung", "M�chten Sie dieses Gebet wirklich l�schen?", "Ja", "Nein");
                if (isConfirmed)
                {
                    try
                    {
                        await _firebaseService.DeletePrayerAsync(prayer); // Gebet aus Firebase l�schen
                        _prayers.Remove(prayer); // Aus der lokalen Liste entfernen
                        UpdatePrayerButtons(); // UI aktualisieren
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Fehler", "Das Gebet konnte nicht gel�scht werden: " + ex.Message, "OK");
                    }
                }
            }
        }

        // Aktualisiere die UI, um alle Gebete anzuzeigen
        private void UpdatePrayerButtons()
        {
            PrayerButtonsLayout.Children.Clear();

            foreach (var prayer in _prayers)
            {
                var prayerButton = new Button
                {
                    Text = prayer.Name,
                    WidthRequest = 200,
                    HeightRequest = 50,
                    BackgroundColor = Color.FromArgb("#ff0000"),
                    TextColor = Colors.White,
                    FontAttributes = FontAttributes.Bold,
                    BindingContext = prayer // Setze den Kontext f�r den Button
                };

                var deleteButton = new Button
                {
                    Text = "Delete",
                    WidthRequest = 100,
                    HeightRequest = 50,
                    BackgroundColor = Color.FromArgb("#8B0000"), // Dunkelrot
                    TextColor = Colors.White,
                    FontAttributes = FontAttributes.Bold,
                    CornerRadius = 20,
                    BindingContext = prayer // Setze den Kontext f�r den L�sch-Button
                };
                deleteButton.Clicked += OnDeletePrayerClicked; // Event-Handler f�r L�schen

                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 10,
                    Children = { prayerButton, deleteButton }
                };

                PrayerButtonsLayout.Children.Add(stackLayout); // F�ge das Gebet zur UI hinzu
            }
        }

        // Navigiere zur�ck zur vorherigen Seite
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
