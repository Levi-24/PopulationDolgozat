using Population;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace Population
{
    public partial class MainWindow : Window
    {
        private readonly List<Allampolgar> lakossag;
        const int feladatokSzama = 5;

        public MainWindow()
        {
            InitializeComponent();
            lakossag = new List<Allampolgar>();

            using var sr = new StreamReader(@"..\..\..\SRC\population.txt");
            _ = sr.ReadLine();

            while (!sr.EndOfStream)
            {
                lakossag.Add(new Allampolgar(sr.ReadLine()));
            }

            for (int i = 1; i <= feladatokSzama; i++)
            {
                feladatComboBox.Items.Add($"{i}.");
            }

            DataContext = this;
            MegoldasTeljes.ItemsSource = lakossag;
        }

        private void FeladatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MegoldasMondatos.Content = null;
            MegoldasLista.Items.Clear();
            MegoldasTeljes.ItemsSource = null;

            var methodName = $"Feladat{feladatComboBox.SelectedIndex + 1}";
            var method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(this, null);
        }

        private void Feladat1()
        {
           var yorkshireNok = lakossag.Where(x => x.Nem == "nő" && x.Megye == "Yorkshire").ToList();
           foreach (var szemely in yorkshireNok)
                MegoldasLista.Items.Add(szemely.Megjelenit(false));
        }

        private void Feladat2()
        {
            MegoldasTeljes.ItemsSource = lakossag.OrderBy(x => x.SzuletesiDatum).Take(1);
        }

        private void Feladat3()
        {
            MegoldasTeljes.ItemsSource = lakossag.Where(x => !x.Egeszsegbiztositas);
            //MegoldasTeljes.ItemsSource = lakossag.Where(x => x.EredetiEgeszsegbiztositas == "nincs");
        }

        private void Feladat4()
        {
           var legmagasabbAtlag = lakossag.GroupBy(p => p.Megye).Select(g => new
          {
              Megye = g.Key,
              AtlagosBevetel = g.Average(p => p.HaviBruttoJovedelem * 12)
          })
          .OrderByDescending(g => g.AtlagosBevetel).FirstOrDefault();

            MegoldasMondatos.Content = $"A legmagasabb átlagkeresetű megye: {legmagasabbAtlag.Megye}, az átlag éves jövedelem: {legmagasabbAtlag.AtlagosBevetel:0.00}font";
        }

        private void Feladat5()
        {
            var noiBevetelMegyenkent = lakossag.GroupBy(p => p.Megye).Select(g => new
            {
                Megye = g.Key,
                LegmagasabbNoiBevetel = g.Where(x => x.Nem == "nő").Max(p => p.HaviBruttoJovedelem * 12)
            });

            MegoldasLista.ItemsSource = noiBevetelMegyenkent;
        }
    }
}
