using System.Linq;
using System.Text;
using GameHelper.Interfaces.LowLevel;

namespace GameHelper.UserControls
{
    public partial class TextEncoderControl
    {
        private Datagram _datagram;

        public Datagram Datagram
        {
            get => _datagram;
            set
            {
                if (_datagram == value)
                    return;

                _datagram = value;

                _dg.ItemsSource = _datagram != null
                        ? Encoding.GetEncodings()
                            .OrderBy(e => e.Name)
                            .Select(e => new { Value = e.GetEncoding().GetString(_datagram.Data), Encoding = e.Name })
                        : null;
            }
        }

        public TextEncoderControl()
        {
            InitializeComponent();
        }
    }
}
