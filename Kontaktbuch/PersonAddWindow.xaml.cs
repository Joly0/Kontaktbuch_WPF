using System;
using System.Windows;

namespace Kontaktbuch
{
    public partial class PersonAddWindow : Window
    {
        private readonly int selectedID = -1;
        public PersonAddWindow()
        {
            InitializeComponent();
        }
        public PersonAddWindow(PersonModel p)
        {
            InitializeComponent();
            addPersonButton.Content = Languages.Language.Change;
            if (p != null)
            {
                selectedID = p.Id;
                vorNameText.Text = p.VorName;
                nachNameText.Text = p.NachName;
                straßeText.Text = p.Straße;
                hausnummerText.Text = p.Hausnummer;
                postleitzahlText.Text = p.Postleitzahl;
                ortText.Text = p.Ort;
                lieblingsLehrerText.Text = p.LieblingsLehrer;
                lieblingsDinoText.Text = p.LieblingsDino;
            }
        }
        private void addPersonButton_Click(object sender, EventArgs e)
        {
            PersonModel p = new()
            {
                Id = selectedID,
                VorName = vorNameText.Text,
                NachName = nachNameText.Text,
                Straße = straßeText.Text,
                Hausnummer = hausnummerText.Text,
                Postleitzahl = postleitzahlText.Text,
                Ort = ortText.Text,
                LieblingsLehrer = lieblingsLehrerText.Text,
                LieblingsDino = lieblingsDinoText.Text
            };

            SqliteDataAccess.SavePerson(p);

            Close();
        }
    }
}
