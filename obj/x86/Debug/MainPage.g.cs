﻿#pragma checksum "E:\script\FiaMedKnuff\FiaMedKnuff\FiaMedKnuff\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "65F66E4B1FC67E4A1A694059527A3FF8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FiaMedKnuff
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // MainPage.xaml line 10
                {
                    this.GameBoard = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 3: // MainPage.xaml line 157
                {
                    global::Windows.UI.Xaml.Controls.Button element3 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element3).Click += this.RollDice_Click;
                }
                break;
            case 4: // MainPage.xaml line 158
                {
                    this.DiceRollResult = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 5: // MainPage.xaml line 146
                {
                    this.Player1Token = (global::Windows.UI.Xaml.Shapes.Ellipse)(target);
                }
                break;
            case 6: // MainPage.xaml line 148
                {
                    this.Player2Token = (global::Windows.UI.Xaml.Shapes.Ellipse)(target);
                }
                break;
            case 7: // MainPage.xaml line 150
                {
                    this.Player3Token = (global::Windows.UI.Xaml.Shapes.Ellipse)(target);
                }
                break;
            case 8: // MainPage.xaml line 152
                {
                    this.Player4Token = (global::Windows.UI.Xaml.Shapes.Ellipse)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

