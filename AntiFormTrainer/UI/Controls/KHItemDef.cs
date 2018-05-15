using System.Windows.Shapes;
using System.Windows.Media;

namespace AntiFormTrainer.UI
{
    public class KHItemDef : Shape
    {
        public KHItemDef() : base()
        {
            this.Stretch = System.Windows.Media.Stretch.Fill;
        }

        protected override Geometry DefiningGeometry
        {
            get { return GetGeometry(); }
        }

        private Geometry GetGeometry()
        {
            return Geometry.Parse("M0 0 L10 0 L12 5 L10 10 L0 10 Z");
        }
    }
}
