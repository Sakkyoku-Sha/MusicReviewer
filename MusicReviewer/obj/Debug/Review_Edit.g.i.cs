#pragma checksum "..\..\Review_Edit.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3DCD1E1F7A8BD15DCD3A6407D237BA9D48D75052"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MusicReviewer;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MusicReviewer
{


    /// <summary>
    /// Review_Edit
    /// </summary>
    public partial class Review_Edit : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MusicReviewer;component/review_edit.xaml", System.UriKind.Relative);

#line 1 "..\..\Review_Edit.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            this._contentLoaded = true;
        }

        internal System.Windows.Controls.Label TitleEntryLabel;
        internal System.Windows.Controls.TextBox TitleBox;
        internal System.Windows.Controls.Label ReleaseDataLabel;
        internal System.Windows.Controls.ComboBox YearBox;
        internal System.Windows.Controls.Label AlbumLabel;
        internal System.Windows.Controls.TextBox AlbumName;
        internal System.Windows.Controls.Label DropLabel;
        internal System.Windows.Controls.Border DropBorder;
        internal System.Windows.Controls.Label FileNameLabel;
        internal System.Windows.Controls.Label Artist_Name;
        internal System.Windows.Controls.TextBox Artist_NameBox;
        internal System.Windows.Controls.Label Score;
        internal System.Windows.Controls.TextBox ScoreBox;
        internal System.Windows.Controls.TextBox ReviewBox;
        internal System.Windows.Controls.TextBox SearchBox;
        internal System.Windows.Controls.Border TagBorder;
        internal System.Windows.Controls.RichTextBox LanguagesBox;
        internal System.Windows.Controls.RichTextBox GenresBox;
        internal System.Windows.Controls.RichTextBox InstrumentsBox;
        internal System.Windows.Controls.Label SearchError;
        internal System.Windows.Controls.Button SelectTagsButton;
    }
}

