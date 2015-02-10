using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.Preview.Vixen3DPreview.Props;

namespace VixenModules.Preview.Vixen3DPreview
{
    [DataContract]
    [KnownType(typeof(Line))]
	public class Vixen3DPreviewPrivateData
	{
        private List<PropBase> _props = new List<PropBase>();

        public Vixen3DPreviewPrivateData(string instanceId)
        {
            InstanceId = instanceId;
        }

		[DataMember]
		public bool SaveLocations { get; set; }

	    [DataMember]
	    public List<PropBase> Props
	    {
            get { return _props ?? (_props = new List<PropBase>()); }
            set { _props = value ?? (_props = new List<PropBase>()); }
        }

        private string _instanceId = "";
        public string InstanceId
        {
            get
            {
                if (string.IsNullOrEmpty(_instanceId))
                {
                    _instanceId = Guid.NewGuid().ToString();
                }
                return _instanceId;
            }
            set { _instanceId = value; }
        }
	}
}