using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Sys;
using Vixen.Module.Preview;
using Vixen.Sys.Attribute;

namespace Vixen3DPreview
{
	public class Vixen3DPreviewDescriptor : PreviewModuleDescriptorBase
	{
		static Vixen3DPreviewDescriptor()
		{
			ModulePath = "Vixen3DPreview";
		}

        private static Guid _typeId = new Guid("{3C042FBC-7063-4011-8429-5FD99ACE3D32}");

		[ModuleDataPath]
		public static string ModulePath { get; set; }

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Vixen3DPreviewModuleInstance); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (Vixen3DPreviewData); }
		}

		public override string Author
		{
			get { return "Derek Backus"; }
		}

		public override string TypeName
		{
			get { return "Vixen 3D Display Preview"; }
		}

		public override string Description
		{
			get { return "Vixen 3D Display Preview"; }
		}

		public override string Version
		{
			get { return "0.0.1"; }
		}

	}
}