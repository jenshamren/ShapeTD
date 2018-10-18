using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShapeTD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        private Dictionary<string, FrameworkElement> gameModels;
        private GameController game;
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
            game = new GameController(playArea, this, gameModels);
        }
        private void Initialize()
        {
            gameModels = new Dictionary<string, FrameworkElement>();
            UIElementCollection collection = playArea.Children;
            for(int i = collection.Count -1; i >= 0; i--)
            {
                FrameworkElement obj = collection[i] as FrameworkElement;
                gameModels.Add((string)obj.Tag, obj);
                playArea.Children.Remove(obj);
            }
        }
      }
    }
