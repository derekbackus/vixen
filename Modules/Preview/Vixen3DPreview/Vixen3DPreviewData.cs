using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen3DPreview.Props;

namespace Vixen3DPreview
{
    [DataContract]
    [KnownType(typeof(Line))]
	public class Vixen3DPreviewData : ModuleDataModelBase
	{
        private List<PropBase> _props = new List<PropBase>(); 

		public override IModuleDataModel Clone()
		{
			var result = new Vixen3DPreviewData
			    {
			        Width = 1024,
			        Height = 800
			    };
			return result;
		}

		[DataMember]
		public int Top { get; set; }

		[DataMember]
		public int Left { get; set; }

		[DataMember]
		public int SetupTop { get; set; }

		[DataMember]
		public int SetupLeft { get; set; }

		[DataMember]
		public int SetupWidth { get; set; }

		[DataMember]
		public int SetupHeight { get; set; }

		[DataMember]
		public int Width { get; set; }

		[DataMember]
		public int Height { get; set; }

		[DataMember]
		public bool SaveLocations { get; set; }

	    [DataMember]
	    public List<PropBase> Props
	    {
            get { return _props ?? (_props = new List<PropBase>()); }
            set { _props = value ?? (_props = new List<PropBase>()); }
        } 
	}
}