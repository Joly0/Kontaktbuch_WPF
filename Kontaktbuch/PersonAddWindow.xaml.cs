using System;
using System.Windows;

namespace Kontaktbuch
{
    public partial class PersonAddWindow : Window
    {
        private int selectedID = -1;
        public PersonAddWindow()
        {
            InitializeComponent();
        }
        public PersonAddWindow(PersonModel p)
        {
            InitializeComponent();
            addPersonButton.Content = "Ändern";
            if (p != null)
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
        private void addPersonButton_Click(object sender, EventArgs e)
        {
            PersonModel p = new PersonModel();

            p.Id = selectedID;
            p.VorName = vorNameText.Text;
            p.NachName = nachNameText.Text;
            p.Straße = straßeText.Text;
            p.Hausnummer = hausnummerText.Text;
            p.Postleitzahl = postleitzahlText.Text;
            p.Ort = ortText.Text;
            p.LieblingsLehrer = lieblingsLehrerText.Text;
            p.LieblingsDino = lieblingsDinoText.Text;

            SqliteDataAccess.SavePerson(p);

            Close();
        }
    }
}
