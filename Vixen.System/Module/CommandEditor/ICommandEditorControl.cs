﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.CommandEditor {
    public interface ICommandEditorControl {
		object[] CommandParameterValues { get; }
    }
}
