using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AntiFormTrainer.UI
{
    public class KHButton : Button
    {
        public static DependencyProperty DescriptionProperty;
        public static DependencyProperty IsSelectedProperty;

        static KHButton()
        {
            DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(KHButton));
            IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(KHButton));
        }

        public string Description
        {
            get { return (string)base.GetValue(DescriptionProperty); }
            set { base.SetValue(DescriptionProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)base.GetValue(IsSelectedProperty);}
            set { base.SetValue(IsSelectedProperty, value); }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Communicator.Instance.SelectedButton = this;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Communicator.Instance.SelectedButton = null;
        }
    }
}
