using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CATHODE;
using CATHODE.Scripting;

namespace AIFirstCustomScript {

    class AIFirstCustomScript {

        private static string rootDirectory = "F:/SteamLibrary/steamapps/common/Alien Isolation/";

        /*
         * Using an existing COMMANDS.PAK and modify it (spawn player + add objective)
         */
        public static void Main() {
            Commands commands = new Commands(rootDirectory + "DATA/ENV/PRODUCTION/ENG_ALIEN_NEST/WORLD/COMMANDS.PAK");

            Composite composite = commands.EntryPoints[0];
            composite.functions.Clear();

            FunctionEntity checkpoint = composite.AddFunction(FunctionType.Checkpoint);
            FunctionEntity playerSpawn = composite.AddFunction("ARCHTYPES\\SCRIPT\\MISSION\\SPAWNPOSITIONSELECT");

            checkpoint.AddParameter("is_first_checkpoint", new cBool(true));
            checkpoint.AddParameter("section", new cString("Entry"));

            checkpoint.AddParameterLink("finished_loading", playerSpawn, "Spawn player");

            FunctionEntity objective = composite.AddFunction(FunctionType.SetPrimaryObjective);
            objective.AddParameter("title", new cString("Where Stevies??"));
            objective.AddParameter("additional_info", new cString("Go find him"));
            checkpoint.AddParameterLink("finished_loading", objective, "trigger");

            commands.Save();
        }

        /*
         * First example of creating a new COMMANDS.PAK file using CathodeLib
         */
        public static void MainCreateNewCommandsPak() {
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
