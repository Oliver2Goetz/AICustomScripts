using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CATHODE;
using CATHODE.Scripting;

namespace AIFirstCustomScript {

    class AIFirstCustomScript {

        public static void Main() {
            Commands commands = new Commands("COMMANDS.PAK");
            Composite composite = commands.AddComposite("My coolScript", true);

            FunctionEntity checkpoint = composite.AddFunction(FunctionType.Checkpoint);
            checkpoint.AddParameter("is_first_checkpoint", new cBool(true));
            checkpoint.AddParameter("section", new cString("Entry"));

            FunctionEntity objective = composite.AddFunction(FunctionType.SetPrimaryObjective);
            objective.AddParameter("title", new cString("Where Steve?"));
            objective.AddParameter("additional_info", new cString("Go and find Steve!"));

            checkpoint.AddParameterLink("finished_loading", objective, "trigger");

            commands.Save();
        }
    }
}
