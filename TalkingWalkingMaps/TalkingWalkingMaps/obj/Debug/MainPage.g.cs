﻿

#pragma checksum "C:\Users\abornst\Documents\Visual Studio 2013\Projects\TalkingWalkingMaps\TalkingWalkingMaps\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B97731AF801933F645AB9108CB9EBD29"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TalkingWalkingMaps
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 15 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).KeyDown += this.SearchBox_KeyDown;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 24 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.RouteToHere;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 25 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.RouteBetween;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 26 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ListDirections;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 27 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ToggleVoice;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 28 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.LocationToggle_Click;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 31 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Refresh;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 32 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ZoomOut;
                 #line default
                 #line hidden
                break;
            case 9:
                #line 33 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ZoomIn;
                 #line default
                 #line hidden
                break;
            case 10:
                #line 34 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Search;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


