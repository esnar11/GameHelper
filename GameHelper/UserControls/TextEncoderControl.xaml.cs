using System.Linq;
using System.Text;

namespace GameHelper.UserControls
{
    public partial class TextEncoderControl
    {
        private byte[] _binaryData;

        public byte[] BinaryData
        {
            get => _binaryData;
            set
            {
                if (_binaryData == value)
                    return;

                _binaryData = value;

                _dg.ItemsSource = _binaryData != null
                        ? Encoding.GetEncodings()
                            .OrderBy(e => e.Name)
                            .Select(e => new { Value = e.GetEncoding().GetString(_binaryData), Encoding = e.Name })
                        : null;
            }
        }

        public TextEncoderControl()
        {
            InitializeComponent();
        }
    }
}
