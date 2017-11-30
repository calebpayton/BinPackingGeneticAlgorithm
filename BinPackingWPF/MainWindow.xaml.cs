using BinPackingWPF.Algorithm;
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
using System.Windows.Media.Animation;
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
        public IList<double> FitnessData { get; set; }
        private Random random;
        private int populationSize = 100;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;

            graph.DataContext = new List<KeyValuePair<string, double>>();

            _packagesGenerator = new PackagesGenerator();
            _fleetGenerator = new FleetGenerator();
            random = new Random();
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
                PerformAlgorithm();
            }
            else
            {
                MessageBox.Show("Ensure all feilds are entered.");
            }
        }

        private async void PerformAlgorithm()
        {
            var packages = _packagesGenerator.GeneratePackages(viewModel.NumPackages, viewModel.BinVolume, random);
            
            var ga = new GeneticAlgorithm(packages, populationSize, random, viewModel.BinVolume);

            if (progressBar.Value > 0)
                progressBar.Value = 0;

            progressBar.Maximum = viewModel.NumGenerations;
            graph.DataContext = null;

            if (viewModel.NumGenerations > 1)
            {
                var progress = new Progress<int>(value => progressBar.Value = value);
                await Task.Run(() =>
                {
                    do
                    {
                        if (ga.Generation == viewModel.NumGenerations - 1)
                            ga.FinalGeneration = true;

                        var task = Task.Factory.StartNew(() => ga.NewGeneration(crossoverNewDNA: true));
                        task.Wait();
                        ((IProgress<int>)progress).Report(ga.Generation);
                    }
                    while (ga.Generation < viewModel.NumGenerations);
                });
            }            

            FitnessData = ga.FitnessOverTime;
            FitnessData.Add(ga.WoCFitness);
            PlotGraph();
        }

        private void PlotGraph()
        {
            var valueList = new List<KeyValuePair<string, double>>();
            var addedItems = new List<double>();
            for (int i = 0; i < FitnessData.Count; i++)
            {
                if (!addedItems.Contains(FitnessData[i]) && i != FitnessData.Count - 1)
                {
                    valueList.Add(new KeyValuePair<string, double>((i + 1).ToString(), FitnessData[i] * 100));
                    addedItems.Add(FitnessData[i]);
                }
                else if (i == FitnessData.Count - 2)
                {
                    valueList.Add(new KeyValuePair<string, double>((i + 1).ToString(), FitnessData[i] * 100));
                }
                else if (i == FitnessData.Count - 1)
                {
                    valueList.Add(new KeyValuePair<string, double>("WoC", FitnessData[i] * 100));
                }
            }

            graph.DataContext = valueList;
        }
    }
}