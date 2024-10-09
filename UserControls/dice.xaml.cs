using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FiaMedKnuff.UserControls
{
	public sealed partial class Dice : UserControl
	{
		public Dice()
		{
			this.InitializeComponent();
		}

		public void DiceVisualEffect(int diceThrow)
		{
			if (diceThrow == 1) DiceThrowOne();
			if (diceThrow == 2) DiceThrowTwo();
			if (diceThrow == 3) DiceThrowThree();
			if (diceThrow == 4) DiceThrowFour();
			if (diceThrow == 5) DiceThrowFive();
			if (diceThrow == 6) DiceThrowSix();
		}

		private void DiceThrowOne()
		{
			dotOne.Visibility = Visibility.Collapsed;
			dotTwo.Visibility = Visibility.Collapsed;
			dotThree.Visibility = Visibility.Collapsed;
			dotFour.Visibility = Visibility.Visible;
			dotFive.Visibility = Visibility.Collapsed;
			dotSix.Visibility = Visibility.Collapsed;
			dotSeven.Visibility = Visibility.Collapsed;
		}
		private void DiceThrowTwo()
		{
			dotOne.Visibility = Visibility.Collapsed;
			dotTwo.Visibility = Visibility.Visible;
			dotThree.Visibility = Visibility.Collapsed;
			dotFour.Visibility = Visibility.Collapsed;
			dotFive.Visibility = Visibility.Collapsed;
			dotSix.Visibility = Visibility.Visible;
			dotSeven.Visibility = Visibility.Collapsed;
		}
		private void DiceThrowThree()
		{
			dotOne.Visibility = Visibility.Visible;
			dotTwo.Visibility = Visibility.Collapsed;
			dotThree.Visibility = Visibility.Collapsed;
			dotFour.Visibility = Visibility.Visible;
			dotFive.Visibility = Visibility.Collapsed;
			dotSix.Visibility = Visibility.Collapsed;
			dotSeven.Visibility = Visibility.Visible;
		}
		private void DiceThrowFour()
		{
			dotOne.Visibility = Visibility.Visible;
			dotTwo.Visibility = Visibility.Visible;
			dotThree.Visibility = Visibility.Collapsed;
			dotFour.Visibility = Visibility.Collapsed;
			dotFive.Visibility = Visibility.Collapsed;
			dotSix.Visibility = Visibility.Visible;
			dotSeven.Visibility = Visibility.Visible;
		}
		private void DiceThrowFive()
		{
			dotOne.Visibility = Visibility.Visible;
			dotTwo.Visibility = Visibility.Visible;
			dotThree.Visibility = Visibility.Collapsed;
			dotFour.Visibility = Visibility.Visible;
			dotFive.Visibility = Visibility.Collapsed;
			dotSix.Visibility = Visibility.Visible;
			dotSeven.Visibility = Visibility.Visible;
		}
		private void DiceThrowSix()
		{
			dotOne.Visibility = Visibility.Visible;
			dotTwo.Visibility = Visibility.Visible;
			dotThree.Visibility = Visibility.Visible;
			dotFour.Visibility = Visibility.Collapsed;
			dotFive.Visibility = Visibility.Visible;
			dotSix.Visibility = Visibility.Visible;
			dotSeven.Visibility = Visibility.Visible;
		}
	}
}
