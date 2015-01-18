using System;
using System.Windows.Forms;
using System.Diagnostics;
using Vixen.Execution.Context;
using Vixen.Module.Preview;
using Vixen.Sys;
using Vixen.Sys.Instrumentation;

namespace Vixen3DPreview
{
	public partial class Vixen3DPreviewModuleInstance : FormPreviewModuleInstanceBase
	{
		private SetupForm _setupForm;
		private IDisplayForm _displayForm;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private MillisecondsValue _updateTimeValue = new MillisecondsValue("Update time for 3D preview");

		public Vixen3DPreviewModuleInstance()
		{
			VixenSystem.Instrumentation.AddValue(_updateTimeValue);
		}

		public override void Stop()
		{
			base.Stop();
		}

		public override void Resume()
		{
			base.Resume();
		}

		public override void Pause()
		{
			base.Pause();
		}

		public override bool IsRunning
		{
			get { return base.IsRunning; }
		}

		public override bool HasSetup
		{
			get { return base.HasSetup; }
		}

		public override Vixen.Module.IModuleDataModel ModuleData
		{
			get
			{
				if (base.ModuleData == null) {
					base.ModuleData = new Vixen3DPreviewData();
					Logging.Warn("Vixen3DPreview: access of null ModuleData. Creating new one. (Thread ID: " +
					                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				}
				return base.ModuleData;
			}
			set
			{
				base.ModuleData = value;
			}
		}

		protected override Form Initialize()
		{
			Execution.NodesChanged += ExecutionNodesChanged;
			VixenSystem.Contexts.ContextCreated += ProgramContextCreated;
			VixenSystem.Contexts.ContextReleased += ProgramContextReleased;
 
			SetupPreviewForm();

			return (Form)_displayForm;
		}

		private object formLock = new object();
		private void SetupPreviewForm()
		{
			lock (formLock) {
                _displayForm = new ViewerForm(GetDataModel());
                _displayForm.Setup();
			}
		}

		private Vixen3DPreviewData GetDataModel()
		{
			return ModuleData as Vixen3DPreviewData;
		}

		public override void Start()
		{
			//System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;
			base.Start();
		}

		public override bool Setup()
		{
			_setupForm = new SetupForm(GetDataModel());

			_setupForm.ShowDialog();

			if (_displayForm != null)
			{
                _displayForm.Data = GetDataModel();
                _displayForm.Setup();
			}

			return base.Setup();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
                if (_displayForm != null)
                    _displayForm.Close();
				VixenSystem.Contexts.ContextCreated -= ProgramContextCreated;
				VixenSystem.Contexts.ContextReleased -= ProgramContextReleased;	
			}
			
			base.Dispose(disposing);
		}

		private void ExecutionNodesChanged(object sender, EventArgs e)
		{
		}

		private void ProgramContextCreated(object sender, ContextEventArgs contextEventArgs)
		{
			var programContext = contextEventArgs.Context as IProgramContext;
			//
			// This is always null... why does this event get called?
			//
			if (programContext != null) {
				//_programContexts.Add(programContext);
				programContext.ProgramStarted += ProgramContextProgramStarted;
				programContext.ProgramEnded += ProgramContextProgramEnded;
				programContext.SequenceStarted += context_SequenceStarted;
				programContext.SequenceEnded += context_SequenceEnded;
			}
		}

		private void ProgramContextProgramEnded(object sender, ProgramEventArgs e)
		{
			Stop();
		}

		private void ProgramContextProgramStarted(object sender, ProgramEventArgs e)
		{
			Start();
		}

		protected void context_SequenceStarted(object sender, SequenceStartedEventArgs e)
		{
		}

		protected void context_SequenceEnded(object sender, SequenceEventArgs e)
		{
		}

		private void ProgramContextReleased(object sender, ContextEventArgs contextEventArgs)
		{
			var programContext = contextEventArgs.Context as IProgramContext;
			if (programContext != null) {
				programContext.ProgramStarted -= ProgramContextProgramStarted;
				programContext.ProgramEnded -= ProgramContextProgramEnded;
				programContext.SequenceStarted -= context_SequenceStarted;
				programContext.SequenceEnded -= context_SequenceEnded;
			}
		}

		protected override void Update()
		{
			var sw = Stopwatch.StartNew();
			try 
            {
                _displayForm.UpdatePreview();
			}
			catch (Exception e) {
				Logging.Error("Exception in preview update {0} - {1}", e.Message, e.StackTrace);
			}
			_updateTimeValue.Set(sw.ElapsedMilliseconds);
		}
	}
}