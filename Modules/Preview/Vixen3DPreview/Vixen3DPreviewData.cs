using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.Preview.Vixen3DPreview.Props;

namespace VixenModules.Preview.Vixen3DPreview
{
    [DataContract]
    [KnownType(typeof(Line))]
	public class Vixen3DPreviewData : ModuleDataModelBase
	{
        //private List<PropBase> _props = new List<PropBase>(); 
        private Vixen3DPreviewPrivateData _privateData = null;

		public override IModuleDataModel Clone()
		{
			var result = new Vixen3DPreviewData
			    {
                    //Width = 1024,
                    //Height = 800
                    _dataInstanceId = _dataInstanceId
			    };
			return result;
		}

        [DataMember] 
        private Guid _dataInstanceId;
        public Guid DataInstanceId
        {
            get
            {
                if (_dataInstanceId == Guid.Empty)
                {
                    Console.WriteLine("_dataInstanceId == Guid.Empty, Assigning New Guid");
                    _dataInstanceId = Guid.NewGuid();
                }
                return _dataInstanceId;
            }
            set
            {
                if (value == Guid.Empty)
                {
                    Console.WriteLine("_dataInstanceId value == Guid.Empty, Assigning New Guid");
                    _dataInstanceId = Guid.NewGuid();
                }
                else
                {
                    Console.WriteLine("Returning existing _dataInstanceId");
                    _dataInstanceId = value;
                }
            }
        }

        public Vixen3DPreviewPrivateData PrivateData
        {
            get
            {
                if (_privateData == null)
                {
                    Console.WriteLine("Reading DataInstanceId: " + DataInstanceId.ToString());
                    _privateData = PreviewUtils.ReadData(DataInstanceId.ToString());
                    return _privateData;
                }
                else
                {
                    Console.WriteLine("Existing DataInstanceId, Alreadey Loaded Not Read: " + DataInstanceId.ToString());
                    return _privateData;
                }
            }
        }

        //[DataMember]
        //public bool SaveLocations { get; set; }

        //[DataMember]
        //public List<PropBase> Props
        //{
        //    get { return _props ?? (_props = new List<PropBase>()); }
        //    set { _props = value ?? (_props = new List<PropBase>()); }
        //} 
	}
}