using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using OpenTK;
using Vixen.Sys;
using Vixen.Intent;
using Color = System.Drawing.Color;
using OpenTK.Graphics;

namespace VixenModules.Preview.Vixen3DPreview.Props
{
    [DataContract]
    public class PreviewPixel
    {
        private ElementNode _node;
        private Guid _nodeId = Guid.Empty;
        private Color4[] _colors;
        private int _size = 1;

        public PreviewPixel(float x, float y, float z)
        {
            Location = new Vector3(x, y, z);
        }

        public PreviewPixel(Vector3 vector)
        {
            Location = vector;
        }

        [DataMember]
        public Vector3 Location { get; set; }

        public int Size
        {
            get
            {
                if (_size < 1) _size = 1;
                return _size;
            }
            set { _size = value; }
        }

        [DataMember]
        public Guid NodeId
        {
            get
            {
                return _nodeId;
            }
            set
            {
                _nodeId = value;
                _node = VixenSystem.Nodes.GetElementNode(_nodeId);
            }
        }

        public bool DiscreteColored
        {
            get
            {
                return _node != null && Property.Color.ColorModule.isElementNodeDiscreteColored(_node);
            }
        }

        public ElementNode Node
        {
            get { return _node; }
            set
            {
                _node = value;
                NodeId = _node.Id;
            }
        }

        public Color4[] Colors
        {
            get
            {
                if (_colors == null)
                {
                    Colors = new Color4[1];
                }
                return _colors;
            }
            set { _colors = value; }
        }


        public void ClearColors()

        {
            for (var i = 0; i < Colors.Length; i++)
            {
                Colors[i].A = 0;
                Colors[i].R = 0;
                Colors[i].G = 0;
                Colors[i].B = 0;
            }
        }

        public void SetColorsFromIntents(IIntentStates states)
        {
            ClearColors();
            //Console.WriteLine("Setting Colors");
            if (DiscreteColored)
            {
                // All this to try to save some GC troubles. Does it work?
                var colors = IntentHelpers.GetAlphaAffectedDiscreteColorsForIntents(states);
                if (colors.Count() != Colors.Length)
                {
                    Colors = new Color4[colors.Count()];
                }
                var i = 0;
                foreach (var color in colors)
                {
                    Colors[i].A = color.A;
                    Colors[i].R = color.R;
                    Colors[i].G = color.G;
                    Colors[i].B = color.B;
                }
            }
            else
            {
                var color = IntentHelpers.GetAlphaRGBMaxColorForIntents(states);
                if (Colors.Length != 1)
                {
                    Colors = new Color4[1];
                }
                Colors[0].A = color.A;
                Colors[0].R = color.R;
                Colors[0].G = color.G;
                Colors[0].B = color.B;
            }
        }
    }
}
