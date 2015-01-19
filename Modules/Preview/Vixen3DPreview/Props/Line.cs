using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Vixen3DPreview.Props
{
    class Line: PropBase
    {
        private List<Vector3> _points;
 
        [DataMember]
        public List<Vector3> EndPoints
        {
            get { return _points ?? (_points = new List<Vector3>()); }
            set { _points = value ?? (_points = new List<Vector3>()); }
        }
    }
}
