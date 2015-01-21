using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Vixen.Sys;

namespace Vixen3DPreview.Props
{
    [DataContract]
    class Line: PropBase
    {
        private List<Vector3> _points;
        private int _lightCount ;

        public Line(ElementNode node, Vector3 startPoint)
        {
            EndPoints.Add(startPoint);
        }

        #region "Public Properties"
        [DataMember,
         Category("Settings"),
         Description("The number of unique light points in the Line"),
         DisplayName("Light Count")]
        public virtual int LightCount
        {
            get { return _lightCount; }
            set { _lightCount = value; }
        }
        #endregion

        [DataMember]
        public List<Vector3> EndPoints
        {
            get { return _points ?? (_points = new List<Vector3>()); }
            set { _points = value ?? (_points = new List<Vector3>()); }
        }

        /// <summary>
        /// Layout all of the pixels for the Line
        /// </summary>
        public override void Layout()
        {
            throw new NotImplementedException();
        }

        public override void CompleteAdd(Vector3 clientVectorPosition)
        {
            EndPoints.Add(clientVectorPosition);
        }
    }
}
