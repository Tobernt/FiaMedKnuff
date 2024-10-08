using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public sealed partial class HighScore_Popup : UserControl
	{
		public MainMenu MainMenuInstance { get; set; }
		public HighScore_Popup()
		{
			this.InitializeComponent();
		}

		private void Exit_Highscore_Btn(object sender, RoutedEventArgs e)
		{
			if (MainMenuInstance != null && MainMenuInstance.highScorePage != null)
			{
				MainMenuInstance.highScorePage.Visibility = Visibility.Collapsed;
			}
			else
			{
				// Handle null case or log the error
				Debug.WriteLine("MainPageInstance or highScorePage is null");
			}
		}
	}
}
