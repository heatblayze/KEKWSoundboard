using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace KEKWSoundboard.Components
{
    public class ImageToggle : ToggleButton
    {
        public static readonly DependencyProperty NormalBrushProperty = DependencyProperty.Register(
            "NormalBrush", typeof(Brush), typeof(ImageToggle)
        );

        public static readonly DependencyProperty CheckedBrushProperty = DependencyProperty.Register(
            "CheckedBrush", typeof(Brush), typeof(ImageToggle)
        );

        public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(
            "HighlightBrush", typeof(Brush), typeof(ImageToggle)
        );

        public static readonly DependencyProperty PressBrushProperty = DependencyProperty.Register(
            "PressBrush", typeof(Brush), typeof(ImageToggle)
        );

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(ImageSource), typeof(ImageToggle)
        );

        public Brush NormalBrush
        {
            get => (Brush)GetValue(NormalBrushProperty);
            set => SetValue(NormalBrushProperty, value);
        }

        public Brush CheckedBrush
        {
            get => (Brush)GetValue(CheckedBrushProperty);
            set => SetValue(CheckedBrushProperty, value);
        }

        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        public Brush PressBrush
        {
            get => (Brush)GetValue(PressBrushProperty);
            set => SetValue(PressBrushProperty, value);
        }

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);
            Background = CheckedBrush;
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);
            Background = NormalBrush;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Background = HighlightBrush;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Background = (IsChecked.HasValue && IsChecked.Value) ? CheckedBrush : NormalBrush;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Background = PressBrush;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (IsMouseOver)
                Background = HighlightBrush;
            else
                Background = (IsChecked.HasValue && IsChecked.Value) ? CheckedBrush : NormalBrush;
        }
    }
}
