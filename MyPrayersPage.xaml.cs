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

        // Wenn ein Gebet hinzugefügt wird
        private async void OnPrayerAdded(object sender, Prayer prayer)
        {
            _prayers.Add(prayer); // Gebet zur Liste hinzufügen
            await _firebaseService.AddPrayerAsync(prayer); // Gebet in Firebase speichern
            UpdatePrayerButtons(); // UI aktualisieren
        }

        // Event-Handler für das Löschen eines Gebets
        private async void OnDeletePrayerClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var prayer = (Prayer)button.BindingContext;

            if (prayer != null)
            {
                bool isConfirmed = await DisplayAlert("Bestätigung", "Möchten Sie dieses Gebet wirklich löschen?", "Ja", "Nein");
                if (isConfirmed)
                {
                    try
                    {
                        await _firebaseService.DeletePrayerAsync(prayer); // Gebet aus Firebase löschen
                        _prayers.Remove(prayer); // Aus der lokalen Liste entfernen
                        UpdatePrayerButtons(); // UI aktualisieren
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Fehler", "Das Gebet konnte nicht gelöscht werden: " + ex.Message, "OK");
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
                    BindingContext = prayer // Setze den Kontext für den Button
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
                    BindingContext = prayer // Setze den Kontext für den Lösch-Button
                };
                deleteButton.Clicked += OnDeletePrayerClicked; // Event-Handler für Löschen

                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 10,
                    Children = { prayerButton, deleteButton }
                };

                PrayerButtonsLayout.Children.Add(stackLayout); // Füge das Gebet zur UI hinzu
            }
        }

        // Navigiere zurück zur vorherigen Seite
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
