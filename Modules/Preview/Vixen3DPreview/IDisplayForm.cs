using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen3DPreview {
	public interface IDisplayForm : IDisposable {
		Vixen3DPreviewData Data { get; set; }
		void Setup();
		void Close();
		void UpdatePreview();
	}
}
