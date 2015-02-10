using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.Preview.Vixen3DPreview
{
	public interface IDisplayForm : IDisposable {
		Vixen3DPreviewPrivateData Data { get; set; }
		void Setup();
		void Close();
		void UpdatePreview();
	}
}
