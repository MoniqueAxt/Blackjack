using CardGame_BLL;
using CardGame_EL.Models;
using CardGame_EL.Players;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BlackJack_PL
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private DatabaseManager _gameManager;
        private Game _selectedGame;

        public Window2(List<Game> games, DatabaseManager gameManager)
        {
            InitializeComponent();
            _gameManager = gameManager;
            lbxDbItems.ItemsSource = DatabaseManager.GetAllGamesFromDb();
        }

        /// <summary>
        /// Delete a game from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGame == null || lbxDbItems.SelectedItems.Count < 0)
            {
                MessageBox.Show("No game selected", "Error");
                return;
            }

            DatabaseManager.Delete(_selectedGame);
            ResetSelectedGameInfo();
            MessageBox.Show("Game deleted", "Success");
        }

        /// <summary>
        /// Edit a game from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGame == null || lbxDbItems.SelectedItems.Count < 0)
            {
                MessageBox.Show("No game selected", "Error");
                return;
            }

            _selectedGame.GameName = tbxGameName.Text;
            DatabaseManager.Update(_selectedGame);
            MessageBox.Show("Game updated", "Success");
        }

        /// <summary>
        /// Load a specific game's information into the components
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItcDbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxDbItems.SelectedItems.Count > 0 && lbxDbItems.SelectedItem is Game game)
            {
                _selectedGame = game;

                tbxGameName.Text = game.GameName;
                tbxNrDecks.Text = game.GameSettings?.NumberDecks.ToString();
                tbxNrHumans.Text = game.GameSettings?.NumberHumanPlayers.ToString();
                tbxDealerWon.Text = game.Dealer.Status == CardGame_EL.Enums.Status.Won ? "Yes" : "No";

                lblPlayers.Content = $"Players ({game.Players.Count})";
                lbxPlayers.Items.Clear();
                foreach (Player player in game.Players)
                {
                    lbxPlayers.Items.Add(player.Name);
                }
            }
        }

        /// <summary>
        /// Filter games based on given search criteria
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            // no search filter criteria specified
            if (cbxNrDecks.SelectedIndex < 0 && cbxNrPlayers.SelectedIndex < 0 && tbxGameName.Text == "")
            {
                return;
            }

            int nrDecks = -1;
            int nrPlayers = -1;

            // filter by number of decks used in the game
            if (cbxNrDecks.SelectedIndex > -1)
            {
                string temp = cbxNrDecks.SelectedValue.ToString();
                nrDecks = int.Parse(temp);
            }

            // filter by number of players in the game
            if (cbxNrPlayers.SelectedIndex > -1)
            {
                string temp = cbxNrPlayers.SelectedValue.ToString();
                nrPlayers = int.Parse(temp);
            }

            // filter by the name of the game
            string gameName = tbxFilterGameName.Text;

            List<Game> filteredGames = _gameManager.Filter(nrDecks, nrPlayers, gameName);
            lbxDbItems.ItemsSource = filteredGames;
        }

        /// <summary>
        /// Reset the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetSearchFilters();
            ResetSelectedGameInfo();
        }

        /// <summary>
        /// Clear the components containing info on the selected game
        /// </summary>
        private void ResetSelectedGameInfo()
        {
            tbxGameName.Text = "-";
            tbxNrDecks.Text = "-";
            tbxNrHumans.Text = "-";
            tbxDealerWon.Text = "-";
            lbxPlayers.Items.Clear();
            lbxPlayers.ItemsSource = null;
        }

        /// <summary>
        /// Clear the components containing search filter info
        /// </summary>
        private void ResetSearchFilters()
        {
            cbxNrDecks.SelectedIndex = -1;
            cbxNrPlayers.SelectedIndex = -1;
            tbxFilterGameName.Clear();
            lbxDbItems.ItemsSource = DatabaseManager.GetAllGamesFromDb();
        }
    }
}
