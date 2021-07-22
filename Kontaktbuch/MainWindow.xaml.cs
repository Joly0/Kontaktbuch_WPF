using Kontaktbuch.Helper;
using Kontaktbuch.Languages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFCustomMessageBox;

namespace Kontaktbuch
{
    public partial class MainWindow : Window
    {
        private List<PersonModel> people = new();
        private PersonModel currentSelectedPerson;
        public MainWindow()
        {
            CultureInfo default_culture = new(CultureInfo.InstalledUICulture.TwoLetterISOLanguageName);
            CultureResources.ChangeCulture(default_culture);

            InitializeComponent();

            language_changer.SelectionChanged += new SelectionChangedEventHandler(Language_changer_SelectionChanged);
            language_changer.SelectedItem = default_culture;
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            Leeren();
            personGridView.Columns[0].Width = 0;
            PersonListView_SizeChanged(personListView, null);
            LoadPeopleList();
        }

        private void LoadPeopleList()
        {
            people = SqliteDataAccess.LoadPeople();
            personListView.ItemsSource = people;
            Leeren();
        }

        private void AddPersonButton_Click(object sender, EventArgs e)
        {
            PersonAddWindow personAddWindow = new();
            _ = personAddWindow.ShowDialog();

            LoadPeopleList();
        }

        private void PersonListView_SelectionChanged(object sender, EventArgs e)
        {
            foreach (PersonModel p in personListView.SelectedItems)
            {
                currentSelectedPerson = p;
            }
        }

        private void Leeren()
        {
            personListView.SelectedItem = null;

            personListView.UnselectAll();
            currentSelectedPerson = null;
        }

        private void PersonLöschen_Click(object sender, EventArgs e)
        {
            if (currentSelectedPerson != null)
            {
                if (CustomMessageBox.ShowYesNo(Languages.Language.DeleteConfirmation, Languages.Language.ConfirmationTitle, Languages.Language.Yes, Languages.Language.No, null, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    if (personListView != null)
                    {
                        foreach (PersonModel p in personListView.SelectedItems)
                        {
                            SqliteDataAccess.DeletePerson(p);
                        }
                    }
                    LoadPeopleList();
                }
            }
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<PersonModel> filteredPeopleList = new();
            personListView.ItemsSource = people;

            if (!string.IsNullOrEmpty(txtFilter.Text))
            {
                foreach (PersonModel personModel in personListView.Items)
                {
                    if (personModel.VorName.Contains(txtFilter.Text, StringComparison.OrdinalIgnoreCase) ||
                        personModel.NachName.Contains(txtFilter.Text, StringComparison.OrdinalIgnoreCase) ||
                        personModel.Straße.Contains(txtFilter.Text, StringComparison.OrdinalIgnoreCase) ||
                        personModel.Hausnummer.Contains(txtFilter.Text, StringComparison.OrdinalIgnoreCase) ||
                        personModel.Postleitzahl.Contains(txtFilter.Text, StringComparison.OrdinalIgnoreCase) ||
                        personModel.Ort.Contains(txtFilter.Text, StringComparison.OrdinalIgnoreCase) ||
                        personModel.LieblingsLehrer.Contains(txtFilter.Text, StringComparison.OrdinalIgnoreCase) ||
                        personModel.LieblingsDino.Contains(txtFilter.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        filteredPeopleList = FilterPeople(personModel, filteredPeopleList);
                    }
                }
                personListView.ItemsSource = filteredPeopleList;
            }
        }

        private static List<PersonModel> FilterPeople(PersonModel person, List<PersonModel> filteredPeopleList)
        {
            if (!filteredPeopleList.Contains(person))
            {
                filteredPeopleList.Add(person);
            }
            return filteredPeopleList;
        }

        private void PersonListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r != null && r.VisualHit.GetType() == typeof(ScrollViewer))
            {
                GridViewColumnHeader headerClicked = personGridView.Columns[0].Header as GridViewColumnHeader;
                Leeren();
                GridViewSort.ApplySort(personListView.Items, null, personListView, headerClicked);
            }
        }

        private void PersonListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gridView = listView.View as GridView;

            double width = (listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - gridView.Columns[0].Width) / 8;

            foreach (GridViewColumn gridViewColumn in personGridView.Columns)
            {
                if (gridViewColumn.Header.ToString() != "Id")
                {
                    gridViewColumn.Width = width;
                }
            }
        }

        private void PersonListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ScrollViewer))
            {
                if (currentSelectedPerson != null)
                {
                    PersonAddWindow personAddWindow = new(currentSelectedPerson);
                    _ = personAddWindow.ShowDialog();
                    LoadPeopleList();
                }
            }
        }

        private void Language_changer_SelectionChanged(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = language_changer.SelectedItem as CultureInfo;

            CultureInfo selected_culture = language_changer.SelectedItem as CultureInfo;
            //if not current language
            //could check here whether the culture we want to change to is available in order to provide feedback / action
            if (Languages.Language.Culture != null && !Languages.Language.Culture.Equals(selected_culture))
            {
                Debug.WriteLine(string.Format("Change Current Culture to [{0}]", selected_culture));

                //save language in settings
                //Properties.Settings.Default.CultureDefault = selected_culture;
                //Properties.Settings.Default.Save();

                //change resources to new culture
                CultureResources.ChangeCulture(selected_culture);
            }
        }
    }
}
