using System.Runtime.Serialization;
using Vixen.Module;

namespace Vixen3DPreview
{
	[DataContract]
	public class Vixen3DPreviewData : ModuleDataModelBase
	{
		public Vixen3DPreviewData()
		{
		}

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
	}
}