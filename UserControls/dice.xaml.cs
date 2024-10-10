using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
		public async void ThrowDiceVisual(int diceThrow)
		{
			// Visual dice throw
			Random random = new Random();
			int rndPrevious = 0;
			int rnd = 0;
			for (int i = 1; i < 7; i++)
			{
				while (true)
				{
					// The dice does not show same numbers twice in a row
					rnd = random.Next(1,7);
					if (rnd != rndPrevious)
					{
						rndPrevious = rnd;
						break;
					}
				}
				// Delay so that the user can see that the dice is being randomized
				await Task.Delay(100);
				ShowDiceNum(rnd);
			}
			// Shows the final dice result
			ShowDiceNum(diceThrow);
		}
		public void ShowDiceNum(int diceThrow)
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
