using CardGame_EL.Models;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace BlackJack_CardGame
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private readonly GameSettings _gameSettings;
        private readonly NotifyStartEvent _notifyStartEvent;

        public Window1(NotifyStartEvent notifyStartEvent)
        {
            InitializeComponent();
            _gameSettings = new GameSettings(numberPlayersToCreate: 1, numberDecks: 1);
            _notifyStartEvent = notifyStartEvent;
        }

        /// <summary>
        /// Sends the input as settings for the game back to the main form via delegate. If invalid input is found,
        /// default values are used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStartGame_Click(object sender, RoutedEventArgs e)
        {
            _gameSettings.NumberDecks = int.TryParse(TbxNrDecks.Text, out int nrDecks) ? nrDecks : 1;
            _gameSettings.NumberPlayersToCreate = int.TryParse(TbxNrPlayers.Text, out int nrPlayers) ? nrPlayers : 2;
            _gameSettings.NumberHumanPlayers = int.TryParse(TbxNrHumans.Text, out int nrHumans) ? nrHumans : 1;

            _notifyStartEvent.Invoke(_gameSettings);
            this.Close();
        }

        /// <summary>
        /// Disallow non-numeric input to be typed in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^1-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
