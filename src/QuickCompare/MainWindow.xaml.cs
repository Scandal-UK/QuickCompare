﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Options;
using QuickCompareModel;

namespace QuickCompare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, (string, string)> definitionDifferences;

        public MainWindow()
        {
            InitializeComponent();

            ConnectionString1.Text = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Database1;Integrated Security=True";
            ConnectionString2.Text = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Database2;Integrated Security=True";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var builder = new DifferenceBuilder(GetOptions());
            builder.BuildDifferences();

            definitionDifferences = builder.DefinitionDifferences;

            OutputTextBox.AppendText(builder.Differences.ToString());
            ComboBox1.ItemsSource = definitionDifferences.Keys;
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var definitionDifference = definitionDifferences[e.AddedItems[0].ToString()];
            DiffViewer1.OldText = definitionDifference.Item1;
            DiffViewer1.NewText = definitionDifference.Item2;
        }

        private IOptions<QuickCompareOptions> GetOptions()
        {
            var settings = new QuickCompareOptions
            {
                ConnectionString1 = ConnectionString1.Text,
                ConnectionString2 = ConnectionString2.Text,
            };

            return Options.Create(settings);
        }
    }
}
