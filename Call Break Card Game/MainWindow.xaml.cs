﻿using System;
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

namespace Call_Break_Card_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Deck deck = new Deck();
        public MainWindow()
        {
            InitializeComponent();


            for (int i = 0; i < deck.Cards.Length; i++)
            {
                lbTest.Items.Add(deck.Cards[i].Name);
            }
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            lbTest.Items.Clear();
            deck.shuffleDeck();
            for (int i = 0; i < deck.Cards.Length; i++)
            {
                lbTest.Items.Add(deck.Cards[i].Name);
            }
        }
    }
}
