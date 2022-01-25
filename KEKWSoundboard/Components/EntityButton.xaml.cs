using KEKWSoundboard.Database;
using System;
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

namespace KEKWSoundboard.Components
{
    /// <summary>
    /// Interaction logic for EntityButton.xaml
    /// </summary>
    public partial class EntityButton : UserControl
    {
        public DatabaseEntity Entity { get; private set; }

        public EntityButton()
        {
            InitializeComponent();
        }

        public void SetEntity(DatabaseEntity entity)
        {
            // TODO: check the type of entity and display appropriate content
            Entity = entity;

            if (Entity == null)
            {
                mainButton.Style = (Style)Application.Current.Resources["DisabledButton"];
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //// Make a visual brush out of the masking control.
            //VisualBrush brush = new VisualBrush(borderMask);
            //// Set desired opacity.
            //brush.Opacity = 1.0;
            //// Get the offset between the two controls.
            //Point offset = maskedGrid.TranslatePoint(new Point(0, 0), borderMask);
            //// Determine the difference in scaling.
            //Point scale = new Point(borderMask.ActualWidth / maskedGrid.ActualWidth,
            //    borderMask.ActualHeight / maskedGrid.ActualHeight);
            //TransformGroup group = new TransformGroup();
            //// Set the scale of the mask.
            //group.Children.Add(new ScaleTransform(scale.X, scale.Y, 0, 0));
            //// Translate the mask so that it always stays in place.
            //group.Children.Add(new TranslateTransform(-offset.X, -offset.Y));
            //// Rotate it by the reverse of the control, to keep it oriented correctly.
            //// (I am using a ScatterViewItem, which exposes an ActualOrientation property)
            //group.Children.Add(new RotateTransform(0, 0, 0));
            //brush.Transform = group;
            //maskedGrid.OpacityMask = brush;

            base.OnRender(drawingContext);
        }
    }
}
