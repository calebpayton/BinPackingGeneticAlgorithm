using BinPackingWPF.Generator;
using BinPackingWPF.Model;
using BinPackingWPF.ViewModel;
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

namespace BinPackingWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel viewModel { get; set; }
        private readonly PackagesGenerator _packagesGenerator;
        private readonly FleetGenerator _fleetGenerator;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;

            _packagesGenerator = new PackagesGenerator();
            _fleetGenerator = new FleetGenerator();
        }

        private void numPkgsTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                viewModel.NumPackages = Convert.ToInt32(numPkgsTxtBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Input string is not an integer.");
            }
        }

        private void volBinTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                viewModel.BinVolume = Convert.ToDouble(volBinTxtBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Input string is not an integer.");
            }
        }

        private void numGensTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                viewModel.NumGenerations = Convert.ToInt32(numGensTxtBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Input string is not an integer.");
            }
        }

        private void calcButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(volBinTxtBox.Text) && !String.IsNullOrWhiteSpace(numPkgsTxtBox.Text) && !String.IsNullOrWhiteSpace(numGensTxtBlock.Text))
            {
                MainFunction();
            }
            else
            {
                MessageBox.Show("Ensure all feilds are entered.");
            }
        }

        private void MainFunction()
        {
            var packages = _packagesGenerator.GeneratePackages(viewModel.NumPackages, viewModel.BinVolume);
            var ag = new AlgorithmGenerator(packages, viewModel.BinVolume, viewModel.NumGenerations);
            ag.Generate();
        }
    }
}